using Domain.DTO;
using Domain.DTO.Auth;
using Domain.DTO.Others;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileResponse> GetUserById(long Id)
        {
            var User = await _userManager.FindByIdAsync(Id.ToString()) ?? throw new ValidationException("User Not Found");

            return new UserProfileResponse
            {
                Email = User.Email,
                MobileNumber = User.PhoneNumber,
                Name = User.FullName,
                ImagePath = User.ImagePath
            };
        }

        public async Task<BaseListResponse<UserProfileResponse>> GetUserList(BaseSearch baseSearch)
        {
            var users = await _userManager.Users.AsNoTracking().Where(x => (string.IsNullOrEmpty(baseSearch.Name) ||
                                                                      x.FullName.Contains(baseSearch.Name)  ||
                                                                      x.Email.Contains(baseSearch.Name)))
                                                                  .Skip(baseSearch.PageSize * baseSearch.PageNumber)
                                                                  .Take(baseSearch.PageSize)
                                                                  .Select(x => new UserProfileResponse
                                                                  {
                                                                      Email = x.Email,
                                                                      Name = x.FullName,
                                                                      MobileNumber = x.PhoneNumber,
                                                                      ImagePath = x.ImagePath
                                                                  }).ToListAsync();

            int totalCount = _userManager.Users.Count();

           

            return new BaseListResponse<UserProfileResponse>
            {
                entities = users,
                TotalCount = totalCount
            };
        }

        public Task UploadExcel(FileUploadRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
