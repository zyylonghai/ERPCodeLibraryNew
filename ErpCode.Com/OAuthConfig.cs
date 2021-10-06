using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class OAuthConfig
    {
        /// <summary>
        /// 过期秒数
        /// </summary>
        public const int ExpireIn = 36000;

        /// <summary>
        /// 用户Api相关
        /// </summary>
        public static class UserApi
        {
            public static string Scope = "WeatherForecast";
        }
    }
}
