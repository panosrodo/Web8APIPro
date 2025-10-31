namespace SchoolApp.Repositories
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        StudentRepository StudentRepository { get; }
        TeacherRepository TeacherRepository { get; }
        CourseRepository CourseRepository { get; }

        Task<bool> SaveAsync();
    }
}