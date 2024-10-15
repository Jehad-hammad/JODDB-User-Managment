using Domain.DTO;
using Domain.DTO.Settings;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly JWT _jwtConfiguration;
        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<Roles> roleManager , IOptions<JWT> options)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfiguration = options.Value;
        }

        public async Task<TokenResponseDTO> Login(LoginRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName.Equals(request.UserName)) ?? throw new ValidationException("UserNotFound");

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (signInResult.Succeeded)
            {
                return await BuildUserLoginObjectAsync(user, await BuildClaims(user), await _userManager.GetRolesAsync(user));
            }

            throw new ValidationException("PasswordInCorrect");
        }

        public async Task<bool> RegistNewUser(UserRegistrationRequest request)
        {
            if (_userManager.Users.Any(user => user.Email == request.Email)) throw new ValidationException("Email Used !!");

            if (_userManager.Users.Any(user => user.PhoneNumber == request.MobileNumber)) throw new ValidationException("Mobile Number is used !!");


            var applicationUser = new ApplicationUser
            {
                Email = request.Email,
                PhoneNumber = request.MobileNumber,
                FullName = request.Name,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = request.Email,
                ImagePath = request.filePath,
            };

            IdentityResult result = await _userManager.CreateAsync(applicationUser, request.Password);

            if (result.Succeeded)
            {
                var isRoleExists = await CheckRole("User");
                await _userManager.AddToRoleAsync(applicationUser, "User");
                return true;
            }
            else if (result.Errors.Count() > 0)
            {
                var message = string.Empty;

                foreach (var error in result.Errors)
                {
                    message += error.Description + " ";
                }
                throw new ValidationException(message);
            }

            throw new ValidationException("SomethingWentWrong");
        }

        private async Task<bool> CheckRole(string roleName)
        {
            if (_roleManager.Roles.Any(x => x.Name.Equals(roleName)))
                return true;
            else
            {
                var role = new Roles
                {
                    Name = roleName,
                    NormalizedName = roleName,
                };
                await _roleManager.CreateAsync(role);

                return true;
            }


        }

        private async Task<TokenResponseDTO> BuildUserLoginObjectAsync(ApplicationUser user, List<Claim> claims, IList<string> roles)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
            var res = await _userManager.UpdateAsync(user);

            TokenResponseDTO response = new TokenResponseDTO
            {
                AccessToken = WriteToken(claims),
                UserId = user.Id.ToString(),
                Email = user.Email,
                FullName = user.FullName,
            };
            return response;
        }

        private async Task<List<Claim>> BuildClaims(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, string.IsNullOrEmpty(user.Id.ToString())? "":user.Id.ToString()),
                new Claim(ClaimTypes.Email, string.IsNullOrEmpty(user.Email)? "":user.Email),
                new Claim(ClaimTypes.MobilePhone, string.IsNullOrEmpty(user.PhoneNumber)? "":user.PhoneNumber),
                new Claim(ClaimTypes.UserData, string.IsNullOrEmpty(user.UserName)? "":user.UserName),
                new Claim(ClaimTypes.Name, string.IsNullOrEmpty(user.FullName)? "":user.FullName),
            };

            foreach (var item in roles)
            {
                var roleclaim = new Claim(ClaimTypes.Role, item);
                claims.Add(roleclaim);
            }
            return claims;
        }

        private string WriteToken(IList<Claim> claims)
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(
                source: Convert.FromBase64String(_jwtConfiguration.PrivateKey), // Use the private key to sign tokens
                bytesRead: out _);

            var signingCredentials = new SigningCredentials(
                 key: new RsaSecurityKey(rsa),
                 algorithm: SecurityAlgorithms.RsaSha256 // Important to use RSA version of the SHA algo 
            );

            int.TryParse(_jwtConfiguration.TokenValidityInMinutes, out var tokenValidityInMinutes);

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                    issuer: _jwtConfiguration.ValidIssuer,
                    audience: _jwtConfiguration.ValidAudience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                    signingCredentials: signingCredentials);
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
