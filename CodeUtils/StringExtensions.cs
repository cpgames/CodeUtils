using System;
using System.Linq;
using System.Text;

namespace cpGames.core
{
    /// <summary>
    /// A set of useful extension methods for working with strings.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods
        /// <summary>
        /// Split string into substrings of fixed size, last substring can be shorter.
        /// </summary>
        public static string[] Split(this string s, int size)
        {
            if (s.Length <= size)
            {
                return new[] { s };
            }

            var n = s.Length / size + 1;
            var arr = new string[n];
            for (var i = 0; i < n; i++)
            {
                arr[i] = s.Substring(i * size, Math.Min(size, s.Length - i * size));
            }
            return arr;
        }

        /// <summary>
        /// Aggregate substring array into one large string
        /// </summary>
        public static string Join(this string[] s)
        {
            var str = string.Empty;
            return s.Length == 0 ?
                str :
                s.Aggregate(str, (current, subStr) => current + subStr);
        }

        /// <summary>
        /// Capitalize first character of a string.
        /// </summary>
        public static string Capitalize(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return char.ToUpper(s[0]) + s[1..];
        }

        /// <summary>
        /// Decapitalize first character of a string.
        /// </summary>
        public static string Decapitalize(this string s)
        {
            return char.ToLower(s[0]) + s[1..];
        }

        /// <summary>
        /// Truncates the string if it exceeds provided length.
        /// </summary>
        public static string Truncate(this string s, int length)
        {
            if (length < 1 || s.Length >= length)
            {
                return s;
            }
            return string.Concat(s[..length], "...");
        }

        /// <summary>
        /// Increment naming index.
        /// Example:
        /// <code>
        /// string name1 = "MyFile1";
        /// string name2 = name1.IncrementIndex(); // name2 is "MyFile2"
        /// </code>
        /// </summary>
        public static string IncrementIndex(this string s)
        {
            var builder = new StringBuilder();
            var letterPart = "";
            for (var i = s.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(s[i]))
                {
                    letterPart = s[..(i + 1)];
                    break;
                }
                builder.Insert(0, s[i]);
            }

            var index = 0;
            if (builder.Length > 0)
            {
                index = int.Parse(builder.ToString());
            }
            index++;
            letterPart += index.ToString();
            return letterPart;
        }
        #endregion
    }
}