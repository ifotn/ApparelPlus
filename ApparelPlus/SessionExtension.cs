using Newtonsoft.Json;

namespace ApparelPlus
{
    public static class SessionExtension
    {
        // source: https://www.talkingdotnet.com/store-complex-objects-in-asp-net-core-session/
        // purpose: extend asp.net session var support to store complex objects
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
