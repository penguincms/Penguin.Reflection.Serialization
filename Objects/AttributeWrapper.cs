using Loxifi;
using Loxifi.Interfaces;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Constructors;
using System;
using System.Reflection;
using RType = System.Type;

namespace Penguin.Reflection.Serialization.Objects
{
    internal class AttributeWrapper : MetaWrapper
    {
        #region Properties

        public IAttributeInstance<Attribute> Attribute { get; set; }

        public string Key { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public RType Type { get; set; }

        #endregion Properties

        #region Constructors

        public AttributeWrapper(IAttributeInstance<Attribute> a, PropertyInfo p, MetaConstructor c)
        {
            PropertyInfo = p;
            Attribute = a;
            Type = p.DeclaringType;

            if (!c.Cache.Attributes.ContainsKey(a.Instance))
            {
                c.Cache.Attributes.Add(a.Instance, c.Cache.Attributes.Count);
            }

            Key = $"@{c.Cache.Attributes[a.Instance]}|{a.IsInherited}";
        }

        public AttributeWrapper(IAttributeInstance<Attribute> a, RType t, MetaConstructor c)
        {
            Attribute = a;
            Type = t;

            if (!c.Cache.Attributes.ContainsKey(a.Instance))
            {
                c.Cache.Attributes.Add(a.Instance, c.Cache.Attributes.Count);
            }

            Key = $"@{c.Cache.Attributes[a.Instance]}|{a.IsInherited}";
        }

        #endregion Constructors

        #region Methods

        public override string GetKey()
        {
            return Key;
        }

        #endregion Methods

        //
    }
}