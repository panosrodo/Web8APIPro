using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SchoolApp.Helpers
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            if (authAttributes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                // Add security requirement
                operation.Security = new List<OpenApiSecurityRequirement>();

                var roles = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                    .SelectMany(attr => attr.Roles!.Split(','));

                // Add the security requirment for JWT Bearer with specified roles
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                                Description = "Add token to header",
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                In = ParameterLocation.Header,
                                Scheme = JwtBearerDefaults.AuthenticationScheme,
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                        },
                        roles.ToList()
                    }
                });
            }
        }
    }
}