using JwtGenerator.Jwt;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JwtGenerator.Config
{
    public class TokenConfig
    {
        [JsonProperty(Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Audience { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Issuer { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TokenLifetime Lifetime { get; set; }

        public string Role { get; set; }

        public JwtClaim[] Claims { get; set; }

    }
}
