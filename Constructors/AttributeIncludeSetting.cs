namespace Penguin.Reflection.Serialization.Constructors
{
    /// <summary>
    /// Defines the style of include to use when serializing attributes
    /// </summary>
    public enum AttributeIncludeSetting
    {
        /// <summary>
        /// All attributes are serialized
        /// </summary>
        All,

        /// <summary>
        /// Attributes are serialized as long as they dont exist in the constructor blacklist
        /// </summary>
        BlackList,

        /// <summary>
        /// Attributes are only serialized if the type exists in the whitelist
        /// </summary>
        WhiteList,

        /// <summary>
        /// All attributes are skipped during serialization
        /// </summary>
        None
    }
}