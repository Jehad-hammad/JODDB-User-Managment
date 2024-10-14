using Domain.DTO;
using Domain.DTO.Auth;
using Domain.DTO.Others;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseListResponse<UserProfileResponse>> GetUserList(BaseSearch baseSearch);
        Task<UserProfileResponse> GetUserById(long Id);
        Task UploadExcel(FileUploadRequest request);
    }
}
