using Penguin.Reflection.Serialization.Abstractions.Interfaces;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// Object used for serializing/holding an enum value representation
    /// </summary>
    public class EnumValue : IEnumValue
    {
        #region Properties

        /// <summary>
        /// The name of the option
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Value of the option
        /// </summary>
        public string Value { get; set; }

        #endregion Properties
    }
}