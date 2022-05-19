using System.Reflection;

namespace DrakersDev
{
    public static class AppPathHelper
    {
        /// <summary>
        /// 현재 실행중인 프로그램 실행 위치를 가져온다.
        /// </summary>
        /// <returns>실행 위치</returns>
        public static String? GetCurrentApplicationExecutePath()
        {
            Assembly? ea = Assembly.GetEntryAssembly();
            return ea != null ? System.IO.Path.GetDirectoryName(ea.Location) :
                System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}