﻿using Penguin.Reflection.Serialization.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private const string CantSetBlacklistMessage = "Can not set attribute blacklist when attribute whitelist is set";
        private const string CantSetWhitelesMessage = "Can not set attribute whitelist when attribute blacklist is set";
        private const string UndefinedAttributeSerializationSettingsMessage = "Undefined attribute serialization settings";
        private const string UndefinedMatchTypeMessage = "Undefined attribute match type settings";

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
                    AttributeIncludeSettings = AttributeIncludeSetting.BlackList;
                    _AttributeBlacklist = value;
                }
                else
                {
                    throw new UnauthorizedAccessException(CantSetBlacklistMessage);
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
            get
            {
                return _AttributeWhitelist;
            }
            set
            {
                if (_AttributeBlacklist is null)
                {
                    AttributeIncludeSettings = AttributeIncludeSetting.WhiteList;
                    _AttributeWhitelist = value;
                }
                else
                {
                    throw new Exception(CantSetWhitelesMessage);
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
        public MetaConstructorSettings()
        {
            this.IgnoreHiddenForeignTypes = true;
            this.IgnoreNullDeclaringType = true;
            this.IgnoreInheritedProperties = false;
            this.AttributeMatchSettings = AttributeMatchSetting.Equality;
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
        public void OverridePropertyGetter(PropertyInfo pi, Func<object, object> func)
        {
            Contract.Requires(pi != null);

            this.PropertyGetterOverride.Add(pi.GetUniquePropertyId(), func);
        }

        #endregion Methods

        internal Dictionary<string, Func<object, object>> PropertyGetterOverride { get; set; }

        internal List<Func<RType, RType>> TypeGetterOverride { get; set; }

        private List<RType> _AttributeBlacklist { get; set; }

        private List<RType> _AttributeWhitelist { get; set; }

        private object Owner { get; set; }

        internal object GetOwner() => this.Owner;

        internal bool IsAttributeMatch(Type typeA, Type typeB)
        {
            switch (AttributeMatchSettings)
            {
                case AttributeMatchSetting.Equality:
                    return typeA == typeB;

                case AttributeMatchSetting.AssemblyQualifiedName:
                    return typeA.AssemblyQualifiedName == typeB.AssemblyQualifiedName;

                case AttributeMatchSetting.Name:
                    return typeA.Name == typeB.Name;

                case AttributeMatchSetting.FullName:
                    return typeA.FullName == typeB.FullName;

                default:
                    throw new Exception(UndefinedMatchTypeMessage);
            }
        }

        internal bool IsOwner(object o) => this.Owner == o;

        internal bool ShouldAddAttribute(RType attributeType)
        {
            switch (AttributeIncludeSettings)
            {
                case AttributeIncludeSetting.All:
                    return true;

                case AttributeIncludeSetting.WhiteList:
                    return AttributeWhitelist != null && AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType));

                case AttributeIncludeSetting.BlackList:
                    return AttributeBlacklist is null || !AttributeWhitelist.Any(t => IsAttributeMatch(t, attributeType));

                case AttributeIncludeSetting.None:
                    return false;

                default:
                    throw new ArgumentException(UndefinedAttributeSerializationSettingsMessage);
            }
        }

        internal void TrySetOwner(object o) => this.Owner = this.Owner ?? o;
    }
}