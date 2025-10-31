using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(SchoolAppDbContext context) : base(context)
        {
        }

        public async Task<List<Student>> GetCourseStudentsAsync(int courseId)
        {
            return await context.Courses
               .Where(c => c.Id == courseId)
               .SelectMany(c => c.Students)
               .ToListAsync();
        }

        public async Task<Teacher?> GetCourseTeacherAsync(int courseId)
        {
            //var course = await context.Courses
            //    .Where(c => c.Id == courseId)
            //    .FirstOrDefaultAsync();

            //return course?.Teacher;   // since lazy loaded in enabled, makes a second query

            var course = await context.Courses
                    .Include(c => c.Teacher) // agerly loads related entities in the same query
                    .FirstOrDefaultAsync(c => c.Id == courseId);

            return course?.Teacher; // not second query, since teacher has loaded
        }
    }
}