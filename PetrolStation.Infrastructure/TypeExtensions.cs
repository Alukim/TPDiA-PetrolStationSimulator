using System;

namespace PetrolStation.Infrastructure
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets a friendly name of a generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <see href="https://stackoverflow.com/questions/401681/how-can-i-get-the-correct-text-definition-of-a-generic-type-using-reflection"/>
        /// <returns></returns>
        public static string GetFriendlyTypeName(this Type type)
        {
            if (type.IsArray)
            {
                return string.Format("{0}[]", type.GetElementType().GetFriendlyTypeName());
            }

            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            if (!type.IsGenericType)
            {
                return type.FullName;
            }

            var builder = new System.Text.StringBuilder();
            var name = type.Name;
            var index = name.IndexOf("`", StringComparison.Ordinal);
            builder.AppendFormat("{0}.{1}", type.Namespace, name.Substring(0, index));
            builder.Append('<');
            var first = true;
            foreach (var arg in type.GetGenericArguments())
            {
                if (!first)
                {
                    builder.Append(',');
                }
                builder.Append(GetFriendlyTypeName(arg));
                first = false;
            }
            builder.Append('>');
            return builder.ToString();
        }
    }
}
