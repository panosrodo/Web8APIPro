using SchoolApp.Core.Filters;
using SchoolApp.Data;
using SchoolApp.DTO;
using SchoolApp.Models;

namespace SchoolApp.Services
{
    public interface IUserService
    {
        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber,
            int pageSize, UserFiltersDTO userFiltersDTO);
        Task<UserReadOnlyDTO> GetUserByIdAsync(int id);
        Task<UserTeacherReadOnlyDTO?> GetUserTeacherByUsernameAsync(string username);
    }
}