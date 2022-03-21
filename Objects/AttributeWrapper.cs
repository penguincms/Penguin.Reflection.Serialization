using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Constructors;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Objects
{
    internal class AttributeWrapper : MetaWrapper
    {
        #region Properties

        public AttributeInstance Attribute { get; set; }

        public string Key { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public RType Type { get; set; }

        #endregion Properties

        #region Constructors

        public AttributeWrapper(AttributeInstance a, PropertyInfo p, MetaConstructor c)
        {
            this.PropertyInfo = p;
            this.Attribute = a;
            this.Type = p.DeclaringType;

            if (!c.Cache.Attributes.ContainsKey(a.Instance))
            {
                c.Cache.Attributes.Add(a.Instance, c.Cache.Attributes.Count);
            }

            this.Key = $"@{c.Cache.Attributes[a.Instance]}|{a.IsInherited}";
        }

        public AttributeWrapper(AttributeInstance a, RType t, MetaConstructor c)
        {
            this.Attribute = a;
            this.Type = t;

            if (!c.Cache.Attributes.ContainsKey(a.Instance))
            {
                c.Cache.Attributes.Add(a.Instance, c.Cache.Attributes.Count);
            }

            this.Key = $"@{c.Cache.Attributes[a.Instance]}|{a.IsInherited}";
        }

        #endregion Constructors

        #region Methods

        public override string GetKey() => this.Key;

        #endregion Methods

        //
    }
}