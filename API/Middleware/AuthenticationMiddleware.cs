using Domain.DTO.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace API.AppMiddleware
{
    public static class AuthenticationMiddleware
    {
        static AuthenticationMiddleware()
        {
        }

        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtConfig = new JWT();
            config.Bind("JWT", jwtConfig);

            // Load JWT configuration values
            var secret = jwtConfig.PrivateKey;
            var validIssuer = jwtConfig.ValidIssuer;
            var validAudience = jwtConfig.ValidAudience;

            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(
                source: Convert.FromBase64String(secret), // Use the private key to sign tokens
                bytesRead: out _);

            var signingCredentials = new SigningCredentials(
                 key: new RsaSecurityKey(rsa),
                 algorithm: SecurityAlgorithms.RsaSha256 // Important to use RSA version of the SHA algo 
            );

            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = signingCredentials.Key,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    ValidIssuer = validIssuer,
                    RequireExpirationTime = true,
                    ValidAudience = validAudience
                };
            });

            return services;
        }
    }
}
