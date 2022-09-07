using System;
using System.Reflection;

namespace cpGames.core
{
    /// <summary>
    /// Standardized Singleton pattern.
    /// <para />
    /// Usage: derive your class from Singleton.
    /// <para />
    /// Example:
    /// <code>MyClass : Singleton&lt;MyClass&gt;
    /// {
    ///     private MyClass() { }
    /// }</code>
    /// </summary>
    public class Singleton<T>
    {
        #region Fields
        private static T? _instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                var ctor =
                    typeof(T).GetConstructor(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                        null, Type.EmptyTypes, null);
                if (ctor != null)
                {
                    _instance = (T)ctor.Invoke(null);
                }
                else
                {
                    throw new Exception($"{typeof(T).Name} has no parameterless constructor");
                }
                return _instance;
            }
        }
        #endregion
    }
}