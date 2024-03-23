using System;
using System.Collections.Generic;

namespace cpGames.core
{
    /// <summary>
    /// Simple implementation of IIdProvider
    /// </summary>
    public class IdContainer : IIdProvider, IIdGenerator
    {
        #region Fields
        protected readonly object _syncRoot = new();
        private readonly HashSet<Id> _ids = new();
        private readonly IdGenerator _generator = new();
        #endregion

        #region Constructors
        public IdContainer(byte idSize = 4)
        {
            if (idSize == 0)
            {
                throw new Exception("Id size can't be 0.");
            }
            IdSize = idSize;
        }
        #endregion

        #region IIdGenerator Members
        public Outcome GenerateId(out Id id, bool add = true)
        {
            var generateOutcome = _generator.GenerateId(this, out id);
            if (!generateOutcome)
            {
                return generateOutcome;
            }
            return add ? AddId(id) : Outcome.Success();
        }
        #endregion

        #region IIdProvider Members
        public byte IdSize { get; }

        public bool HasId(Id id)
        {
            lock (_syncRoot)
            {
                return _ids.Contains(id);
            }
        }
        #endregion

        #region Methods
        public Outcome AddId(Id id)
        {
            lock (_syncRoot)
            {
                return _ids.Add(id) ?
                    Outcome.Success() :
                    Outcome.Fail($"Id <{id}> already exists.", this);
            }
        }

        public Outcome RemoveId(Id id)
        {
            lock (_syncRoot)
            {
                return _ids.Remove(id) ?
                    Outcome.Success() :
                    Outcome.Fail($"Id <{id}> does not exist.", this);
            }
        }
        #endregion
    }
}