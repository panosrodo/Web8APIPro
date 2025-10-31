namespace SchoolApp.Services
{
    public interface IApplicationService
    {
        UserService UserService { get; }
        TeacherService TeacherService { get; }
        StudentService StudentService { get; }
        // Other services can be added here as needed
    }
}