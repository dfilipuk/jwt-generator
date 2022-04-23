using Newtonsoft.Json;

namespace JwtGenerator.Jwt
{
    public class JwtClaim
    {
        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Value { get; set; }
    }
}
