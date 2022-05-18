namespace CryptocurrencyPaymentAPI.Mappers.Utils
{
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumDescriptionHelper
    {
        public static string GetEnumValueAsString<T>(this T stateEnum)
        {
            if (stateEnum == null || stateEnum.GetType() == null) { return string.Empty; }
            var state = stateEnum.ToString() ?? string.Empty;
            if (stateEnum.GetType().GetField(state) == null) { return state; }
            return GetValue(stateEnum.GetType().GetField(state), state);
        }

        private static string GetValue(FieldInfo fieldInfo, string state)
        {
            var customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (customAttributes.Length > 0)
            {
                return ((DescriptionAttribute)customAttributes[0]).Description;
            }
            return state;
        }
    }
}
