using System.Reflection;

namespace Penguin.Reflection.Serialization.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetUniquePropertyId(this PropertyInfo pi)
        {
            return $"{pi.DeclaringType.FullName}.{pi.PropertyType.FullName}.{pi.Name}";
        }
    }
}