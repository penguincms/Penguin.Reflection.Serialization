﻿using Penguin.Debugging;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Extensions;
using Penguin.Reflection.Serialization.Abstractions.Constructors;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Constructors;
using Penguin.Reflection.Serialization.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// The most commonly used class, a fully serialized object for use in MetaSerialization systems
    /// </summary>
	public class MetaObject : AbstractMeta, ITypeInfo, IMetaObject, IAbstractMeta
    {
        #region Properties

        /// <summary>
        /// Top level exception cache for referencing exception messages. Used because recursive serialization has the potential to generate a LOT
        /// of rerundant errors
        /// </summary>
        public IDictionary<int, string> BuildExceptions { get; set; }

        /// <summary>
        /// If this object is a collection, this list contains the contents
        /// </summary>
        public IList<IMetaObject> CollectionItems { get; set; }

        /// <summary>
        /// If this object was not created due to an error, this should contain the ID of the error
        /// </summary>
        public int Exception { get; set; }

        /// <summary>
        /// True if this was the object used by the original constructor
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// MetaData cache for top level object to reference during deserialization. Not useful for anything but deserialization
        /// </summary>
        public IDictionary<int, IAbstractMeta> Meta { get; set; }

        /// <summary>
        /// True if the object was created using a null
        /// </summary>
        public bool Null { get; set; }

        /// <summary>
        /// A list of the accessible (child) properties for this object
        /// </summary>
        public IList<IMetaObject> Properties { get; set; }

        /// <summary>
        /// The parent property referencing this object. Unreliable in local types
        /// </summary>
        public IMetaProperty Property { get; set; }

        /// <summary>
        /// If this is a collection, this contains an empty instance of the collection unit type (for creating new children)
        /// </summary>
        public IMetaObject Template { get; set; }

        /// <summary>
        /// The type information for this Meta instance
        /// </summary>
        public IMetaType Type { get; set; }

        /// <summary>
        /// The index of the Value in the Meta dictionary, if the value of this object is cached there
        /// </summary>
        public int? v { get; set; }

        /// <summary>
        /// A string representation of the value if value type, or ToString if not
        /// </summary>
        public string Value { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// This constructor should only be user externally
        /// for creating a temporary instance
        /// </summary>
        public MetaObject()
        {
            Properties = new List<IMetaObject>();
            CollectionItems = new List<IMetaObject>();
        }

        /// <summary>
        /// This constructor should only be user externally
        /// for creating a temporary instance
        /// </summary>
        /// <param name="id"></param>
        public MetaObject(int id) : base(id) { }

        /// <summary>
        /// This constructor should only be user externally
        /// for creating a temporary instance. This builds
        /// a generic list with a property name for use in select editors
        /// </summary>
        /// <param name="PropertyName">The property name of the list</param>
        /// <param name="Values">The values contained within the list</param>
        /// <param name="c">The constructor to use when generating the list</param>
        public MetaObject(string PropertyName, IList<object> Values, MetaConstructor c) : base()
        {
            Contract.Requires(c != null);
            Contract.Requires(Values != null);

            this.Properties = new List<IMetaObject>();
            this.CollectionItems = new List<IMetaObject>();

            foreach (object o in Values)
            {
                if (o != null)
                {
                    this.AddItem(FromConstructor(c, new ObjectConstructor(null, null, o)));
                }
            }

            this.Type = MetaType.FromConstructor(c, Values.GetType());

            this.Property = new MetaProperty()
            {
                Name = PropertyName,
                Type = Type
            };
        }

        /// <summary>
        /// Creates a MetaObject with a constructor parameter, for serializing lists or using custom settings
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="c"></param>
        public MetaObject(object Value, MetaConstructor c) : this(c?.Clone(Value)) { }

        /// <summary>
        /// Creates a single use MetaObject using the default settings
        /// </summary>
        /// <param name="Value"></param>
        public MetaObject(object Value) : this(new MetaConstructor(Value)) { }

        /// <summary>
        /// Creates a single use MetaObject with a property name ONLY
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        public MetaObject(string PropertyName, object Value) : this(new MetaConstructor(Value) { Property = new Objects.MetaProperty() { Name = PropertyName } }) { }

        /// <summary>
        /// Creates a single use MetaObject with root level property information
        /// </summary>
        /// <param name="Property">The property Info to use during construction</param>
        /// <param name="Value">The object to serialize</param>
        public MetaObject(PropertyInfo Property, object Value) : this(new MetaConstructor(new ObjectConstructor(Property, null, Value))) { }

        /// <summary>
        /// Creates a new MetaObject using a constructor that should contain the object information to serialize
        /// </summary>
        /// <param name="c">The constructor to use</param>
        public MetaObject(MetaConstructor c) : base()
        {
            Contract.Requires(c != null);

            this.Constructor = c;

            c.ClaimOwnership(this);

            //Check for an existing MetaProperty, in case we need a top level Name
            this.Property = c.Property;
            if (c.PropertyInfo != null)
            {
                this.Property = Penguin.Reflection.Serialization.Objects.MetaProperty.FromConstructor(c, c.PropertyInfo);
            }

            this.Null = (c.Object is null);

            this.Type = Penguin.Reflection.Serialization.Objects.MetaType.FromConstructor(c, c.Type);

            CoreType thisCoreType = (c.Type).GetCoreType();

            if (c.Object is IMetaObject)
            {
                this.Properties = new List<IMetaObject>();

                this.AddProperty(c.Object as IMetaObject);
            }
            else if (thisCoreType == CoreType.Enum)
            {
                if (c.Object is null)
                {
                    this.v = c.Claim("0");
                }
                else
                {
                    RType thisType = c.Type;
                    this.v = c.Claim(Convert.ChangeType(c.Object, Enum.GetUnderlyingType(thisType)).ToString());
                }
            }
            else if (thisCoreType == CoreType.Value)
            {
                string Default = (c.Object ?? c.Type.GetDefaultValue())?.ToString();

                if (Default != null)
                {
                    this.v = c.Claim(Default);
                }
            }
            else //Reference
            {
                if (c.Object != null)
                {
                    this.v = c.Claim(c.Object.ToString());
                }

                this.Properties = new List<IMetaObject>();

                if (thisCoreType == CoreType.Collection)
                {
                    this.CollectionItems = new List<IMetaObject>();

                    this.Template = MetaObject.FromConstructor(c, new ObjectConstructor(c.PropertyInfo, c.Type.GetCollectionType(), null));

                    if (!(c.Object is null))
                    {
                        List<object> toGet = ((IEnumerable<object>)c.Object).ToList<object>();

                        foreach (object o in toGet)
                        {
                            if (o is null)
                            {
                                continue;
                            }

                            //We dont count IEnumerable containers as structures
                            //so the parent property of the child is the property
                            //that held the IEnumerable

                            //Ex       Bookshelf.Books => List<T>[0] => Book
                            //Becomes  Bookshelf.Books => Book
                            MetaObject i = FromConstructor(c, new ObjectConstructor(c.PropertyInfo, c.Type.GetCollectionType(), o));

                            i.Property = this.Property;

                            this.AddItem(i);
                        }
                    }
                }
                else if (thisCoreType == CoreType.Dictionary)
                {
                    this.CollectionItems = new List<IMetaObject>();

                    Type checkType = c.Object?.GetType() ?? c.PropertyInfo.PropertyType;

                    RType KVPType = typeof(KeyValuePair<,>).MakeGenericType(checkType.GetGenericArguments()[0], checkType.GetGenericArguments()[1]);

                    this.Template = FromConstructor(c, new ObjectConstructor(c.PropertyInfo, KVPType, null));

                    if (!(c.Object is null))
                    {
                        IEnumerable toGet = c.Object as IEnumerable;

                        foreach (object o in toGet)
                        {
                            if (o is null)
                            {
                                continue;
                            }

                            //We dont count IEnumerable containers as structures
                            //so the parent property of the child is the property
                            //that held the IEnumerable

                            //Ex       Bookshelf.Books => List<T>[0] => Book
                            //Becomes  Bookshelf.Books => Book
                            MetaObject i = FromConstructor(c, new ObjectConstructor(c.PropertyInfo, o.GetType(), o));

                            i.Property = this.Property;

                            this.AddItem(i);
                        }
                    }
                }
                else
                {
                    RType t = c.Object?.GetType();

                    foreach (System.Func<RType, RType> typeOverride in c.Settings.TypeGetterOverride)
                    {
                        if (t != null)
                        {
                            t = typeOverride.Invoke(t);
                        }
                    }

                    foreach (PropertyInfo thisProperty in c.GetProperties(t))
                    {
                        if (c.Settings.IgnoreInheritedProperties && !this.Type.Is(thisProperty.DeclaringType.FullName))
                        {
                            continue;
                        }

                        IMetaObject i;

                        if (!c.Validate(thisProperty))
                        {
                            continue;
                        }
                        if (c.Object is null)
                        {
                            i = FromConstructor(c, new ObjectConstructor(thisProperty, null, null));
                        }
                        else
                        {
                            try
                            {
                                object Object = c.GetValue(thisProperty);

                                if (Object is IMetaObject)
                                {
                                    i = Object as IMetaObject;
                                }
                                else
                                {
                                    i = FromConstructor(c, new ObjectConstructor(thisProperty, null, Object));
                                }
                            }
                            catch (Exception ex)
                            {
                                if (StaticLogger.IsListening)
                                {
                                    StaticLogger.Log(ex.Message, StaticLogger.LoggingLevel.Call);
                                    StaticLogger.Log(ex.StackTrace, StaticLogger.LoggingLevel.Call);
                                }

                                i = FromConstructor(c, new ObjectConstructor(thisProperty, null, null));
                                i.Exception = c.AddException($"{thisProperty.Name}: {ex.Message}");
                            }
                        }

                        this.AddProperty(i);
                    }
                }
            }

            if (c.IsOwner(this))
            {
                RegisterConstructor(c);
            }
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Returns an instance of a property by property name. Recursive notation supported using "." delimiter
        /// </summary>
        /// <param name="PropertyName">The property name to search for</param>
        /// <returns>The property if exists, or Error if not</returns>
        public IMetaObject this[string PropertyName]
        {
            get
            {
                Contract.Requires(PropertyName != null);

                IMetaObject m = this;

                foreach (string chunk in PropertyName.Split('.'))
                {
                    m = m.Properties.First(p => p.Property.Name == chunk);
                }

                return m;
            }
        }

        /// <summary>
        /// Returns an instance of a property by IMetaProperty
        /// </summary>
        /// <param name="metaProperty">The property to search for</param>
        /// <returns>The property if exists, or Error if not</returns>
        public IMetaObject this[IMetaProperty metaProperty]    // Indexer declaration
        {
            get
            {
                Contract.Requires(metaProperty != null);

                if (metaProperty.Type.IsNullable || metaProperty.Type.CoreType == CoreType.Reference)
                {
                    return this.Properties.FirstOrDefault(p => p.Property.Name == metaProperty.Name);
                }
                else
                {
                    return this.Properties.First(p => p.Property.Name == metaProperty.Name);
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Creates a new serialized object using the provided MetaConstructor
        /// </summary>
        /// <param name="c">The MetaConstructor to use</param>
        /// <returns>A newly serialized and DEHYDRATED object</returns>
        public static MetaObject FromConstructor(MetaConstructor c)
        {
            Contract.Requires(c != null);

            return FromConstructor(c, new ObjectConstructor(c.PropertyInfo, c.Type, c.Object));
        }

        /// <summary>
        /// Creates a new serialized object using the provided Constructor, and object
        /// </summary>
        /// <param name="c">The constructor to use for serialization</param>
        /// <param name="o">The object to serialize</param>
        /// <returns>A newly serialized and DEHYDRATED object</returns>
        public static MetaObject FromConstructor(MetaConstructor c, object o) => FromConstructor(c, new ObjectConstructor(null, null, o));

        /// <summary>
        /// Creates a new serialized object using the provided Constructor, and Object Constructor
        /// </summary>
        /// <param name="c">The constructor to use for serializing</param>
        /// <param name="oc">An ObjectConstructor containing the relevant serialization context information</param>
        /// <returns>A newly serialized and DEHYDRATED object</returns>
        public static MetaObject FromConstructor(MetaConstructor c, ObjectConstructor oc)
        {
            Contract.Requires(c != null);

            MetaObject i;

            KeyGroup Wrapper = new KeyGroup(oc);

            if (!c.Contains(Wrapper))
            {
                AbstractMeta placeHolder = c.Claim(Wrapper);
                i = new MetaObject(c.Clone(oc))
                {
                    i = placeHolder.i
                };
                c.UpdateClaim(i, Wrapper);
            }
            else
            {
                i = new MetaObject(c.GetId(Wrapper));
            }

            return i;
        }

        /// <summary>
        /// Adds an item to the underlying collection and sets this object as its parent
        /// </summary>
        /// <param name="instance">The item to add to the collection</param>
        public void AddItem(IMetaObject instance)
        {
            if (instance is null)
            {
                return;
            }

            if (instance.GetParent() != null)
            {
                instance.GetParent().RemoveItem(instance);
            }

            instance.SetParent(this);
            this.CollectionItems.Add(instance);
        }

        /// <summary>
        /// Adds a Property Value to this object
        /// </summary>
        /// <param name="instance">The Property Object to add</param>
        public void AddProperty(IMetaObject instance)
        {
            if (instance is null)
            {
                return;
            }

            if (instance.GetParent() != null)
            {
                instance.GetParent().RemoveProperty(instance);
            }

            instance.SetParent(this);
            this.Properties.Add(instance);
        }

        /// <summary>
        /// Gets the CoreType of this instance from the set type
        /// </summary>
        /// <returns>The CoreType of this instance</returns>
        public CoreType GetCoreType() => this.Type?.CoreType ?? CoreType.Null;

        /// <summary>
        /// Returns the Parent of this object (property holder) or null if empty.
        /// UNRELIABLE for complex structures if SetParent is not called during recursion
        /// since Dehydration can cause objects to dereference parents.
        /// </summary>
        /// <returns>The parent of the object or null if no parent</returns>
        public IMetaObject GetParent() => this.Parent;

        /// <summary>
        /// Checks to see if this objects declared type contains a property
        /// </summary>
        /// <param name="PropertyName">The property name to check for</param>
        /// <returns>Whether or not the objects declared type contains a property</returns>
        public bool HasProperty(string PropertyName)    // Indexer declaration
            => this.Type.Properties.Any(p => p.Name == PropertyName);

        /// <summary>
        /// Hydrates this object instance. Should be called once the serialized object is ready for use
        /// </summary>
        /// <param name="meta">An optional MetaData dictionary to use as the cache for hydration. Not needed if this is a top level instance, as it is provided by the internal constructor</param>
        public override void Hydrate(IDictionary<int, IAbstractMeta> meta = null)
        {
            //If we never updated the root meta because it was called outside of the
            //recursive function
            if (this.IsRoot && this.Meta.ContainsKey(0) && this.Meta[0].GetType() == typeof(AbstractMeta))
            {
                this.Meta[0] = this;
            }

            meta = meta ?? this.Meta ?? this.Constructor.Meta.Select(v => v.Value).ToDictionary(k => k.i, v => v);
            ;

            this.Property = this.HydrateChild(this.Property, meta);
            this.Type = this.HydrateChild(this.Type, meta);
            this.Template = this.HydrateChild(this.Template, meta);

            if (this.GetCoreType() != CoreType.Value || (this.Properties != null && this.Properties.Any()))
            {
                this.HydrateList(this.Properties, meta);
            }

            if (this.GetCoreType() == CoreType.Collection || this.GetCoreType() == CoreType.Dictionary || (this.CollectionItems != null && this.CollectionItems.Any()))
            {
                this.HydrateList(this.CollectionItems, meta);
            }

            if (this.v.HasValue)
            {
                this.Value = (meta[this.v.Value] as StringHolder).v;
            }

            this.IsHydrated = true;
        }

        /// <summary>
        /// Should check if the reference is recursive but I dont think we can trust this
        /// </summary>
        /// <returns></returns>
        public bool IsRecursive()
        {
            IMetaObject parent = this.Parent;

            while (parent != null)
            {
                if (this == parent)
                {
                    return true;
                }

                parent = parent.GetParent();
            }

            return false;
        }

        /// <summary>
        /// If using an external constructor from another serialization, this method should be called on top level objects to
        /// tell it that this constructor is the source for the cache
        /// </summary>
        /// <param name="c">The constructor to register</param>
        public void RegisterConstructor(MetaConstructor c)
        {
            Contract.Requires(c != null);

            this.Meta = c.Meta.Select(v => v.Value).ToDictionary(k => k.i, v => v);
            this.BuildExceptions = c.Exceptions.ToDictionary(k => k.Value, v => v.Key);
            this.IsRoot = true;
        }

        /// <summary>
        /// Removes an item instance from the underlying collection. Does not dereference parent
        /// </summary>
        /// <param name="instance">The object instance to remove from the collection</param>
        public void RemoveItem(IMetaObject instance)
        {
            Contract.Requires(instance != null);

            instance.SetParent(null);

            this.CollectionItems.Remove(instance);
        }

        /// <summary>
        /// Removes a property from this object. Does not dereference parent
        /// </summary>
        /// <param name="instance">The instance of the property value to remove</param>
        public void RemoveProperty(IMetaObject instance)
        {
            Contract.Requires(instance != null);

            instance.SetParent(null);

            this.Properties.Remove(instance);
        }

        /// <summary>
        /// Call this while recursing through the object structure to ensure that the parent on each object is set correctly
        /// </summary>
        /// <param name="parent">The parent of this object</param>
        public void SetParent(IMetaObject parent) => this.Parent = parent;

        /// <summary>
        /// Returns the Property.Name ?? Type.Name ?? Empty in that order
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{this.Property?.Name ?? this.Type?.Name ?? string.Empty}";

        /// <summary>
        /// The Property.Name ?? Type.Name ?? Empty in that order
        /// </summary>
        /// <returns></returns>
        public IMetaType TypeOf() => this.Type;

        #endregion Methods

        private MetaConstructor Constructor { get; set; }

        private IMetaObject Parent { get; set; }
    }
}