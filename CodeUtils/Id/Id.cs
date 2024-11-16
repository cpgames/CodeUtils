using System;
using System.Runtime.InteropServices;

namespace cpGames.core
{
    /// <summary>
    ///     Represents a unique Id used with object collections.
    ///     Can be serialized.
    ///     Generated with <see cref="IdGenerator" />
    /// </summary>
    public readonly struct Id : IComparable<Id>
    {
        #region Fields
        public static Id INVALID = new();
        #endregion

        #region Properties
        public byte Length => (byte)(Bytes?.Length ?? 0);
        public bool IsValid => Bytes is { Length: > 0 };
        public byte[]? Bytes { get; }
        #endregion

        #region Constructors
        public Id(params byte[] bytes)
        {
            Bytes = bytes;
        }

        public Id(Id other)
        {
            if (other.IsValid)
            {
                Bytes = new byte[other.Bytes!.Length];
                for (var i = 0; i < other.Bytes.Length; i++)
                {
                    Bytes[i] = other.Bytes[i];
                }
            }
            else
            {
                Bytes = null;
            }
        }

        public Id(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                Bytes = null;
            }
            else
            {
                Bytes = new byte[str.Length / 2];
                for (var i = 0; i < str.Length; i += 2)
                {
                    var byteStr = str.Substring(i, 2);
                    var b = Convert.ToByte(byteStr, 16);
                    Bytes[i / 2] = b;
                }
            }
        }
        #endregion

        #region Methods
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] dest, byte[] src, long count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcpy(byte[] dest, byte[] src, long count);

        public static Outcome TryParse(string str, out Id id)
        {
            id = INVALID;
            if (str.Length == 0 || str == "0")
            {
                return Outcome.Success();
            }
            if (str.Length % 2 != 0)
            {
                return Outcome.Fail("Id string is not a multiple of 2");
            }
            try
            {
                id = new Id(str);
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }

            return Outcome.Success();
        }

        private bool Equals(Id other)
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
            //return !Bytes.Where((t, i) => t != other.Bytes[i]).Any();
            return memcmp(Bytes, other.Bytes, Bytes.Length) == 0;
        }

        public byte GetLastByte()
        {
            return Bytes != null ?
                Bytes[Bytes.Length - 1] :
                (byte)0;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Id)obj);
        }

        public override int GetHashCode()
        {
            if (!IsValid)
            {
                return 0;
            }
            var bytes = new byte[] { 0, 0, 0, 0 };
            memcpy(bytes, Bytes!, Bytes!.Length);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static bool operator ==(Id lhs, Id rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Id lhs, Id rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Id(byte b)
        {
            return new Id(b);
        }

        public static implicit operator Id(byte[] bytes)
        {
            return new Id(bytes);
        }

        public override string ToString()
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            var str = string.Empty;
            for (var i = 0; i < Bytes!.Length; i++)
            {
                str += $"{Bytes[i]:X2}";
            }
            return str;
        }

        public static object Deserialize(byte[] data)
        {
            return new Id(data);
        }

        public static byte[]? Serialize(object obj)
        {
            var id = (Id)obj;
            return id.Bytes;
        }

        public int CompareTo(Id other)
        {
            if (Bytes == null && other.Bytes == null)
            {
                return 0;
            }
            if (Bytes == null)
            {
                return -1;
            }
            if (other.Bytes == null)
            {
                return 1;
            }
            var minLength = Math.Min(Bytes.Length, other.Bytes.Length);
            for (var i = 0; i < minLength; i++)
            {
                var comparison = Bytes[i].CompareTo(other.Bytes[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }
            return Bytes.Length.CompareTo(other.Bytes.Length);
        }
        #endregion
    }
}