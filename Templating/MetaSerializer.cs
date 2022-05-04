using Penguin.Reflection.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/* Unmerged change from project 'Penguin.Reflection.Serialization.Local (netstandard2.1)'
Before:
namespace Penguin.Reflection.Serialization.Templating
After:
using System.Text;


namespace Penguin.Reflection.Serialization.Templating
*/



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


            _ = sb.Append('[');

            int ElementIndex = 0;
            foreach (Enum thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                _ = sb.Append(System.Convert.ChangeType(toSerialize, Enum.GetUnderlyingType(toSerialize.GetType())).ToString());
            }

            _ = sb.Append(']');

        }

        public static void SerializeDictionary(this IDictionary toSerialize, StringBuilder sb)
        {



            _ = sb.Append('{');

            int ElementIndex = 0;

            foreach (DictionaryEntry thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                if (thisChild.Value.GetType() == typeof(string))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":\"{thisChild.Value.ToJSONValue()}\"");
                    continue;
                }


                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.Value.GetType()))
                {
                    _ = sb.Append($"\"{thisChild.Key.ToString()}\":");
                    Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable, sb);
                    continue;
                }

                throw new Exception("Template does not support Dictionary Value type");
            }

            _ = sb.Append('}');

        }

        public static void SerializeList(this IList toSerialize, StringBuilder sb)
        {

            _ = sb.Append('[');

            int ElementIndex = 0;
            foreach (object thisChild in toSerialize)
            {
                if (thisChild is null)
                {
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta, sb);
                    continue;
                }

                if (typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.GetType()))
                {
                    if (ElementIndex++ > 0)
                    {
                        _ = sb.Append(',');
                    }

                    Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable, sb);
                    continue;
                }
            }

            _ = sb.Append(']');

        }



        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.AssemblyQualifiedName != null)
            {
                _ = sb.Append($",\"AssemblyQualifiedName\":\"{toSerialize.AssemblyQualifiedName.ToJSONValue()}\"");
            }

            if (toSerialize.BaseType != null)
            {
                _ = sb.Append(",\"BaseType\":");
                Serialize(toSerialize.BaseType, sb);
            }

            if (toSerialize.CollectionType != null)
            {
                _ = sb.Append(",\"CollectionType\":");
                Serialize(toSerialize.CollectionType, sb);
            }

            if (toSerialize.CoreType != 0)
            {
                _ = sb.Append($",\"CoreType\":{System.Convert.ChangeType(toSerialize.CoreType, Enum.GetUnderlyingType(toSerialize.CoreType.GetType()))}");
            }

            if (toSerialize.Default != null)
            {
                _ = sb.Append($",\"Default\":\"{toSerialize.Default.ToJSONValue()}\"");
            }

            if (toSerialize.FullName != null)
            {
                _ = sb.Append($",\"FullName\":\"{toSerialize.FullName.ToJSONValue()}\"");
            }

            if (toSerialize.IsArray)
            {
                _ = sb.Append(",\"IsArray\":true");
            }

            if (toSerialize.IsEnum)
            {
                _ = sb.Append(",\"IsEnum\":true");
            }

            if (toSerialize.IsNullable)
            {
                _ = sb.Append(",\"IsNullable\":true");
            }

            if (toSerialize.IsNumeric)
            {
                _ = sb.Append(",\"IsNumeric\":true");
            }

            if (toSerialize.Name != null)
            {
                _ = sb.Append($",\"Name\":\"{toSerialize.Name.ToJSONValue()}\"");
            }

            if (toSerialize.Namespace != null)
            {
                _ = sb.Append($",\"Namespace\":\"{toSerialize.Namespace.ToJSONValue()}\"");
            }

            if (toSerialize.Parameters is IList il_Parameters && il_Parameters.Count > 0)
            {
                _ = sb.Append(",\"Parameters\":");
                SerializeList(il_Parameters, sb);
            }

            if (toSerialize.Values is IList il_Values && il_Values.Count > 0)
            {
                _ = sb.Append(",\"Values\":");
                SerializeList(il_Values, sb);
            }

            if (toSerialize.Attributes != null && toSerialize.Attributes.Any())
            {
                _ = sb.Append(",\"Attributes\":");
                SerializeList(toSerialize.Attributes.ToList(), sb);
            }

            if (toSerialize.Properties is IList il_Properties && il_Properties.Count > 0)
            {
                _ = sb.Append(",\"Properties\":");
                SerializeList(il_Properties, sb);
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.DeclaringType != null)
            {
                _ = sb.Append(",\"DeclaringType\":");
                Serialize(toSerialize.DeclaringType, sb);
            }

            if (toSerialize.Name != null)
            {
                _ = sb.Append($",\"Name\":\"{toSerialize.Name.ToJSONValue()}\"");
            }

            if (toSerialize.Type != null)
            {
                _ = sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }

            if (toSerialize.Attributes != null && toSerialize.Attributes.Any())
            {
                _ = sb.Append(",\"Attributes\":");
                SerializeList(toSerialize.Attributes.ToList(), sb);
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.V != null)
            {
                _ = sb.Append($",\"V\":\"{toSerialize.V.ToJSONValue()}\"");
            }

            if (toSerialize.I != 0)
            {
                _ = sb.Append($",\"I\":{toSerialize.I}");
            }

            if (toSerialize.IsHydrated)
            {
                _ = sb.Append(",\"IsHydrated\":true");
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if ((toSerialize as Penguin.Reflection.Serialization.Objects.MetaObject)?.Meta is IDictionary id)
            {
                SerializeDictionary(id, sb);
            }

            if (toSerialize.CollectionItems is IList il_CollectionItems && il_CollectionItems.Count > 0)
            {
                _ = sb.Append(",\"CollectionItems\":");
                SerializeList(il_CollectionItems, sb);
            }

            if (toSerialize.Null)
            {
                _ = sb.Append(",\"Null\":true");
            }

            if (toSerialize.Properties is IList il_Properties && il_Properties.Count > 0)
            {
                _ = sb.Append(",\"Properties\":");
                SerializeList(il_Properties, sb);
            }

            if (toSerialize.Property != null)
            {
                _ = sb.Append(",\"Property\":");
                Serialize(toSerialize.Property, sb);
            }

            if (toSerialize.Template != null)
            {
                _ = sb.Append(",\"Template\":");
                Serialize(toSerialize.Template, sb);
            }

            if (toSerialize.Type != null)
            {
                _ = sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }

            if (toSerialize.Value != null)
            {
                _ = sb.Append($",\"Value\":\"{toSerialize.Value.ToJSONValue()}\"");
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.Instance != null)
            {
                _ = sb.Append(",\"Instance\":");
                Serialize(toSerialize.Instance, sb);
            }

            if (toSerialize.IsInherited)
            {
                _ = sb.Append(",\"IsInherited\":true");
            }

            if (toSerialize.Type != null)
            {
                _ = sb.Append(",\"Type\":");
                Serialize(toSerialize.Type, sb);
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.Label != null)
            {
                _ = sb.Append($",\"Label\":\"{toSerialize.Label.ToJSONValue()}\"");
            }

            if (toSerialize.Value != null)
            {
                _ = sb.Append($",\"Value\":\"{toSerialize.Value.ToJSONValue()}\"");
            }

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            _ = sb.Append('}');
        }


        public static void Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable> toSerialize, StringBuilder sb)
        {


            int ElementIndex = 0;
            _ = sb.Append('[');

            foreach (Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable thisChild in toSerialize)
            {
                if (ElementIndex++ > 0)
                {
                    _ = sb.Append(',');
                }

                Serialize(thisChild, sb);
            }

            _ = sb.Append(']');

        }

        public static void Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable toSerialize, StringBuilder sb)
        {


            _ = sb.Append('{');

            _ = sb.Append($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            _ = sb.Append('}');
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
