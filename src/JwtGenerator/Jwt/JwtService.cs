using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtGenerator.Config;
using Microsoft.IdentityModel.Tokens;

namespace JwtGenerator.Jwt
{
    public static class JwtService
    {
        public static string CreateToken(TokenConfig config)
        {
            byte[] tokenData = Convert.FromBase64String(config.Key);
            var securityKey = new SymmetricSecurityKey(tokenData);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var jwtHeader = new JwtHeader(signingCredentials);

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, config.Role));

            foreach (var claim in config.Claims)
            {
                claimsIdentity.AddClaim(new Claim(claim.Type, claim.Value));
            }

            int tokenLifetime = GetTokenLifetime(config.Lifetime);
            var jwtPayload = new JwtPayload(config.Issuer, config.Audience, claimsIdentity.Claims, 
                null, DateTime.Now.AddSeconds(tokenLifetime));
            var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwtToken);
        }

        private static int GetTokenLifetime(TokenLifetime lifetime)
        {
            switch (lifetime)
            {
                case TokenLifetime.OneHour:
                    return 3600;
                case TokenLifetime.TwentyFourHours:
                    return 24 * 3600;
                case TokenLifetime.OneMonth:
                    return 30 * 24 * 3600;
                case TokenLifetime.OneYear:
                    return 365 * 24 * 3600;
                default:
                    return 0;
            }
        }
    }
}
