using Penguin.Reflection.Serialization.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Constructors
{
    /// <summary>
    /// Reference class used to hold settings used during object serialization
    /// </summary>
    public class MetaConstructorSettings
    {
        private const string CANT_SET_BLACKLIST_MESSAGE = "Can not set attribute blacklist when attribute whitelist is set";
        private const string CANT_SET_WHITELES_MESSAGE = "Can not set attribute whitelist when attribute blacklist is set";
        private const string UNDEFINED_ATTRIBUTE_SERIALIZATION_SETTINGS_MESSAGE = "Undefined attribute serialization settings";
        private const string UNDEFINED_MATCH_TYPE_MESSAGE = "Undefined attribute match type settings";

        #region Properties

        /// <summary>
        /// When using AttributeIncludeSettings.Blacklist, this list should contain a list of attribute types to skip serializing
        /// </summary>

        public List<RType> AttributeBlacklist
        {
            get => _AttributeBlacklist;
            set
            {
                if (_AttributeWhitelist is null)
                {
                    AttributeIncludeSettings = AttributeIncludeSetting.BlackList;
                    _AttributeBlacklist = value;
                }
                else
                {
                    throw new UnauthorizedAccessException(CANT_SET_BLACKLIST_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The style of setting used to determine whether or not attributes are serialized
        /// </summary>
        public AttributeIncludeSetting AttributeIncludeSettings { get; set; }

        /// <summary>
        /// Specifies the type of match to use when determining whether or not to serialize assemblies
        /// </summary>
        public AttributeMatchSetting AttributeMatchSettings { get; set; }

        /// <summary>
        /// When using AttributeIncludeSettings.Whitelist, this list should contain a list of attribute types to serialize
        /// </summary>

        public List<RType> AttributeWhitelist
        {
            get => _AttributeWhitelist;
            set
            {
                if (_AttributeBlacklist is null)
                {
                    AttributeIncludeSettings = AttributeIncludeSetting.WhiteList;
                    _AttributeWhitelist = value;
                }
                else
                {
                    throw new Exception(CANT_SET_WHITELES_MESSAGE);
                }
            }
        }

        /// <summary>
        /// Ignores properties that match (!thisProperty.ReflectedType.IsVisible &amp;&amp; thisProperty.ReflectedType.Assembly != this.Cache.CallingAssembly). Default True
        /// </summary>
        public bool IgnoreHiddenForeignTypes { get; set; }

        /// <summary>
        /// Ignores properties that hold "object"
        /// </summary>
        public bool IgnoreObjectProperties { get; set; }

        /// <summary>
        /// Ignores property values holding System.Type information. (default True)
        /// </summary>
        public bool IgnoreTypeValueProperties { get; set; } = true;

        /// <summary>
        /// Do not traverse below the top level object when serializing properties. This setting is applied recursively and should only be used when serializing
        /// DTOs
        /// </summary>
        public bool IgnoreInheritedProperties { get; set; }

        /// <summary>
        /// Should ignore dynamic properties. Default True
        /// </summary>
        public bool IgnoreNullDeclaringType { get; set; }

        /// <summary>
        /// Types to ignore while serializing. Helpful for ignoring errors. By default ignores Type and MemberInfo since in most cases those are handled seperately
        /// </summary>
        public List<RType> IgnoreTypes { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the constructor settings
        /// </summary>
        public MetaConstructorSettings()
        {
            IgnoreHiddenForeignTypes = true;
            IgnoreNullDeclaringType = true;
            IgnoreInheritedProperties = false;
            AttributeMatchSettings = AttributeMatchSetting.Equality;
            IgnoreTypes = new List<RType>
                {
                    typeof(System.Type),
                    typeof(System.Reflection.MemberInfo)
                };
            PropertyGetterOverride = new Dictionary<string, Func<object, object>>();
            TypeGetterOverride = new List<Func<RType, RType>>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Function that all type resolutions pass through that can affect the return type
        /// Useful for unshelling proxy types. If multiple, they are chained, so be careful
        /// </summary>
        /// <param name="func">The function that the type passes through</param>
        public void AddTypeGetterOverride(Func<RType, RType> func)
        {
            TypeGetterOverride.Add(func);
        }

        /// <summary>
        /// Overrides the method used to resolve an object property, for returning a custom type (or null to skip)
        /// </summary>
        /// <param name="pi">The PropertyInfo of the property to be overridden</param>
        /// <param name="func">The function to call instead of PropertyInfo.GetValue. Object in is the Parent of the property, object out is the Property Value</param>
        public void OverridePropertyGetter(PropertyInfo pi, Func<object, object> func)
        {
            if (pi is null)
            {
                throw new ArgumentNullException(nameof(pi));
            }

            PropertyGetterOverride.Add(pi.GetUniquePropertyId(), func);
        }

        #endregion Methods

        private List<RType> _AttributeBlacklist;
        private List<RType> _AttributeWhitelist;

        internal Dictionary<string, Func<object, object>> PropertyGetterOverride { get; set; }

        internal List<Func<RType, RType>> TypeGetterOverride { get; set; }

        private object Owner { get; set; }

        /// <summary>
        /// A list of attribute types to skip based on namespace, since
        /// not all compiler generated attributes are accessible using type
        /// </summary>
        public HashSet<string> ForceSkipNameSpaces { get; internal set; } = new HashSet<string>()
        {
            "System.Runtime"
        };

        /// <summary>
        /// A list of attribute types to skip based on name, since
        /// not all compiler generated attributes are accessible using type
        /// </summary>
        public HashSet<string> ForceSkip { get; internal set; } = new HashSet<string>()
        {
            "System.AttributeUsageAttribute",
            "System.SerializableAttribute"
        };

        internal object GetOwner()
        {
            return Owner;
        }

        internal bool IsAttributeMatch(Type typeA, Type typeB)
        {
            return AttributeMatchSettings switch
            {
                AttributeMatchSetting.Equality => typeA == typeB,
                AttributeMatchSetting.AssemblyQualifiedName => typeA.AssemblyQualifiedName == typeB.AssemblyQualifiedName,
                AttributeMatchSetting.Name => typeA.Name == typeB.Name,
                AttributeMatchSetting.FullName => typeA.FullName == typeB.FullName,
                _ => throw new Exception(UNDEFINED_MATCH_TYPE_MESSAGE),
            };
        }

        internal bool IsOwner(object o)
        {
            return Owner == o;
        }

        internal bool ShouldAddAttribute(RType attributeType)
        {
            if (ForceSkipNameSpaces.Any(f => attributeType.FullName.IndexOf(f + ".") == 0))
            {
                //Debug.WriteLine("Force skipping attribute: " + attributeType.Name);
                return false;
            }

            return !ForceSkip.Contains(attributeType.FullName)
&& AttributeIncludeSettings switch
{
    AttributeIncludeSetting.All => true,
    AttributeIncludeSetting.WhiteList => AttributeWhitelist != null && AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType)),
    AttributeIncludeSetting.BlackList => AttributeBlacklist is null || !AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType)),
    AttributeIncludeSetting.None => false,
    _ => throw new ArgumentException(UNDEFINED_ATTRIBUTE_SERIALIZATION_SETTINGS_MESSAGE),
};
        }

        internal void TrySetOwner(object o)
        {
            Owner ??= o;
        }
    }
}