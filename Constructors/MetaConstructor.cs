using Penguin.Reflection.Serialization.Abstractions.Constructors;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Constructors
{
    /// <summary>
    /// Defines the style of include to use when serializing attributes
    /// </summary>
    public enum AttributeIncludeSettings
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

    /// <summary>
    /// Specifies how attributes should be matched to the whitelist/blacklist for inclusion
    /// </summary>
    public enum AttributeMatchSettings
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

    /// <summary>
    /// Class to manage settings and resources used during recursive serialization of objects
    /// </summary>
    public class MetaConstructor
    {
        #region Properties

        /// <summary>
        /// Any exceptions that occured during serialization are listed here along with the Id(i) of the object that threw the error
        /// </summary>
        public Dictionary<string, int> Exceptions { get; set; }

        /// <summary>
        /// Contains a list of references used for rehydrating the object tree. This exists to keep the serialized (json) size as small as possible by avoiding the
        /// need for reference handling as well as avoiding struct duplication
        /// </summary>
        public Dictionary<object, IAbstractMeta> Meta { get; set; }

        /// <summary>
        /// Contains the settings to be used when constructing the serialized object tree
        /// This is a reference so that the settings can be copied as the MetaConstructor clones itself down the tree
        /// </summary>
        public ConstructorSettings Settings { get; set; }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Reference class used to hold settings used during object serialization
        /// </summary>
        public class ConstructorSettings
        {
            #region Properties

            /// <summary>
            /// When using AttributeIncludeSettings.Blacklist, this list should contain a list of attribute types to skip serializing
            /// </summary>
            public List<RType> AttributeBlacklist
            {
                get
                {
                    return _AttributeBlacklist;
                }
                set
                {
                    if (_AttributeWhitelist is null)
                    {
                        AttributeIncludeSettings = AttributeIncludeSettings.BlackList;
                        _AttributeBlacklist = value;
                    }
                    else
                    {
                        throw new Exception("Can not set attribute blacklist when attribute whitelist is set");
                    }
                }
            }

            /// <summary>
            /// The style of setting used to determine whether or not attributes are serialized
            /// </summary>
            public AttributeIncludeSettings AttributeIncludeSettings { get; set; }

            /// <summary>
            /// Specifies the type of match to use when determining whether or not to serialize assemblies
            /// </summary>
            public AttributeMatchSettings AttributeMatchSettings { get; set; }

            /// <summary>
            /// When using AttributeIncludeSettings.Whitelist, this list should contain a list of attribute types to serialize
            /// </summary>
            public List<RType> AttributeWhitelist
            {
                get
                {
                    return _AttributeWhitelist;
                }
                set
                {
                    if (_AttributeBlacklist is null)
                    {
                        AttributeIncludeSettings = AttributeIncludeSettings.WhiteList;
                        _AttributeWhitelist = value;
                    }
                    else
                    {
                        throw new Exception("Can not set attribute whitelist when attribute blacklist is set");
                    }
                }
            }

            /// <summary>
            /// Ignores properties that match (!thisProperty.ReflectedType.IsVisible &amp;&amp; thisProperty.ReflectedType.Assembly != this.Cache.CallingAssembly). Default True
            /// </summary>
            public bool IgnoreHiddenForeignTypes { get; set; }

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
            public ConstructorSettings()
            {
                this.IgnoreHiddenForeignTypes = true;
                this.IgnoreNullDeclaringType = true;
                this.IgnoreInheritedProperties = false;
                this.AttributeMatchSettings = AttributeMatchSettings.Equality;
                this.IgnoreTypes = new List<RType>
                {
                    typeof(System.Type),
                    typeof(System.Reflection.MemberInfo)
                };
                this.PropertyGetterOverride = new Dictionary<string, Func<object, object>>();
                this.TypeGetterOverride = new List<Func<RType, RType>>();
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Function that all type resolutions pass through that can affect the return type
            /// Usefull for unshelling proxy types. If multiple, they are chained, so be careful
            /// </summary>
            /// <param name="func">The function that the type passes through</param>
            public void AddTypeGetterOverride(Func<RType, RType> func) => this.TypeGetterOverride.Add(func);

            /// <summary>
            /// Overrides the method used to resolve an object property, for returning a custom type (or null to skip)
            /// </summary>
            /// <param name="pi">The PropertyInfo of the property to be overridden</param>
            /// <param name="func">The function to call instead of PropertyInfo.GetValue. Object in is the Parent of the property, object out is the Property Value</param>
            public void OverridePropertyGetter(PropertyInfo pi, Func<object, object> func) => this.PropertyGetterOverride.Add(GetUniquePropertyId(pi), func);

            #endregion Methods

            internal Dictionary<string, Func<object, object>> PropertyGetterOverride { get; set; }

            internal List<Func<RType, RType>> TypeGetterOverride { get; set; }

            internal object GetOwner() => this.Owner;

            internal bool IsAttributeMatch(Type typeA, Type typeB)
            {
                switch (AttributeMatchSettings)
                {
                    case AttributeMatchSettings.Equality:
                        return typeA == typeB;

                    case AttributeMatchSettings.AssemblyQualifiedName:
                        return typeA.AssemblyQualifiedName == typeB.AssemblyQualifiedName;

                    case AttributeMatchSettings.Name:
                        return typeA.Name == typeB.Name;

                    case AttributeMatchSettings.FullName:
                        return typeA.FullName == typeB.FullName;

                    default:
                        throw new Exception("Undefined attribute match type settings");
                }
            }

            internal bool IsOwner(object o) => this.Owner == o;

            internal bool ShouldAddAttribute(RType attributeType)
            {
                switch (AttributeIncludeSettings)
                {
                    case AttributeIncludeSettings.All:
                        return true;

                    case AttributeIncludeSettings.WhiteList:
                        return AttributeWhitelist != null && AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType));

                    case AttributeIncludeSettings.BlackList:
                        return AttributeBlacklist is null || !AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType));

                    case AttributeIncludeSettings.None:
                        return false;

                    default:
                        throw new Exception("Undefined attribute serialization settings");
                }
            }

            internal void TrySetOwner(object o) => this.Owner = this.Owner ?? o;

            private List<RType> _AttributeBlacklist { get; set; }
            private List<RType> _AttributeWhitelist { get; set; }
            private object Owner { get; set; }
        }

        #endregion Classes

        #region Constructors

        /// <summary>
        /// Creates the default instance of the MetaConstructor
        /// </summary>
        public MetaConstructor()
        {
            this.Meta = this.Meta ?? new Dictionary<object, IAbstractMeta>();
            this.Settings = this.Settings ?? new MetaConstructor.ConstructorSettings();
            this.Cache = new CacheContainer();
            this.TypeProperties = new Dictionary<RType, IList<PropertyInfo>>();
            this.Exceptions = new Dictionary<string, int>();
        }

        /// <summary>
        /// Creates a MetaConstructor instance for a single object
        /// </summary>
        /// <param name="o">The object to serialize</param>
        public MetaConstructor(object o) : this(new ObjectConstructor(null, null, o))
        {
        }

        /// <summary>
        /// Creates a MetaConstructor for an object with a declared property name, to be used in MetaSerializing a single property of an existing object
        /// </summary>
        /// <param name="PropertyName">The name of the property to give</param>
        /// <param name="o">The object to serialize</param>
        public MetaConstructor(string PropertyName, object o = null) : this(o)
        {
            this.Property = new MetaProperty()
            {
                Name = PropertyName
            };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Sets the input object as the top level owner of the metadata tree
        /// </summary>
        /// <param name="o">The object to set</param>
        public void ClaimOwnership(object o) => this.Settings.TrySetOwner(o);

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="o">The object to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>
        public MetaConstructor Clone(object o) => this.Clone(new ObjectConstructor(null, null, o));

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="p">The property info to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>
        public MetaConstructor Clone(PropertyInfo p) => this.Clone(new ObjectConstructor(p, null, null));

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="t">The Type to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>
        public MetaConstructor Clone(RType t) => this.Clone(new ObjectConstructor(null, t, null));

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="oc">The ObjectConstructor to use when populating the internal values</param>
        /// <returns>A new instance of a MetaConstructor</returns>
        public MetaConstructor Clone(ObjectConstructor oc) => new MetaConstructor(oc) { Meta = Meta, Settings = Settings, Cache = Cache, Exceptions = Exceptions };

        #endregion Methods

        internal CacheContainer Cache { get; set; }
        internal object Object { get; set; }

        internal MetaProperty Property { get; set; }

        internal PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal RType Type { get; set; }

        internal class CacheContainer
        {
            #region Properties

            public Dictionary<System.Attribute, int> Attributes { get; set; }

            public Assembly CallingAssembly
            {
                get
                {
                    if (this.callingAssembly is null)
                    {
                        this.callingAssembly = Assembly.GetCallingAssembly();
                    }

                    return this.callingAssembly;
                }
            }

            #endregion Properties

            #region Constructors

            public CacheContainer()
            {
                Attributes = new Dictionary<System.Attribute, int>();
            }

            #endregion Constructors

            private Assembly callingAssembly { get; set; }
        }

        internal MetaConstructor(ObjectConstructor oc) : this()
        {
            this.Type = oc.Type;
            this.PropertyInfo = oc.PropertyInfo;
            this.Object = oc.Object;
        }

        internal static object GetKey(object o)
        {
            if (o is PropertyInfo || o is RType || o is System.Attribute || o is KeyGroup || o is string)
            {
                return o;
            }

            if (o is MetaWrapper)
            {
                return (o as MetaWrapper).GetKey();
            }

            throw new Exception("Unsupported object Key Get");
        }

        internal static string GetUniquePropertyId(PropertyInfo pi) => $"{pi.DeclaringType.FullName}.{pi.PropertyType.FullName}.{pi.Name}";

        internal int AddException(string Message)
        {
            if (!this.Exceptions.ContainsKey(Message))
            {
                this.Exceptions.Add(Message, this.Exceptions.Count);
            }

            return this.Exceptions[Message];
        }

        internal int Claim(string s)
        {
            if (!this.Meta.ContainsKey(s))
            {
                int Index = this.Meta.Count;

                this.Meta.Add(s, new StringHolder() { v = s, i = Index });
            }

            return this.Meta[s].i;
        }

        internal AbstractMeta Claim(object o)
        {
            AbstractMeta placeHolder = new AbstractMeta()
            {
                i = this.Meta.Count
            };

            this.Meta.Add(GetKey(o), placeHolder);

            return placeHolder;
        }

        internal bool Contains(object o) => this.Meta.ContainsKey(GetKey(o));

        internal int GetId(object o)
        {
            if (o is null)
            {
                return -1;
            }
            else
            {
                int ToReturn = this.Meta[GetKey(o)].i;

                return ToReturn;
            }
        }

        internal IList<PropertyInfo> GetProperties(RType t = null)
        {
            RType type = t ?? this.Type ?? this.Object.GetType();

            if (!TypeProperties.ContainsKey(type))
            {
                TypeProperties.Add(type, Penguin.Reflection.Cache.GetProperties(type).Where(this.Validate).ToList());
            }

            return TypeProperties[type];
        }

        internal object GetValue(PropertyInfo pi)
        {
            if (this.Settings.PropertyGetterOverride.ContainsKey(GetUniquePropertyId(pi)))
            {
                return this.Settings.PropertyGetterOverride[GetUniquePropertyId(pi)].Invoke(this.Object);
            }
            else
            {
                return pi.GetValue(this.Object);
            }
        }

        internal bool IsOwner(object o) => this.Settings.IsOwner(o);

        internal void UpdateClaim(AbstractMeta a, object o)
        {
            if (a.i < 0)
            {
                throw new Exception("Object with ID < 0 should not be dehydrated");
            }
            this.Meta[GetKey(o)] = a;
        }

        internal bool Validate(PropertyInfo thisProperty)
        {
            bool HiddenType = !thisProperty.ReflectedType.IsVisible && thisProperty.ReflectedType.Assembly != this.Cache.CallingAssembly;

            bool skip = false;

            skip = skip || thisProperty.GetGetMethod() == null;
            skip = skip || (thisProperty.DeclaringType is null && this.Settings.IgnoreNullDeclaringType);
            skip = skip || this.Settings.IgnoreTypes.Contains(thisProperty.DeclaringType);
            skip = skip || (HiddenType && this.Settings.IgnoreHiddenForeignTypes);
            skip = skip || thisProperty.GetIndexParameters().Any();

            return !skip;
        }

        private Dictionary<RType, IList<PropertyInfo>> TypeProperties { get; set; }
    }
}