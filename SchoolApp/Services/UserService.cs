using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SchoolApp.Core.Enums;
using SchoolApp.Core.Filters;
using SchoolApp.Data;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Models;
using SchoolApp.Repositories;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace SchoolApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<UserService>();

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber,
            int pageSize, UserFiltersDTO userFiltersDTO)
        {
            List<User> users = [];
            List<Expression<Func<User, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.UserRole.ToString() == userFiltersDTO.UserRole);
            }

            var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = result.Data.Select(u => new UserReadOnlyDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    UserRole = u.UserRole.ToString()!
                }).ToList(),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
            logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;
        }

        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            try
            {
                User? user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", "User with username: " + " not found");
                }

                logger.LogInformation("User found: {Username}", username);
                return new UserReadOnlyDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    UserRole = user.UserRole.ToString()!
                };
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by username: {Username}. {Message}", username, ex.Message);
                throw;
            }
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = null;
            try
            {
                user = await unitOfWork.UserRepository.GetUserAsync(credentials.Username!, credentials.Password!);

                if (user == null)
                {
                    throw new EntityNotAuthorizedException("User", "Bad Credentials");
                    // see Resources/Strings.resx for localization
                    //throw new EntityNotAuthorizedException("User", Resources.ErrorMessages.BadCredentials); 
                }

                logger.LogInformation("User with username {Username} found", credentials.Username!);
            }
            catch (EntityNotAuthorizedException e)
            {
                logger.LogError("Authentication failed for username {Username}. {Message}",
                    credentials.Username, e.Message);
            }
            return user;
        }

        public string CreateUserToken(int userId, string username, string email, UserRole userRole,
            string appSecurityKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, userRole.ToString())
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            // Serialize the token to a string
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return userToken;
        }

        public async Task<UserReadOnlyDTO> GetUserByIdAsync(int id)
        {
            User? user = null;

            try
            {
                user = await unitOfWork.UserRepository.GetAsync(id);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", "User with id: " + id + " not found");
                }
                logger.LogInformation("User found with ID: {Id}", id);
                return mapper.Map<UserReadOnlyDTO>(user);
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by ID: {Id}. {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<UserTeacherReadOnlyDTO?> GetUserTeacherByUsernameAsync(string username)
        {
            UserTeacherReadOnlyDTO? userTeacherReadOnlyDTO = null;
            try
            {
                userTeacherReadOnlyDTO = await unitOfWork.UserRepository.GetUserTeacherAsync(username);
                if (userTeacherReadOnlyDTO == null)
                {
                    throw new EntityNotFoundException("User", "User with username: " + username + " not found");
                    //return null;
                }
                logger.LogInformation("User found with username={Username}", username);

                return userTeacherReadOnlyDTO;
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError("Error retrieving user by username: {Username}. {Message}", username, e.Message);
                throw;
            }
            //return userTeacherReadOnlyDTO;
        }
    }
}