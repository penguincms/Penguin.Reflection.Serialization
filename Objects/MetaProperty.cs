using Loxifi;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Constructors;
using System.Collections.Generic;
using System.Reflection;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// A Meta Class representing the property info for this object in the tree. Can be unreliable for complex structures at the moment due to the way objects are cached
    /// </summary>
    public class MetaProperty : AbstractMeta, IMetaProperty
    {
        #region Properties

        IEnumerable<IMetaAttribute> IHasAttributes.Attributes => Attributes;

        /// <summary>
        /// A list of attributes declared on this property
        /// </summary>
        public IEnumerable<MetaAttribute> Attributes { get; set; }

        IMetaType IMetaProperty.DeclaringType => Type;

        /// <summary>
        /// The DeclaringType of this property
        /// </summary>
        public MetaType DeclaringType { get; set; }

        /// <summary>
        /// The name of this property
        /// </summary>
        public string Name { get; set; }

        IMetaType IMetaProperty.Type => Type;

        /// <summary>
        /// The Type of this property
        /// </summary>
        public MetaType Type { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a specific instance of this object referencing a cached index in a list of metadata, or -1 for temporary
        /// </summary>
        /// <param name="id">The index of the MetaData in the cache</param>
        public MetaProperty(int id) : base(id)
        {
        }

        /// <summary>
        /// Creates a new empty instance of this object for temporary use
        /// </summary>
        public MetaProperty()
        {
            Attributes = new List<MetaAttribute>();
        }

        /// <summary>
        /// Creates an instance of this object from an existing MetaProperty using Name and Type
        /// </summary>
        /// <param name="p">The existing MetaProperty</param>
        public MetaProperty(MetaProperty p) : this()
        {
            if (p is null)
            {
                throw new System.ArgumentNullException(nameof(p));
            }

            Name = p.Name;
            Type = p.Type;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Hydrates this property using a list of MetaInformation provided by the constructor
        /// </summary>
        /// <param name="meta"></param>
        public override void Hydrate(IDictionary<int, IHydratable> meta = null)
        {
            //This should be done through an accessor because right now we're relying on
            //The fact that the constructor sets it to a list, which is not the correct way
            //to do this.
            HydrateList(Attributes as IList<MetaAttribute>, meta);

            Type = HydrateChild(Type, meta);
            DeclaringType = HydrateChild(DeclaringType, meta);
        }

        /// <summary>
        /// Returns the fully qualified name of this property
        /// </summary>
        /// <returns>The fully qualified name of this property</returns>
        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }

        /// <summary>
        /// Returns the type of the underlying property
        /// </summary>
        /// <returns>The fully qualified name of this property</returns>
        public IMetaType TypeOf()
        {
            return Type;
        }

        #endregion Methods

        internal MetaProperty(MetaConstructor c) : base()
        {
            Name = c.PropertyInfo.Name;
            Type = MetaType.FromConstructor(c, c.PropertyInfo.PropertyType);
            DeclaringType = MetaType.FromConstructor(c, c.PropertyInfo.DeclaringType);

            List<MetaAttribute> attributes = new();
            Attributes = attributes;

            if (c.Settings.AttributeIncludeSettings != AttributeIncludeSetting.None)
            {
                foreach (AttributeInstance a in TypeFactory.GetCustomAttributes(c.PropertyInfo))
                {
                    if (c.Settings.ShouldAddAttribute(a.Instance.GetType()))
                    {
                        attributes.Add(MetaAttribute.FromConstructor(c, a, c.PropertyInfo));
                    }
                }
            }
        }
        private TypeFactory TypeFactory { get; set; } = new TypeFactory(new TypeFactorySettings());
        internal static MetaProperty FromConstructor(MetaConstructor c, PropertyInfo propertyInfo)
        {
            MetaProperty p;

            if (!c.Contains(propertyInfo))
            {
                AbstractMeta placeHolder = c.Claim(propertyInfo);
                p = new MetaProperty(c.Clone(propertyInfo))
                {
                    I = placeHolder.I
                };
                c.UpdateClaim(p, propertyInfo);
            }
            else
            {
                p = new MetaProperty(c.GetId(propertyInfo));
            }

            return p;
        }
    }
}