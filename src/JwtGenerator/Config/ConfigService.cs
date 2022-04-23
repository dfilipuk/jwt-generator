using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JwtGenerator.Config
{
    public static class ConfigService
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static TokenConfig LoadConfigFromFile(string path)
        {
            string configJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<TokenConfig>(configJson, _serializerSettings);
        }

        public static void SaveConfigIntoFile(string path, TokenConfig config)
        {
            string configJson = JsonConvert.SerializeObject(config, _serializerSettings);
            File.WriteAllText(path, configJson);
        }
    }
}
