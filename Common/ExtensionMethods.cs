using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Common
{
    public static class ExtensionMethods
    {
        public static double ToRadians(this int angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static TEnum ParseEnumValue<TEnum>(this string value, TEnum? fallback = null)
                where TEnum : struct, IComparable, IFormattable
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }
            var enumValueMember = type.GetMembers().Select(s => new
            {
                Value = s,
                Attribute = s.GetCustomAttributes<EnumMemberAttribute>(false).FirstOrDefault()
            }).FirstOrDefault(s => s.Attribute != null && s.Attribute.Value != null && s.Attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase));


            if (enumValueMember != null && enumValueMember.Value != null)
            {
                return (TEnum)Enum.Parse(type, enumValueMember.Value.Name);
            }
            else if (!fallback.HasValue)
            {
                throw new ArgumentException(@"Value " + value + @" cannot be found in " + type.Name, "value");
            }
            else
            {
                return fallback.Value;
            }
        }
    }
}