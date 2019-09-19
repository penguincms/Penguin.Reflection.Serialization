using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Constructors;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// A Meta Class representing the property info for this object in the tree. Can be unreliable for complex structures at the moment due to the way objects are cached
    /// </summary>
    public class MetaProperty : AbstractMeta, IMetaProperty
    {
        #region Properties

        /// <summary>
        /// A list of attributes declared on this property
        /// </summary>
        public IEnumerable<IMetaAttribute> Attributes { get; set; }

        /// <summary>
        /// The DeclaringType of this property
        /// </summary>
        public IMetaType DeclaringType { get; set; }

        /// <summary>
        /// The name of this property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Type of this property
        /// </summary>
        public IMetaType Type { get; set; }

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
            Attributes = new List<IMetaAttribute>();
        }

        /// <summary>
        /// Creates an instance of this object from an existing MetaProperty using Name and Type
        /// </summary>
        /// <param name="p">Thhe existing MetaProperty</param>
        public MetaProperty(IMetaProperty p) : this()
        {
            Contract.Requires(p != null);

            this.Name = p.Name;
            this.Type = p.Type;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Hydrates this property using a list of MetaInformation provided by the constructor
        /// </summary>
        /// <param name="meta"></param>
        public override void Hydrate(IDictionary<int, IAbstractMeta> meta = null)
        {
            //This should be done through an accessor because right now we're relying on
            //The fact that the constructor sets it to a list, which is not the correct way
            //to do this.
            this.HydrateList(this.Attributes as IList<IMetaAttribute>, meta);

            this.Type = this.HydrateChild(this.Type, meta);
            this.DeclaringType = this.HydrateChild(this.DeclaringType, meta);
        }

        /// <summary>
        /// Returns the fully qualified name of this property
        /// </summary>
        /// <returns>The fully qualified name of this property</returns>
        public override string ToString() => $"{this.Type.Name} {this.Name}";

        /// <summary>
        /// Returns the type of the underlying property
        /// </summary>
        /// <returns>The fully qualified name of this property</returns>
        public IMetaType TypeOf() => this.Type;

        #endregion Methods

        internal MetaProperty(MetaConstructor c) : base()
        {
            this.Name = c.PropertyInfo.Name;
            this.Type = MetaType.FromConstructor(c, c.PropertyInfo.PropertyType);
            this.DeclaringType = MetaType.FromConstructor(c, c.PropertyInfo.DeclaringType);

            List<IMetaAttribute> attributes = new List<IMetaAttribute>();
            this.Attributes = attributes;

            if (c.Settings.AttributeIncludeSettings != AttributeIncludeSetting.None)
            {
                foreach (AttributeInstance a in TypeCache.GetCustomAttributes(c.PropertyInfo))
                {
                    if (c.Settings.ShouldAddAttribute(a.Instance.GetType()))
                    {
                        attributes.Add(MetaAttribute.FromConstructor(c, a, c.PropertyInfo));
                    }
                }
            }
        }

        internal static MetaProperty FromConstructor(MetaConstructor c, PropertyInfo propertyInfo)
        {
            MetaProperty p;

            if (!c.Contains(propertyInfo))
            {
                AbstractMeta placeHolder = c.Claim(propertyInfo);
                p = new MetaProperty(c.Clone(propertyInfo))
                {
                    i = placeHolder.i
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