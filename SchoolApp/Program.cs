
using Microsoft.EntityFrameworkCore;
using SchoolApp.Configuration;
using SchoolApp.Data;
using SchoolApp.Repositories;
using Serilog;

namespace SchoolApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            connString = connString!.Replace("{DB_PASS}", Environment.GetEnvironmentVariable("DB_PASS") ?? "");

            builder.Services.AddDbContext<SchoolAppDbContext>(options => options.UseSqlServer(connString));
            builder.Services.AddRepositories();
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());
            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
