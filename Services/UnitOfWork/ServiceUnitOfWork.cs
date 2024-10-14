using Domain.DTO.Settings;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Repository.Context;
using Services.Interfaces;
using Services.Services;

namespace Service.UnitOfWork
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
      

        public Lazy<IAuthService> Auth { get; set; }
        public Lazy<IUserService> Users { get; set; }


        public ServiceUnitOfWork
        (
            ApplicationContext _applicationContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<Roles> _roleManager,
            SignInManager<ApplicationUser> _signInManager,
            IOptions<JWT> _jwtConfiguration
        )
        {

            Auth = new Lazy<IAuthService>(() => new AuthService(_signInManager, userManager, _roleManager, _jwtConfiguration));
            Users = new Lazy<IUserService>(() => new UserService(userManager));

        }

        public void Dispose()
        {
        }
    }
}