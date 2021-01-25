using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Constructors;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Constructors;
using System.Collections.Generic;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// Meta representation of an attribute, including the retrieved instance with properties
    /// </summary>
    public class MetaAttribute : AbstractMeta, IMetaAttribute
    {
        #region Properties

        IMetaObject IMetaAttribute.Instance => this.Instance;

        /// <summary>
        /// An instance representing the retrieved attribute
        /// </summary>
        public MetaObject Instance { get; set; }

        /// <summary>
        /// True if the attribute is declared on a parent type
        /// </summary>
        public bool IsInherited { get; set; }

        IMetaType IMetaAttribute.Type => this.Type;

        /// <summary>
        /// The Type of the attribute
        /// </summary>
        public MetaType Type { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// For serialization ONLY. Do not use
        /// </summary>
        public MetaAttribute() : base(-1)
        {
        }

        /// <summary>
        /// Create an instance of the MetaAttribute referencing a specific index in the MetaConstructor
        /// </summary>
        /// <param name="id">The ID of the index of the Meta in the MetaConstructor</param>
        public MetaAttribute(int id) : base(id)
        {
        }

        /// <summary>
        /// Create an instance using information found in the MetaConstructor
        /// </summary>
        /// <param name="c">The MetaConstructor to  use</param>
        public MetaAttribute(MetaConstructor c) : base()
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            this.Instance = MetaObject.FromConstructor(c, new ObjectConstructor(c.PropertyInfo, c.Type, (c.Object as AttributeInstance).Instance));

            this.Type = MetaType.FromConstructor(c, (c.Object as AttributeInstance).Instance);

            this.IsInherited = (c.Object as AttributeInstance).IsInherited;
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Retrieve a property from the MetaObject instance of this attribute based  on property name
        /// </summary>
        /// <param name="PropertyName">The name of the property to search for</param>
        /// <returns>The property, if it exists</returns>
        public IMetaObject this[string PropertyName] => this.Instance[PropertyName];

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Hydrates this instance of the MetaObject
        /// </summary>
        /// <param name="meta">The dictionary of MetaData generated during construction</param>
        public override void Hydrate(IDictionary<int, IHydratable> meta = null)
        {
            this.Type = this.HydrateChild(this.Type, meta);
            this.Instance = this.HydrateChild(this.Instance, meta);
        }

        /// <summary>
        /// Returns the Type Name
        /// </summary>
        /// <returns>The Type Name</returns>
        public override string ToString()
        {
            return this.Type.Name;
        }

        internal static MetaAttribute FromConstructor(MetaConstructor c, AttributeInstance o, PropertyInfo p)
        {
            return FromConstructor(c, new AttributeWrapper(o, p, c));
        }

        internal static MetaAttribute FromConstructor(MetaConstructor c, AttributeInstance o, RType t)
        {
            return FromConstructor(c, new AttributeWrapper(o, t, c));
        }

        internal static MetaAttribute FromConstructor(MetaConstructor c, AttributeWrapper wrapper)
        {
            MetaAttribute a;
            if (!c.Contains(wrapper))
            {
                AbstractMeta placeHolder = c.Claim(wrapper);

                //Drop the type the attribute is declared on so we dont think we're trying to override the attribute.GetType()
                a = new MetaAttribute(c.Clone(new ObjectConstructor(wrapper.PropertyInfo, wrapper.Attribute.Instance.GetType(), wrapper.Attribute)))
                {
                    I = placeHolder.I
                };
                c.UpdateClaim(a, wrapper);
            }
            else
            {
                a = new MetaAttribute(c.GetId(wrapper));
            }

            return a;
        }

        #endregion Methods
    }
}