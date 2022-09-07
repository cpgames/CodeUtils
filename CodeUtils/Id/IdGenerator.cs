using System;

namespace cpGames.core
{
    /// <summary>
    /// Generates unique <see cref="Id" />s.
    /// </summary>
    public class IdGenerator
    {
        #region Fields
        private static readonly Random RND = new();
        private readonly int _retries;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// Typically only a single IdGenerator is necessary.
        /// </summary>
        /// <param name="retries">How many attempts to take generating an Id before giving up.</param>
        /// <exception cref="Exception"></exception>
        public IdGenerator(int retries = 1000)
        {
            _retries = retries;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generate a unique Id given the subset of existing Ids in <see cref="IIdProvider" />.
        /// </summary>
        /// <param name="provider">Provider to generate Id for.</param>
        /// <param name="id">
        /// Unique Id that is not present in provider.
        /// Invalid Id if max retries has been reached.
        /// </param>
        /// <returns>Success or Fail.</returns>
        public Outcome GenerateId(IIdProvider provider, out Id id)
        {
            lock (provider)
            {
                var retries = _retries;
                do
                {
                    var bytes = new byte[provider.IdSize];
                    RND.NextBytes(bytes);
                    id = new Id(bytes);
                } while (provider.HasId(id) && --retries > 0);
                if (_retries == 0)
                {
                    id = Id.INVALID;
                    return Outcome.Fail("Generating random id timed out.");
                }
                return Outcome.Success();
            }
        }
        #endregion
    }
}