using Microsoft.AspNetCore.Mvc;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Services;

namespace SchoolApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IConfiguration configuration;

        public UsersController(IApplicationService applicationService, IConfiguration configuration) :
            base(applicationService)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<UserReadOnlyDTO>> SignupUserTeacherAsync(TeacherSignupDTO teacherSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(e => e.Value!.Errors.Any())
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                    });
                throw new InvalidRegistrationException("ErrorsInRegistration" + errors);
            }

            UserReadOnlyDTO returnedUserDTO = await applicationService.TeacherService.SignUpUserAsync(teacherSignupDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = returnedUserDTO.Id }, returnedUserDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            UserReadOnlyDTO userReadOnlyDTO = await applicationService.UserService.GetUserByIdAsync(id);
            return Ok(userReadOnlyDTO);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserTeacherReadOnlyDTO>> GetUserTeacherByUsernameAsync(string? username)
        {
            var returnedUserDTO = await applicationService.UserService.GetUserTeacherByUsernameAsync(username!);
            return Ok(returnedUserDTO);
        }

        [HttpPost]
        public async Task<ActionResult<JwtTokenDTO>> LoginUserAsync(UserLoginDTO credentials)
        {
            var user = await applicationService.UserService.VerifyAndGetUserAsync(credentials)
                ?? throw new EntityNotAuthorizedException("User", "Bad Credentials");

            var token = applicationService.UserService.CreateUserToken(user.Id, user.Username, user.Email,
                user.UserRole, configuration["Authentication:SecretKey"]!);
            JwtTokenDTO userToken = new JwtTokenDTO { Token = token };
            return Ok(userToken);
        }
    }
}