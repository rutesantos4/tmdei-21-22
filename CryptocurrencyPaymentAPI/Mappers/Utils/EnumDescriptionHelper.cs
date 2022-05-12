namespace CryptocurrencyPaymentAPI.Mappers.Utils
{
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumDescriptionHelper
    {
        public static string GetEnumValueAsString<T>(this T stateEnum)
        {
            var state = stateEnum?.ToString() ?? string.Empty;
            return GetValue(stateEnum.GetType().GetField(state), state);
        }

        private static string GetValue(FieldInfo fieldInfo, string state)
        {
            var customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (customAttributes.Length > 0)
            {
                return (customAttributes[0] as DescriptionAttribute).Description;
            }
            return state;
        }
    }
}
