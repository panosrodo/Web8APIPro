using AutoMapper;
using SchoolApp.Repositories;

namespace SchoolApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public UserService UserService => new(unitOfWork, mapper);
        public TeacherService TeacherService => new(unitOfWork, mapper);
        public StudentService StudentService => new(unitOfWork, mapper);
    }
}