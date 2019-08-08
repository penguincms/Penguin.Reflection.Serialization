using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Penguin.Reflection.Extensions;


namespace Penguin.Reflection.Serialization.Templating
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static partial class MetaSerializer {

		private static string _assemblyName {get;set;}

		public static string AssemblyName {
			get {
				if (string.IsNullOrWhiteSpace(_assemblyName)) {
					_assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
				}

				return _assemblyName;
			}
		}

        public static string Serialize(this IList<Enum> toSerialize) {

            List<string> Properties = new  List<string>();

            foreach(Enum thisChild in toSerialize) {
                Properties.Add(System.Convert.ChangeType(toSerialize, Enum.GetUnderlyingType(toSerialize.GetType())).ToString());
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string SerializeDictionary(this IDictionary toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(DictionaryEntry thisChild in toSerialize) {

            if(thisChild.Value.GetType() == typeof(String)) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":\"" + thisChild.Value.ToJSONValue() + "\"");
                    continue;
            }

                

                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.Value.GetType())) { 
                    Properties.Add("\"" + thisChild.Key.ToString() +"\":" + Serialize(thisChild.Value as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable));
                    continue;
                }

  
                throw new Exception("Template does not support Dictionary Value type");
            }

            return "{" + string.Join(",", Properties) + "}";

        }

        public static string SerializeList(this IList toSerialize) {

            List<string> Properties = new  List<string>();

            foreach(object thisChild in toSerialize) {
                if(thisChild is null) { continue; }

                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue));
                    continue;
                }


                if(typeof(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable).IsAssignableFrom(thisChild.GetType())) { 
                    Properties.Add(Serialize(thisChild as Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable));
                    continue;
                }

        
}

            return "[" + string.Join(",", Properties) + "]";

        }

          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaType toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

              if (toSerialize.AssemblyQualifiedName != null) { Properties.Add("\"AssemblyQualifiedName\":\"" + toSerialize.AssemblyQualifiedName.ToJSONValue() + "\""); }
             if (toSerialize.BaseType != null) { Properties.Add("\"BaseType\":" + Serialize(toSerialize.BaseType)); }
             if (toSerialize.CollectionType != null) { Properties.Add("\"CollectionType\":" + Serialize(toSerialize.CollectionType)); }
            if ((int)toSerialize.CoreType != 0) { Properties.Add("\"CoreType\":" + System.Convert.ChangeType(toSerialize.CoreType, Enum.GetUnderlyingType(toSerialize.CoreType.GetType())).ToString()); }
              if (toSerialize.Default != null) { Properties.Add("\"Default\":\"" + toSerialize.Default.ToJSONValue() + "\""); }
              if (toSerialize.FullName != null) { Properties.Add("\"FullName\":\"" + toSerialize.FullName.ToJSONValue() + "\""); }
             if (toSerialize.IsArray) { Properties.Add("\"IsArray\":true"); }
             if (toSerialize.IsEnum) { Properties.Add("\"IsEnum\":true"); }
             if (toSerialize.IsNullable) { Properties.Add("\"IsNullable\":true"); }
             if (toSerialize.IsNumeric) { Properties.Add("\"IsNumeric\":true"); }
              if (toSerialize.Name != null) { Properties.Add("\"Name\":\"" + toSerialize.Name.ToJSONValue() + "\""); }
              if (toSerialize.Namespace != null) { Properties.Add("\"Namespace\":\"" + toSerialize.Namespace.ToJSONValue() + "\""); }
             if (toSerialize.Parameters != null && toSerialize.Parameters.Any()) { Properties.Add("\"Parameters\":" + SerializeList(toSerialize.Parameters as IList)); }
             if (toSerialize.Values != null && toSerialize.Values.Any()) { Properties.Add("\"Values\":" + SerializeList(toSerialize.Values as IList)); }
            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
             if (toSerialize.Attributes != null && toSerialize.Attributes.Any()) { Properties.Add("\"Attributes\":" + SerializeList(toSerialize.Attributes.ToList())); }
             if (toSerialize.Properties != null && toSerialize.Properties.Any()) { Properties.Add("\"Properties\":" + SerializeList(toSerialize.Properties as IList)); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaProperty toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

             if (toSerialize.DeclaringType != null) { Properties.Add("\"DeclaringType\":" + Serialize(toSerialize.DeclaringType)); }
              if (toSerialize.Name != null) { Properties.Add("\"Name\":\"" + toSerialize.Name.ToJSONValue() + "\""); }
             if (toSerialize.Type != null) { Properties.Add("\"Type\":" + Serialize(toSerialize.Type)); }
            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
             if (toSerialize.Attributes != null && toSerialize.Attributes.Any()) { Properties.Add("\"Attributes\":" + SerializeList(toSerialize.Attributes.ToList())); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaObject toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

             if (toSerialize.BuildExceptions != null && toSerialize.BuildExceptions.Keys.Count > 0) { Properties.Add("\"BuildExceptions\":" + SerializeDictionary(toSerialize.BuildExceptions as IDictionary)); }
             if (toSerialize.CollectionItems != null && toSerialize.CollectionItems.Any()) { Properties.Add("\"CollectionItems\":" + SerializeList(toSerialize.CollectionItems as IList)); }
            if (toSerialize.Exception != 0) { Properties.Add("\"Exception\":" + toSerialize.Exception.ToString()); }
             if (toSerialize.Meta != null && toSerialize.Meta.Keys.Count > 0) { Properties.Add("\"Meta\":" + SerializeDictionary(toSerialize.Meta as IDictionary)); }
            if (toSerialize.v.HasValue) { Properties.Add("\"v\":" + toSerialize.v.Value.ToString()); }
             if (toSerialize.Null) { Properties.Add("\"Null\":true"); }
             if (toSerialize.Properties != null && toSerialize.Properties.Any()) { Properties.Add("\"Properties\":" + SerializeList(toSerialize.Properties as IList)); }
             if (toSerialize.Property != null) { Properties.Add("\"Property\":" + Serialize(toSerialize.Property)); }
             if (toSerialize.Template != null) { Properties.Add("\"Template\":" + Serialize(toSerialize.Template)); }
             if (toSerialize.Type != null) { Properties.Add("\"Type\":" + Serialize(toSerialize.Type)); }
              if (toSerialize.Value != null) { Properties.Add("\"Value\":\"" + toSerialize.Value.ToJSONValue() + "\""); }
            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaAttribute toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

             if (toSerialize.Instance != null) { Properties.Add("\"Instance\":" + Serialize(toSerialize.Instance)); }
             if (toSerialize.IsInherited) { Properties.Add("\"IsInherited\":true"); }
             if (toSerialize.Type != null) { Properties.Add("\"Type\":" + Serialize(toSerialize.Type)); }
            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IStringHolder toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

              if (toSerialize.v != null) { Properties.Add("\"v\":\"" + toSerialize.v.ToJSONValue() + "\""); }
            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IAbstractMeta toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            if (toSerialize.i != 0) { Properties.Add("\"i\":" + toSerialize.i.ToString()); }
             if (toSerialize.IsHydrated) { Properties.Add("\"IsHydrated\":true"); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IEnumValue toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

              if (toSerialize.Label != null) { Properties.Add("\"Label\":\"" + toSerialize.Label.ToJSONValue() + "\""); }
              if (toSerialize.Value != null) { Properties.Add("\"Value\":\"" + toSerialize.Value.ToJSONValue() + "\""); }
            return "{" + string.Join(",", Properties) + "}";
        }
          

        public static string Serialize(this IList<Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable> toSerialize) {

            List<string> Properties = new  List<string>();


            foreach(Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable thisChild in toSerialize) {
                Properties.Add(Serialize(thisChild));
            }

            return "[" + string.Join(",", Properties) + "]";

        }

        public static string Serialize(this Penguin.Reflection.Serialization.Abstractions.Interfaces.IMetaSerializable toSerialize) {
            List<string> Properties = new  List<string>();

            Properties.Add($"\"$type\":\"{toSerialize.GetType().FullName}, {AssemblyName}\"");

            return "{" + string.Join(",", Properties) + "}";
        }
    } 

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
