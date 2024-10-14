using Services.Interfaces;

namespace Service.UnitOfWork
{
    public interface IServiceUnitOfWork : IDisposable
    {
        Lazy<IAuthService> Auth { get; set; }
        Lazy<IUserService> Users { get; set; }
    }
}