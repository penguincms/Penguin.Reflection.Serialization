using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Extensions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Objects;
using Penguin.Reflection.Serialization.Constructors;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Enum = System.Enum;
using Nullable = System.Nullable;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// A MetaData based class holding Type information
    /// </summary>
    public class MetaType : AbstractMeta, IMetaType
    {
        #region Properties

        /// <summary>
        /// The assembly qualified name
        /// </summary>
        public string AssemblyQualifiedName { get; set; }

        IEnumerable<IMetaAttribute> IHasAttributes.Attributes => this.Attributes;

        /// <summary>
        /// The attributes declared on the underlying type, along with instances
        /// </summary>
        public List<MetaAttribute> Attributes { get; set; }

        /// <summary>
        /// The base type for the underlying type
        /// </summary>
        public IMetaType BaseType { get; set; }

        /// <summary>
        /// If this type is a collection, this contains the unit type
        /// </summary>
        public IMetaType CollectionType { get; set; }

        /// <summary>
        /// The CoreType for the underlying object
        /// </summary>
        public CoreType CoreType { get; set; }

        /// <summary>
        /// String representation of default value for this type
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// The FullName for this type
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// True if type is an array
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// True if type is an enumeration
        /// </summary>
        public bool IsEnum => this.CoreType == CoreType.Enum;

        /// <summary>
        /// True if type is a Nullable?
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// True if type contains any numeric type
        /// </summary>
        public bool IsNumeric { get; set; }

        /// <summary>
        /// The Name for the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Namespace the type is found in
        /// </summary>
        public string Namespace { get; set; }

        IReadOnlyList<IMetaType> IMetaType.Parameters => this.Parameters;

        /// <summary>
        /// Generic parameters used for constructing the type
        /// </summary>
        public List<MetaType> Parameters { get; set; }

        IReadOnlyList<IMetaProperty> IHasProperties.Properties => this.Properties;

        /// <summary>
        /// A list of all the properties found on the type
        /// </summary>
        public List<MetaProperty> Properties { get; set; }

        /// <summary>
        /// ToString called on the Type
        /// </summary>
        public string StringValue { get; set; }

        IReadOnlyList<IEnumValue> IMetaType.Values => this.Values;

        /// <summary>
        /// If the type is an enum, this contains all of the possible values
        /// </summary>
        public List<IEnumValue> Values { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the type using the specified index ID
        /// </summary>
        /// <param name="id">The ID of the location in the Metadata cache that the information can be found</param>
        public MetaType(int id) : base(id)
        {
        }

        /// <summary>
        /// Creates a new instance of this type for temporary use
        /// </summary>
        public MetaType()
        {
        }

        /// <summary>
        /// Base type generation or shortcut for temporary instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="properties">Manually set the properties list if type is defining a temporary instance of and object</param>
        public MetaType(Type type, IEnumerable<MetaObject> properties = null) : base()
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);

                Contract.Assert(type != null);

                this.IsNullable = true;
            }

            this.Name = type.Name;
            this.FullName = type.FullName;
            this.Namespace = type.Namespace;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.StringValue = type.ToString();
            this.CoreType = type.GetCoreType();
            this.IsArray = type.IsArray;

            this.IsNumeric = type.IsNumericType();
            this.Default = type.GetDefaultValue()?.ToString();
            this.Values = GetEnumValues(type);

            if (properties != null)
            {
                this.Properties = properties.Select(p => p.Property).ToList();
            }
        }

        /// <summary>
        /// Only for use as a placeholder type for manually created rendering models
        /// </summary>
        /// <param name="s"></param>
        /// <param name="properties">Properties to set for the type</param>
        public MetaType(string s, IEnumerable<MetaObject> properties)
        {
            this.Name = this.AssemblyQualifiedName = s;
            this.Namespace = "Dynamic";

            this.Properties = properties.Select(p => p.Property).ToList();
        }

        public static List<IEnumValue> GetEnumValues(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                List<IEnumValue> Values = new List<IEnumValue>();

                foreach (string Name in Enum.GetNames(type))
                {
                    Values.Add(
                        new EnumValue()
                        {
                            Value = Convert.ChangeType(
                                Enum.Parse(type, Name),
                                Enum.GetUnderlyingType(type)
                            ).ToString(),
                            Label = Name
                        });
                }

                return Values;
            }
            else
            {
                return null;
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new MetaType from a given object
        /// </summary>
        /// <param name="c">The constructor to use</param>
        /// <param name="o">The object to base the MetaType on</param>
        /// <returns>A new instance of MetaType</returns>
        public static MetaType FromConstructor(MetaConstructor c, object o)
        {
            if (c is null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return FromConstructor(c, o.GetType());
        }

        /// <summary>
        /// Tests for inequality between two MetaTypes using AssemblyQualifiedName
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The other MetaType to test</param>
        /// <returns></returns>
        public static bool operator !=(MetaType obj1, IMetaType obj2) => !(obj1 == obj2);

        /// <summary>
        /// Tests for inequality between a MetaType an a System Type using the string value
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The System Type</param>
        /// <returns></returns>
        public static bool operator !=(System.Type obj1, MetaType obj2) => !(obj1 == obj2);

        /// <summary>
        /// Tests for inequality between a MetaType an a System Type using the string value
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The System Type</param>
        /// <returns></returns>
        public static bool operator !=(MetaType obj1, System.Type obj2) => !(obj1 == obj2);

        /// <summary>
        /// Tests for equality between two MetaTypes using AssemblyQualifiedName
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The other MetaType to test</param>
        /// <returns></returns>
        public static bool operator ==(MetaType obj1, IMetaType obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            return (obj1.AssemblyQualifiedName == obj2.AssemblyQualifiedName);
        }

        /// <summary>
        /// Tests for equality between a MetaType an a System Type using the string value
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The System Type</param>
        /// <returns></returns>
        public static bool operator ==(System.Type obj1, MetaType obj2)
        {
            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            return (obj1.ToString() == obj2.ToString());
        }

        /// <summary>
        /// Tests for equality between a MetaType an a System Type using the string value
        /// </summary>
        /// <param name="obj1">This object</param>
        /// <param name="obj2">The System Type</param>
        /// <returns></returns>
        public static bool operator ==(MetaType obj1, System.Type obj2)
        {
            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            return (obj1.ToString() == obj2.ToString());
        }

        /// <summary>
        /// Compares two metatypes based on AssemblyQualifiedName
        /// </summary>
        /// <param name="other">The MetaType to compare against</param>
        /// <returns>Whether or not they share an Assembly Qualified Name</returns>
        public bool Equals(IMetaType other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (this.AssemblyQualifiedName == other.AssemblyQualifiedName);
        }

        /// <summary>
        /// Equality check based on Type. Compares against any IMetaType
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>Whether or not the two objects have the same IMetaType</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((MetaType)obj);
        }

        /// <summary>
        /// The hashcode of the object is just the hashcode of the AssemblyQualifiedName
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return this.AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Hydrates this instance using a provided list of Meta information generated by a MetaConstructor
        /// </summary>
        /// <param name="meta"></param>
        public override void Hydrate(IDictionary<int, IHydratable> meta = null)
        {
            //This should be done through an accessor because right now we're relying on
            //The fact that the constructor sets it to a list, which is not the correct way
            //to do this.
            this.HydrateList(this.Attributes, meta);

            this.HydrateList(this.Parameters, meta);

            this.HydrateList(this.Properties, meta);

            if (this.BaseType is MetaType bt)
            {
                this.BaseType = HydrateChild(bt, meta);
            }

            if (this.CollectionType is MetaType ct)
            {
                this.CollectionType = HydrateChild(ct, meta);
            }
        }

        /// <summary>
        /// Returns the Name of the type this MetaObject represents
        /// </summary>
        /// <returns>The Name of the type this MetaObject represents</returns>
        public override string ToString() => this.StringValue;

        /// <summary>
        /// Returns this, since its a Type
        /// </summary>
        /// <returns>this</returns>
        public IMetaType TypeOf() => this;

        #endregion Methods

        internal MetaType(MetaConstructor c) : this(c.Type)
        {
            Type type = c.Type;

            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
                this.IsNullable = true;
            }

            this.Parameters = new List<MetaType>();

            List<MetaAttribute> attributes = new List<MetaAttribute>();
            this.Attributes = attributes;

            if (type.BaseType != null)
            {
                this.BaseType = MetaType.FromConstructor(c, type.BaseType);
            }

            if (this.CoreType == CoreType.Collection)
            {
                this.CollectionType = MetaType.FromConstructor(c, type.GetCollectionType());
            }

            foreach (Type g in type.GetGenericArguments())
            {
                this.Parameters.Add(MetaType.FromConstructor(c, g));
            }

            if (c.Settings.AttributeIncludeSettings != AttributeIncludeSetting.None)
            {
                foreach (AttributeInstance a in TypeCache.GetCustomAttributes(type))
                {
                    if (c.Settings.ShouldAddAttribute(a.Instance.GetType()))
                    {
                        attributes.Add(MetaAttribute.FromConstructor(c, a, type));
                    }
                }
            }

            this.Properties = new List<MetaProperty>();

            if (this.CoreType != CoreType.Value)
            {
                foreach (PropertyInfo thisProperty in c.GetProperties())
                {
                    if (c.Validate(thisProperty))
                    {
                        this.Properties.Add(MetaProperty.FromConstructor(c, thisProperty));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new MetaType from a given type
        /// </summary>
        /// <param name="c">The constructor to use</param>
        /// <param name="t">The type to base the MetaType on</param>
        /// <returns>A new instance of MetaType</returns>
        public static MetaType FromConstructor(MetaConstructor c, Type t) // Leave this public. Its used by the Reporting Type Serializer
        {
            if (c is null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            if (t is null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            string Name = t.Name;

            foreach (System.Func<Type, Type> typeOverride in c.Settings.TypeGetterOverride)
            {
                t = typeOverride.Invoke(t);
            }

            MetaType i;

            if (!c.Contains(t))
            {
                AbstractMeta placeHolder = c.Claim(t);
                i = new MetaType(c.Clone(t))
                {
                    I = placeHolder.I
                };
                c.UpdateClaim(i, t);

                i = new MetaType(c.GetId(t));
            }
            else
            {
                i = new MetaType(c.GetId(t));
            }

            return i;
        }
    }
}