using Penguin.Reflection.Serialization.Abstractions.Interfaces;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// Converts a string into a MetaType so that it can be cached by a constructor and rehydrated later.
    /// Removes a lot of redundancy on large recursive trees
    /// </summary>
    public class StringHolder : AbstractMeta, IStringHolder
    {
        #region Properties

        /// <summary>
        /// The value of the underlying string
        /// </summary>
        public string V { get; set; }

        #endregion Properties
    }
}