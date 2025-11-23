using AutoMapper;
using SchoolApp.Core.Enums;
using SchoolApp.Data;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Repositories;
using SchoolApp.Security;
using Serilog;

namespace SchoolApp.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<TeacherService> logger = new LoggerFactory().AddSerilog().CreateLogger<TeacherService>();

        public TeacherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<UserReadOnlyDTO> SignUpUserAsync(TeacherSignupDTO request)
        {
            Teacher teacher = ExtractTeacher(request);
            User user = ExtractUser(request);

            try
            {
                user.Password = EncryptionUtil.Encrypt(user.Password);
                await unitOfWork.UserRepository.AddAsync(user);
                await unitOfWork.TeacherRepository.AddAsync(teacher);

                user.Teacher = teacher;
                // teacher.User = user; EF manages the other-end of the relationship since both entities are attached

                await unitOfWork.SaveAsync();
                logger.LogInformation("Teacher {Teacher} signed up successfully.", teacher);
                return mapper.Map<UserReadOnlyDTO>(user);
            }
            catch (EntityAlreadyExistsException ex)
            {
                logger.LogError("Error signing up tecaher {Teacher}. {Message}", teacher, ex.Message);
                throw;
            }
        }

        private User ExtractUser(TeacherSignupDTO signupDTO)
        {
            return new User()
            {
                Username = signupDTO.Username!,
                Password = signupDTO.Password!,
                Email = signupDTO.Email!,
                Firstname = signupDTO.Firstname!,
                Lastname = signupDTO.Lastname!,
                //UserRole = signupDTO.UserRole
                UserRole = UserRole.Teacher
            };
        }

        private Teacher ExtractTeacher(TeacherSignupDTO? signupDTO)
        {
            return new Teacher()
            {
                PhoneNumber = signupDTO!.PhoneNumber!,
                Institution = signupDTO.Institution!
            };
        }
    }
}