using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cpGames.core
{
    /// <summary>
    /// Represents a unique Id used with object collections.
    /// Can be serialized.
    /// Generated with <see cref="IdGenerator"/>
    /// </summary>
    public readonly struct Id
    {
        #region Fields
        public static Id INVALID = new();
        private readonly byte[]? _bytes;
        #endregion

        #region Properties
        public byte Length => (byte)(_bytes?.Length ?? 0);
        public bool IsValid => Bytes is { Length: > 0 };
        public byte[]? Bytes => _bytes;
        #endregion

        #region Constructors
        public Id(params byte[] bytes)
        {
            _bytes = bytes;
        }

        public Id(Id other)
        {
            if (other.IsValid)
            {
                _bytes = new byte[other._bytes!.Length];
                for (var i = 0; i < other._bytes.Length; i++)
                {
                    _bytes[i] = other._bytes[i];
                }
            }
            else
            {
                _bytes = null;
            }
        }

        public Id(string str)
        {
            var tokens =
                str.Replace("{", "")
                    .Replace("}", "")
                    .Split('-');
            if (tokens.All(token => token.Length == 2))
            {
                _bytes = tokens
                    .Select(x => Convert.ToByte(x, 16))
                    .ToArray();
            }
            else
            {
                _bytes = Encoding.ASCII.GetBytes(str);
            }
        }
        #endregion

        #region Methods
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] dest, byte[] src, long count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcpy(byte[] dest, byte[] src, long count);

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
            return Bytes != null ? Bytes[^1] : (byte)0;
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
            var bytes = new byte[4];
            memcpy(bytes, Bytes!, 4);
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

        public string ToString(bool fancy)
        {
            if (!IsValid)
            {
                return "INVALID";
            }
            if (fancy)
            {
                var str = string.Empty;
                for (var i = 0; i < Bytes!.Length - 1; i++)
                {
                    str += $"{Bytes[i]:X2}-";
                }
                str += $"{Bytes[^1]:X2}";
                return "{" + str + "}";
            }
            return Encoding.ASCII.GetString(Bytes!);
        }

        public override string ToString()
        {
            return ToString(true);
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
        #endregion
    }
}