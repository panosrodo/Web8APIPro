using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.DTO;
using SchoolApp.Models;
using SchoolApp.Security;
using System.Linq.Expressions;

namespace SchoolApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(SchoolAppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username
            || u.Email == username);

            if (user == null) return null;

            if (!EncryptionUtil.IsValidPassword(password, user.Password)) return null;

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await context.Users.FirstOrDefaultAsync(u => u.Username == username);


        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users; // δεν εκτελείται

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // εκτελείται, υπονοείται το AND
                }
            }

            int totalRecords = await query.CountAsync(); // εκτελείται

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελείται

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<UserTeacherReadOnlyDTO?> GetUserTeacherAsync(string? username)
        {
            var userTeacher = await context.Users
                .Where(u => u.Username == username)
                .Include(u => u.Teacher)
                .Select(u => new UserTeacherReadOnlyDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    UserRole = u.UserRole.ToString()!,
                    PhoneNumber = u.Teacher!.PhoneNumber,
                    Institution = u.Teacher.Institution
                })
                .FirstOrDefaultAsync();
            Console.WriteLine("UserTeacher: " + userTeacher!.Firstname + ", " + userTeacher.Institution);
            return userTeacher;
        }
    }
}