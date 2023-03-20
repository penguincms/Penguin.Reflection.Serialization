using Loxifi;
using Penguin.Extensions.Collections;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Objects;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Extensions;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections.Generic;
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
        private const string ID_LESS_0_SERIALIZATION_EXCEPTION_MESSAGE = "Object with ID < 0 should not be dehydrated";
        private const string UNSUPPORTED_OBJECT_KEY_GET_MESSAGE = "Unsupported object Key Get";

        #region Properties

        /// <summary>
        /// Any exceptions that occured during serialization are listed here along with the Id(i) of the object that threw the error
        /// </summary>
        public Dictionary<string, int> Exceptions { get; private set; } = new Dictionary<string, int>();

        /// <summary>
        /// Contains a list of references used for rehydrating the object tree. This exists to keep the serialized (json) size as small as possible by avoiding the
        /// need for reference handling as well as avoiding struct duplication
        /// </summary>
        public Dictionary<object, IHydratable> Meta { get; private set; } = new Dictionary<object, IHydratable>();

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
            Settings ??= new MetaConstructorSettings();
            Cache = new CacheContainer();
            TypeProperties = new Dictionary<RType, IList<PropertyInfo>>();
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
            Property = new MetaProperty()
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
        public void ClaimOwnership(object o)
        {
            Settings.TrySetOwner(o);
        }

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="o">The object to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>

        public MetaConstructor Clone(object o)
        {
            return Clone(new ObjectConstructor(null, null, o));
        }

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="p">The property info to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>

        public MetaConstructor Clone(PropertyInfo p)
        {
            return Clone(new ObjectConstructor(p, null, null));
        }

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="t">The Type to use as the basis for serialization</param>
        /// <returns>A new instance of a MetaConstructor</returns>

        public MetaConstructor Clone(RType t)
        {
            return Clone(new ObjectConstructor(null, t, null));
        }

        /// <summary>
        /// Clones an instance of this constructor
        /// </summary>
        /// <param name="oc">The ObjectConstructor to use when populating the internal values</param>
        /// <returns>A new instance of a MetaConstructor</returns>
        public MetaConstructor Clone(ObjectConstructor oc)
        {
            if (oc is null)
            {
                throw new ArgumentNullException(nameof(oc));
            }

            MetaConstructor clone = new(oc)
            {
                Settings = Settings,
                Cache = Cache,
                Meta = Meta,
                Exceptions = Exceptions
            };

            return clone;
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

        private Dictionary<RType, IList<PropertyInfo>> TypeProperties { get; set; }

        internal class CacheContainer
        {
            #region Properties

            public Dictionary<System.Attribute, int> Attributes { get; set; }

            public Assembly CallingAssembly
            {
                get
                {
                    callingAssembly ??= Assembly.GetCallingAssembly();

                    return callingAssembly;
                }
            }

            #endregion Properties

            #region Constructors

            public CacheContainer()
            {
                Attributes = new Dictionary<System.Attribute, int>();
            }

            #endregion Constructors

            private Assembly callingAssembly;
        }

        internal MetaConstructor(ObjectConstructor oc) : this()
        {
            Type = oc.Type;
            PropertyInfo = oc.PropertyInfo;
            Object = oc.Object;
        }

        internal static object GetKey(object o)
        {
            return o is PropertyInfo || o is RType || o is System.Attribute || o is KeyGroup || o is string
                ? o
                : o is MetaWrapper ? (object)(o as MetaWrapper).GetKey() : throw new Exception(UNSUPPORTED_OBJECT_KEY_GET_MESSAGE);
        }

        internal int AddException(string Message)
        {
            if (!Exceptions.ContainsKey(Message))
            {
                Exceptions.Add(Message, Exceptions.Count);
            }

            return Exceptions[Message];
        }

        internal int Claim(string s)
        {
            if (!Meta.ContainsKey(s))
            {
                int Index = Meta.Count;

                Meta.Add(s, new StringHolder() { V = s, I = Index });
            }

            return Meta[s].I;
        }

        internal AbstractMeta Claim(object o)
        {
            AbstractMeta placeHolder = new()
            {
                I = Meta.Count
            };

            Meta.Add(GetKey(o), placeHolder);

            return placeHolder;
        }

        internal bool Contains(object o)
        {
            return Meta.ContainsKey(GetKey(o));
        }

        internal int GetId(object o)
        {
            if (o is null)
            {
                return -1;
            }
            else
            {
                int ToReturn = Meta[GetKey(o)].I;

                return ToReturn;
            }
        }

        internal IList<PropertyInfo> GetProperties(RType t = null)
        {
            RType type = t ?? Type ?? Object.GetType();

            if (!TypeProperties.ContainsKey(type))
            {
                TypeProperties.Add(type, TypeFactory.GetProperties(type).Where(Validate).ToList());
            }

            return TypeProperties[type];
        }

        internal object GetValue(PropertyInfo pi)
        {
            return Settings.PropertyGetterOverride.TryGetValue(pi.GetUniquePropertyId(), out Func<object, object> value) ? value.Invoke(Object)
                : pi.GetValue(Object);
        }

        internal bool IsOwner(object o)
        {
            return Settings.IsOwner(o);
        }

        internal void UpdateClaim(AbstractMeta a, object o)
        {
            if (a.I < 0)
            {
                throw new Exception(ID_LESS_0_SERIALIZATION_EXCEPTION_MESSAGE);
            }

            Meta[GetKey(o)] = a;
        }

        internal bool Validate(PropertyInfo thisProperty)
        {
            bool HiddenType = !thisProperty.ReflectedType.IsVisible && thisProperty.ReflectedType.Assembly != Cache.CallingAssembly;

            bool skip = false;

            skip = skip || thisProperty.GetGetMethod() == null;
            skip = skip || (thisProperty.DeclaringType is null && Settings.IgnoreNullDeclaringType);
            skip = skip || Settings.IgnoreTypes.Contains(thisProperty.DeclaringType);
            skip = skip || (HiddenType && Settings.IgnoreHiddenForeignTypes);
            skip = skip || thisProperty.GetIndexParameters().Any();
            skip = skip || (Settings.IgnoreObjectProperties && thisProperty.PropertyType == typeof(object));

            return !skip;
        }
    }
}