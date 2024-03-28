using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace EShop.Repository
{
    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            //dl bang rong tra ve mac dinh bat ki kieu dl nao khong thi DeserializeObject: chuyen dl thanh json
            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
