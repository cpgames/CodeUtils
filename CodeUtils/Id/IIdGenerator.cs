namespace cpGames.core
{
    public interface IIdGenerator
    {
        #region Methods
        Outcome GenerateId(out Id id, bool add = true);
        #endregion
    }
}