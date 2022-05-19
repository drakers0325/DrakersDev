namespace DrakersDev
{
    public class ConvertHelper
    {
        /// <summary>
        /// 16진수 문자열을 바이트 배열로 변환한다.
        /// </summary>
        /// <param name="hex">16진수 문자열</param>
        /// <returns></returns>
        public static Byte[] ConvertStringToByteArray(String hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// 바이트 배열을 16진수 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String ConvertByteArrayToHexString(Byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", String.Empty);
        }
    }
}
