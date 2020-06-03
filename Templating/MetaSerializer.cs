using Penguin.Reflection.Extensions;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;


namespace Penguin.Reflection.Serialization.Templating
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static partial class MetaSerializer
    {

        private static string _assemblyName { get; set; }

        public static string AssemblyName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_assemblyName))
                {
                    _assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }

                return _assemblyName;
            }
        }

        public static void Serialize(this IList<Enum> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("[");

            int ElementIndex = 0;
            foreach (Enum thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }
                sb.Append(System.Convert.ChangeType(toSerialize, Enum.GetUnderlyingType(toSerialize.GetType())).ToString());
            }

            sb.Append("]");

        }

        public static void SerializeDictionary(this IDictionary toSerialize, StringBuilder sb)
        {

            Contract.Requires(toSerialize != null);

            sb.Append("{");

            int ElementIndex = 0;

            foreach (DictionaryEntry thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }
                if (thisChild.Value.GetType() == typeof(string))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":\"{thisChild.Value.ToJSONValue()}\"");
                    continue;
                }



                if (typeof(MetaType).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as MetaType, sb);
                    continue;
                }


                if (typeof(MetaProperty).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as MetaProperty, sb);
                    continue;
                }


                if (typeof(MetaObject).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as MetaObject, sb);
                    continue;
                }


                if (typeof(MetaAttribute).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as MetaAttribute, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable, sb);
                    continue;
                }


                throw new Exception("Template does not support Dictionary Value type");
            }

            sb.Append("}");

        }

        public static void SerializeList(this IList toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);
            sb.Append("[");

            int ElementIndex = 0;
            foreach (object thisChild in toSerialize)
            {
                if (thisChild is null) { continue; }

                if (typeof(MetaType).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as MetaType, sb);
                    continue;
                }


                if (typeof(MetaProperty).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as MetaProperty, sb);
                    continue;
                }


                if (typeof(MetaObject).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as MetaObject, sb);
                    continue;
                }


                if (typeof(MetaAttribute).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as MetaAttribute, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue, sb);
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        sb.Append(",");
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable, sb);
                    continue;
                }


            }

            sb.Append("]");

        }



        public static void Serialize(this IList<MetaType> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (MetaType thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this MetaType toSerialize, StringBuilder sb)
        {
            if (toSerialize is null)
            {
                throw new ArgumentNullException(nameof(toSerialize));
            }

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.AssemblyQualifiedName != null) { sb.Append($",\"AssemblyQualifiedName\":\"{toSerialize.AssemblyQualifiedName.ToJSONValue()}\""); }
            if (toSerialize.BaseType != null)
            {
                sb.Append(",\"BaseType\":");
                Serialize(toSerialize.BaseType, sb);
            }
            if (toSerialize.CollectionType != null)
            {
                sb.Append(",\"CollectionType\":");
                Serialize(toSerialize.CollectionType, sb);
            }
            if (toSerialize.CoreType != 0) { sb.Append($",\"CoreType\":{System.Convert.ChangeType(toSerialize.CoreType, Enum.GetUnderlyingType(toSerialize.CoreType.GetType()))}"); }
            if (toSerialize.Default != null) { sb.Append($",\"Default\":\"{toSerialize.Default.ToJSONValue()}\""); }
            if (toSerialize.FullName != null) { sb.Append($",\"FullName\":\"{toSerialize.FullName.ToJSONValue()}\""); }
            if (toSerialize.IsArray) { sb.Append(",\"IsArray\":true"); }
            if (toSerialize.IsEnum) { sb.Append(",\"IsEnum\":true"); }
            if (toSerialize.IsNullable) { sb.Append(",\"IsNullable\":true"); }
            if (toSerialize.IsNumeric) { sb.Append(",\"IsNumeric\":true"); }
            if (toSerialize.Name != null) { sb.Append($",\"Name\":\"{toSerialize.Name.ToJSONValue()}\""); }
            if (toSerialize.Namespace != null) { sb.Append($",\"Namespace\":\"{toSerialize.Namespace.ToJSONValue()}\""); }
            if (toSerialize.Parameters != null && toSerialize.Parameters.Any())
            {
                sb.Append(",\"Parameters\":");
                SerializeList(toSerialize.Parameters as IList, sb);
            }
            if (toSerialize.Values != null && toSerialize.Values.Any())
            {
                sb.Append(",\"Values\":");
                SerializeList(toSerialize.Values as IList, sb);
            }
            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            if (toSerialize.Attributes != null && toSerialize.Attributes.Any())
            {
                sb.Append(",\"Attributes\":");
                SerializeList(toSerialize.Attributes.ToList(), sb);
            }
            if (toSerialize.Properties != null && toSerialize.Properties.Any())
            {
                sb.Append(",\"Properties\":");
                SerializeList(toSerialize.Properties as IList, sb);
            }
            sb.Append("}");
        }


        public static void Serialize(this IList<MetaProperty> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (MetaProperty thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this MetaProperty toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (!(toSerialize.DeclaringType is null))
            {
                sb.Append(",\"DeclaringType\":");
                Serialize(toSerialize.DeclaringType, sb);
            }
            if (!(toSerialize.Name is null)) { sb.Append($",\"Name\":\"{toSerialize.Name.ToJSONValue()}\""); }
            if (!(toSerialize.Type is null))
            {
                sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }
            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            if (toSerialize.Attributes != null && toSerialize.Attributes.Any())
            {
                sb.Append(",\"Attributes\":");
                SerializeList(toSerialize.Attributes.ToList(), sb);
            }
            sb.Append("}");
        }


        public static void Serialize(this IList<MetaObject> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (MetaObject thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this MetaObject toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.BuildExceptions != null && toSerialize.BuildExceptions.Keys.Count > 0)
            {
                sb.Append(",\"BuildExceptions\":");
                SerializeDictionary(toSerialize.BuildExceptions as IDictionary, sb);
            }
            if (toSerialize.CollectionItems != null && toSerialize.CollectionItems.Any())
            {
                sb.Append(",\"CollectionItems\":");
                SerializeList(toSerialize.CollectionItems as IList, sb);
            }
            if (toSerialize.Exception != 0) { sb.Append($",\"Exception\":{toSerialize.Exception}"); }
            if (toSerialize.Meta != null && toSerialize.Meta.Keys.Count > 0)
            {
                sb.Append(",\"Meta\":");
                SerializeDictionary(toSerialize.Meta as IDictionary, sb);
            }
            if (toSerialize.Null) { sb.Append(",\"Null\":true"); }
            if (toSerialize.Properties != null && toSerialize.Properties.Any())
            {
                sb.Append(",\"Properties\":");
                SerializeList(toSerialize.Properties as IList, sb);
            }
            if (toSerialize.Property != null)
            {
                sb.Append(",\"Property\":");
                Serialize(toSerialize.Property, sb);
            }
            if (toSerialize.Template != null)
            {
                sb.Append(",\"Template\":");
                Serialize(toSerialize.Template, sb);
            }
            if (!(toSerialize.Type is null))
            {
                sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }
            if (toSerialize.V.HasValue) { sb.Append($",\"v\":{toSerialize.V.Value}"); }
            if (toSerialize.Value != null) { sb.Append($",\"Value\":\"{toSerialize.Value.ToJSONValue()}\""); }
            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            sb.Append("}");
        }


        public static void Serialize(this IList<MetaAttribute> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (MetaAttribute thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this MetaAttribute toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.Instance != null)
            {
                sb.Append(",\"Instance\":");
                Serialize(toSerialize.Instance, sb);
            }
            if (toSerialize.IsInherited) { sb.Append(",\"IsInherited\":true"); }
            if (!(toSerialize.Type is null))
            {
                sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }
            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            sb.Append("}");
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.V != null) { sb.Append($",\"v\":\"{toSerialize.V.ToJSONValue()}\""); }
            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            sb.Append("}");
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IHydratable> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IHydratable toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.I != 0) { sb.Append($",\"i\":{toSerialize.I}"); }
            if (toSerialize.IsHydrated) { sb.Append(",\"IsHydrated\":true"); }
            sb.Append("}");
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.Label != null) { sb.Append($",\"Label\":\"{toSerialize.Label.ToJSONValue()}\""); }
            if (toSerialize.Value != null) { sb.Append($",\"Value\":\"{toSerialize.Value.ToJSONValue()}\""); }
            sb.Append("}");
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable> toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            int ElementIndex = 0;
            sb.Append("[");

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    sb.Append(",");
                }

                Serialize(thisChild, sb);
            }

            sb.Append("]");

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable toSerialize, StringBuilder sb)
        {
            Contract.Requires(toSerialize != null);

            sb.Append("{");

            sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            sb.Append("}");
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
