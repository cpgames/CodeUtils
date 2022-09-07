namespace cpGames.core
{
    public interface IIdProvider
    {
        #region Properties
        byte IdSize { get; }
        #endregion

        #region Methods
        bool HasId(Id id);
        #endregion
    }
}