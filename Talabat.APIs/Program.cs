using Talabat.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Repository;
using Talabat.Repository;
using Talabat.APIs.Helpers;
using Talabat.APIs.Controllers;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Error;
using Talabat.APIs.Middlewares;
using Talabat.APIs.Extentions;
using StackExchange.Redis;
using Talabat.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          

            #region configure Services works with DI

          
            // Add services to the container.

            builder.Services.AddControllers();
            //////////////////////
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            //

            //IConnectionMultiplexer
            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            //
            //
            builder.Services.addApplicationServices();
            //
            #region Identity
            //
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                //options.Password.RequiredUniqueChars = 2;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();
            //


            builder.Services.AddScoped<ITokenService, TokenService>();

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer();

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                         ValidateAudience = true,
                         ValidAudience = builder.Configuration["JWT:ValidAudience"],
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                     };
                 });

            //Add Cors

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyHeader();
                    config.AllowAnyMethod();
                    config.WithOrigins(builder.Configuration["FrontEndBaseUrl"]);
                });
            });
            //
            #endregion

            ///////////////////////AddSwagger
            builder.Services.AddSwaggerServices();
            #endregion
            var app = builder.Build();


            #region Ubdate DataBase
            using var scope = app.Services.CreateScope();
            var Services = scope.ServiceProvider; //DI
            var dbContext = Services.GetRequiredService<StoreContext>();

            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
            try
            {
               // await dbContext.Database.MigrateAsync();
                await dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(dbContext);
                ////////////////////////////////////////////
                await IdentityDbContext.Database.MigrateAsync();
                //
                var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUsersAsync(UserManager);
            }
            catch (Exception ex)
            {
                var Logger = loggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "an error has been occured during apply the migration");
            }

            #endregion

            #region  Configure Request into piplines (middleWare)
            //1-exception middlware
            app.UseMiddleware<ExceptionMiddleware>();
            //or
            //app.Use(async (httpContext, next) =>
            //{
            //    try , catch اللي انا عملتهم
            //});
            //

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.AddSwaggerServices();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //not found end point

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            //

            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            // ImageURL
            //

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}