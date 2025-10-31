using SchoolApp.Data;
using SchoolApp.Models;
using System.Linq.Expressions;

namespace SchoolApp.Repositories
{
    public interface ITeacherRepository
    {
        Task<List<Course>> GetTeacherCoursesAsync(int teacherId);
        Task<Teacher?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetUserTeacherByUsernameAsync(string username);

        Task<PaginatedResult<User>> GetPaginatedUsersTeachersAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<User>> GetPaginatedUsersTeachersFilteredAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}