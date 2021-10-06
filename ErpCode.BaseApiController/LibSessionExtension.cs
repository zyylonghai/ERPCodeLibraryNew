using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.BaseApiController
{
    public static class LibSessionExtension
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            string val = session.GetString(key);
            if (val == null) return default(T);
            return JsonConvert.DeserializeObject<T>(val);
        }
    }
}
