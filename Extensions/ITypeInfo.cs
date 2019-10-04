using Penguin.Reflection.Serialization.Abstractions.Interfaces;

namespace Penguin.Reflection.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IMetaTypeInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Tests for type equality between an object implementing ITypeInfo and a Type, by full name
        /// </summary>
        /// <param name="o">The object to test</param>
        /// <param name="StringValue">The ToString value of the type</param>
        /// <returns>Whether or not the types are equal</returns>
        public static bool Is(this ITypeInfo o, string StringValue)
        {
            if (o is null)
            {
                return false;
            }

            IMetaType toCheck = o.TypeOf();

            do
            {
                if (toCheck.ToString() == StringValue)
                {
                    return true;
                }
            } while ((toCheck = toCheck.BaseType) != null);

            return false;
        }

        #endregion Methods
    }
}