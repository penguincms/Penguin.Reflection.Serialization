using Penguin.Reflection.Serialization.Abstractions.Constructors;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Extensions;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Constructors
{
    /// <summary>
    /// Class to manage settings and resources used during recursive serialization of objects
    /// </summary>
    public class MetaConstructor
    {
        private const string IdLess0SerializationExceptionMessage = "Object with ID < 0 should not be dehydrated";
        private const string UnsupportedObjectKeyGetMessage = "Unsupported object Key Get";

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
        public MetaConstructorSettings Settings { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates the default instance of the MetaConstructor
        /// </summary>
        public MetaConstructor()
        {
            this.Meta = this.Meta ?? new Dictionary<object, IAbstractMeta>();
            this.Settings = this.Settings ?? new MetaConstructorSettings();
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
        public MetaConstructor Clone(ObjectConstructor oc)
        {
            Contract.Requires(oc != null);

            return new MetaConstructor(oc) { Meta = Meta, Settings = Settings, Cache = Cache, Exceptions = Exceptions };
        }

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

            throw new Exception(UnsupportedObjectKeyGetMessage);
        }

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
                TypeProperties.Add(type, Penguin.Reflection.TypeCache.GetProperties(type).Where(this.Validate).ToList());
            }

            return TypeProperties[type];
        }

        internal object GetValue(PropertyInfo pi)
        {
            if (this.Settings.PropertyGetterOverride.ContainsKey(pi.GetUniquePropertyId()))
            {
                return this.Settings.PropertyGetterOverride[pi.GetUniquePropertyId()].Invoke(this.Object);
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
                throw new Exception(IdLess0SerializationExceptionMessage);
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