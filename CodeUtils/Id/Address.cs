using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace cpGames.core
{
    /// <summary>
    /// Address is an aggregation of multiple Ids. It is useful for navigating hierarchical
    /// entity structure.
    /// Instead of holding an array of Ids, Address is a flattened byte array
    /// that contains id length followed by id's bytes.
    /// e.g. Id1 = 12, Id2 = 234, Id3 = 5678,
    /// Address = [2]12[3]234[4]5678
    /// </summary>
    public readonly struct Address
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] dest, byte[] src, long count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcpy(byte[] dest, byte[] src, long count);

        #region Fields
        public static Address INVALID = new();
        private readonly byte[]? _bytes;
        #endregion

        #region Properties
        public byte[]? Bytes => _bytes;
        public int IdCount { get; }
        public bool IsValid => Bytes is { Length: > 0 };
        #endregion

        #region Constructors
        public Address(params Id[] ids)
        {
            var size = ids.Length;
            foreach (var id in ids)
            {
                if (!id.IsValid)
                {
                    throw new Exception("Id is invalid.");
                }
                size += id.Length;
            }
            _bytes = new byte[size];
            var i = 0;
            foreach (var id in ids)
            {
                _bytes[i++] = id.Length;
                id.Bytes!.CopyTo(_bytes, i);
                i += id.Length;
            }
            IdCount = ids.Length;
        }

        public Address(byte[] bytes)
        {
            _bytes = bytes;
            IdCount = 0;
            if (bytes is { Length: > 0 })
            {
                var i = 0;
                while (i < bytes.Length)
                {
                    var size = bytes[i++];
                    if (size > 0)
                    {
                        i += size;
                        IdCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public Address(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                _bytes = null;
                IdCount = 0;
                return;
            }
            var idTokens = str.Split(':');
            var ids = new List<Id>();
            var size = idTokens.Length;
            foreach (var idToken in idTokens)
            {
                var id = new Id(idToken);
                if (!id.IsValid)
                {
                    throw new Exception("Id is invalid.");
                }
                ids.Add(id);
                size += id.Length;
            }
            _bytes = new byte[size];
            var i = 0;
            foreach (var id in ids)
            {
                _bytes[i++] = id.Length;
                id.Bytes!.CopyTo(_bytes, i);
                i += id.Length;
            }
            IdCount = ids.Count;
        }

        public Address(Address other)
        {
            if (other.IsValid)
            {
                _bytes = new byte[other.Bytes!.Length];
                other.Bytes.CopyTo(_bytes, 0);
                IdCount = other.IdCount;
            }
            else
            {
                _bytes = null;
                IdCount = 0;
            }
        }

        internal Address(byte[] bytes, int idCount)
        {
            _bytes = bytes;
            IdCount = idCount;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if other Address is a subset of current Address.
        /// </summary>
        public bool Contains(Address other)
        {
            if (!IsValid || !other.IsValid)
            {
                return false;
            }
            var iSelf = 0;
            var iOther = 0;
            while (iSelf < Bytes!.Length && iOther < other.Bytes!.Length)
            {
                var sizeSelf = Bytes[iSelf++];
                var sizeOther = other.Bytes![iOther];
                if (sizeSelf == sizeOther)
                {
                    iOther++;
                    for (var iInner = 0; iInner < sizeSelf; iInner++)
                    {
                        // skip to the end of the id in case of no match
                        if (Bytes[iSelf] != other.Bytes[iOther])
                        {
                            iSelf += sizeSelf - iInner;
                            // reset other index to 0 to start matching from the beginning
                            iOther = 0;
                            break;
                        }
                        iSelf++;
                        iOther++;
                    }
                }
                else
                {
                    iSelf += sizeSelf;
                }
            }
            return iOther == other.Bytes!.Length;
        }

        /// <summary>
        /// Append Id to the end of this Address.
        /// </summary>
        public Address Append(Id id)
        {
            if (!id.IsValid)
            {
                throw new Exception("Id is invalid.");
            }
            var size = Bytes?.Length ?? 0;
            var bytes = new byte[size + 1 + id.Length];
            if (size > 0)
            {
                Bytes!.CopyTo(bytes, 0);
            }
            bytes[size] = id.Length;
            id.Bytes!.CopyTo(bytes, size + 1);
            return new Address(bytes, IdCount + 1);
        }

        public List<Id> GetIds()
        {
            var ids = new List<Id>();
            var i = 0;

            if (IsValid)
            {
                while (i < Bytes!.Length)
                {
                    var size = Bytes![i++];
                    var bytes = new byte[size];
                    Array.Copy(
                        Bytes,
                        i,
                        bytes,
                        0,
                        bytes.Length);
                    ids.Add(new Id(bytes));
                    i += bytes.Length;
                }
            }
            return ids;
        }

        public Id GetId(int offset)
        {
            if (!IsValid || offset >= IdCount)
            {
                return Id.INVALID;
            }

            var i = 0;
            var n = 0;
            int size;
            while (i < Bytes!.Length && n++ < offset)
            {
                size = Bytes[i++];
                i += size;
            }
            if (i == Bytes.Length)
            {
                return Id.INVALID;
            }
            size = Bytes[i++];
            var bytes = new byte[size];
            Array.Copy(
                Bytes,
                i,
                bytes,
                0,
                bytes.Length);
            return new Id(bytes);
        }

        public Id GetFirstId()
        {
            return GetId(0);
        }

        public Id GetLastId()
        {
            return GetId(IdCount - 1);
        }

        public byte GetLastByte()
        {
            return !IsValid ? (byte)0 : Bytes![Bytes.Length - 1];
        }

        public bool IsPartialAddress(Address fullAddress)
        {
            if (Bytes == null && fullAddress.Bytes == null)
            {
                return true;
            }
            if (Bytes == null || fullAddress.Bytes == null)
            {
                return false;
            }
            return memcmp(Bytes, fullAddress.Bytes, Bytes.Length) == 0;
        }

        private bool Equals(Address other)
        {
            if (Bytes == null && other.Bytes == null)
            {
                return true;
            }
            if (Bytes == null || other.Bytes == null)
            {
                return false;
            }
            if (Bytes.Length != other.Bytes.Length)
            {
                return false;
            }
            return memcmp(Bytes, other.Bytes, Bytes.Length) == 0;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Address)obj);
        }

        public override int GetHashCode()
        {
            if (!IsValid)
            {
                return 0;
            }
            var bytes = new byte[4];
            memcpy(bytes, Bytes!, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static bool operator ==(Address lhs, Address rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Address lhs, Address rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return !IsValid ? string.Empty : GetIds().ToString(":");
        }

        public static object Deserialize(byte[] data)
        {
            return new Address(data);
        }

        public static byte[]? Serialize(object obj)
        {
            var address = (Address)obj;
            return address.Bytes;
        }
        #endregion
    }
}