using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Projekt.Server.Helpers
{
    public class AuthHelper
    {
        public static JwtSecurityToken GetJwtToken(string signingKey, string issuer, string audience, TimeSpan timeout, List<Claim> claims)
        {
            // token is unique
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(signingKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.Add(timeout),
                claims: claims,
                signingCredentials: creds
            );
        }

        public static int GetUserId(ClaimsIdentity identity)
        {
            _ = int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);
            return userId;
        }
    }
}
