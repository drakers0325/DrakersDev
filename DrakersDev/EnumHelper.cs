using System.ComponentModel;

namespace DrakersDev
{
    public static class EnumHelper
    {
        /// <summary>
        /// 열거형의 Description 값들을 가져온다.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="exceptValues">제외시킬 값</param>
        /// <returns></returns>
        public static String[] GetEnumDescriptions<T>(params T[] exceptValues)
        {
            var valueList = new List<String>();
            var enumArray = Enum.GetValues(typeof(T));

            foreach (T eachValue in enumArray)
            {
                if (!exceptValues.Contains(eachValue))
                {
                    String? value = eachValue.ToString();
                    if (value != null)
                    {
                        var enumFieldInfo = eachValue.GetType().GetField(value);
                        if (enumFieldInfo != null)
                        {
                            var attributes = enumFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).ToArray();
                            String displayName = attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : value;
                            valueList.Add(displayName);
                        }
                    }
                }
            }

            return valueList.ToArray();
        }

        /// <summary>
        /// 해당 값의 Description 값을 가져온다.
        /// </summary>
        /// <returns>Description 값</returns>
        public static String GetEnumDescription<T>(T enumValue) where T : Enum
        {
            String value = enumValue.ToString();
            var enumFieldInfo = enumValue.GetType().GetField(value);
            if (enumFieldInfo != null)
            {
                Object[] attributes = enumFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).ToArray();
                return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 해당 Description의 열거형값을 가져온다.
        /// </summary>
        /// <param name="enumType">열거형 타입</param>
        /// <param name="description">Description</param>
        /// <returns></returns>
        public static T GetEnumValue<T>(String description) where T : Enum
        {
            Array enumArray = Enum.GetValues(typeof(T));

            foreach (T eachValue in enumArray)
            {
                String value = eachValue.ToString();
                var enumFieldInfo = eachValue.GetType().GetField(value);
                if (enumFieldInfo != null)
                {
                    var attributes = enumFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).ToArray();
                    if (attributes.Length > 0)
                    {
                        if (((DescriptionAttribute)attributes[0]).Description.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return eachValue;
                        }
                    }
                    else
                    {
                        if (value.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return eachValue;
                        }
                    }
                }
            }
            throw new ApplicationException($"'{description}'에 맞는 '{typeof(T)}'열거형의 값이 없습니다");
        }
    }
}
