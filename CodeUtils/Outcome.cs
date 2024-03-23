using System.Collections.Generic;
using System.Linq;

namespace cpGames.core
{
    /// <summary>
    /// A useful construct for returning function success or failure with an attached error message.
    /// Can be chained using '&amp;&amp;' or '||' operators.
    /// <para />
    /// Example:
    /// <code>
    /// Outcome IsPositive(int n)
    /// {
    ///     if (n > 0)
    ///         return Outcome.Success();
    ///     return Outcome.Fail("n is not positive.");
    /// }
    /// 
    /// Outcome IsEven(int n)
    /// {
    ///     if (n % 2 == 0)
    ///         return Outcome.Success();
    ///     return Outcome.Fail("n is not even.");
    /// }
    /// 
    /// Outcome IsPositiveAndEven(int n)
    /// {
    ///     return IsPositive(n) &amp;&amp; IsEven(n);
    /// }
    /// </code>
    /// </summary>
    public readonly struct Outcome
    {
        #region Fields
        private static readonly Outcome EMPTY_FAIL = new(false, string.Empty, (object?)null);
        private static readonly Outcome EMPTY_SUCCESS = new(true, string.Empty, (object?)null);
        #endregion

        #region Properties
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public List<object> Sources { get; }
        #endregion

        #region Constructors
        private Outcome(bool success, string errorMessage, object? source)
        {
            IsSuccess = success;
            ErrorMessage = errorMessage;
            Sources = new List<object>();
            if (source != null)
            {
                Sources.Add(source);
            }
        }

        private Outcome(bool success, string errorMessage, List<object> sources)
        {
            IsSuccess = success;
            ErrorMessage = errorMessage;
            Sources = sources;
        }
        #endregion

        #region Methods
        public Outcome Append(object source)
        {
            if (IsSuccess)
            {
                return this;
            }
            if (Sources.LastOrDefault() == source)
            {
                return this;
            }
            var newSources = new List<object>(Sources)
            {
                source
            };
            return new Outcome(IsSuccess, ErrorMessage, newSources);
        }
        private bool Equals(Outcome other)
        {
            return IsSuccess == other.IsSuccess;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Outcome)obj);
        }

        public override int GetHashCode()
        {
            return IsSuccess.GetHashCode();
        }

        public static implicit operator bool(Outcome r)
        {
            return r.IsSuccess;
        }

        public static Outcome operator &(Outcome a, Outcome b)
        {
            return !a.IsSuccess ? a : b;
        }

        public static Outcome operator |(Outcome a, Outcome b)
        {
            if (a.IsSuccess)
            {
                return a;
            }
            if (b.IsSuccess)
            {
                return b;
            }
            var newSources = new List<object> { a.Sources, b.Sources };
            return new Outcome(false, $"{a.ErrorMessage}\n{b.ErrorMessage}", newSources);
        }

        public static bool operator ==(Outcome a, Outcome b)
        {
            return a.IsSuccess == b.IsSuccess;
        }

        public static bool operator !=(Outcome a, Outcome b)
        {
            return !(a == b);
        }

        public static bool operator false(Outcome a)
        {
            return !a.IsSuccess;
        }

        public static bool operator true(Outcome a)
        {
            return a.IsSuccess;
        }

        public override string ToString()
        {
            return ErrorMessage;
        }

        public static Outcome Success()
        {
            return EMPTY_SUCCESS;
        }

        public static Outcome Fail(string errorMessage, object? source)
        {
            return new Outcome(false, errorMessage, source);
        }

        public static Outcome Fail()
        {
            return EMPTY_FAIL;
        }
        #endregion
    }
}