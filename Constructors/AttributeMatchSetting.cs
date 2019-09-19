namespace Penguin.Reflection.Serialization.Constructors
{
    /// <summary>
    /// Specifies how attributes should be matched to the whitelist/blacklist for inclusion
    /// </summary>
    public enum AttributeMatchSetting
    {
        /// <summary>
        /// Uses == on the type
        /// </summary>
        Equality,

        /// <summary>
        /// Matches based on type name only
        /// </summary>
        Name,

        /// <summary>
        /// Matches based on AssemblyQualifiedName
        /// </summary>
        AssemblyQualifiedName,

        /// <summary>
        /// Matches based on full name
        /// </summary>
        FullName
    }
}