using Microsoft.EntityFrameworkCore;
using SchoolApp.Core.Enums;
using SchoolApp.Data;
using SchoolApp.Models;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolApp.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(SchoolAppDbContext context) : base(context)
        {
        }

        public async Task<Student?> GetByAm(string? am)
        {
            return await context.Students
                .Where(s => s.Am == am)
                .SingleOrDefaultAsync(); // fetched zero or one
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersStudentsAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var usersWithRoleStudent = await context.Users
                .Where(u => u.UserRole == UserRole.Student)
                .Include(u => u.Student) // Εager loading της σχετικής οντότητας Student   
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await context.Users
                .Where(u => u.UserRole == UserRole.Student)
                .CountAsync();

            return new PaginatedResult<User>(usersWithRoleStudent, totalRecords, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Student>> GetPaginatedUsersStudentsFilteredAsync(int pageNumber,
            int pageSize, List<Expression<Func<Student, bool>>> predicates)
        {
            IQueryable<Student> query = context.Students;

            // Apply predicates as Expression<Func<Student, bool>> so they run in DB
            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            // Get total count BEFORE pagination
            int totalRecords = await query.CountAsync();

            // Paginate AFTER filtering
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Student>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<Course>> GetStudentCoursesAsync(int studentId)
        {
            List<Course> courses;

            courses = await context.Students
                .Where(s => s.Id == studentId)
                .SelectMany(c => c.Courses)
                .ToListAsync();

            return courses;
        }
    }
}