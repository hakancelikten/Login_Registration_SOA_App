using Business.Abstract;
using Core.DependencyResolvers;
using Core.Entities;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using DataAccess.Concrete.EntityFramework;
using Entities.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE Users");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE OperationClaims");
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE UserOperationClaims");

                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash("123", out passwordHash, out passwordSalt);
                var user = new User
                {
                    Email = "admin@admin.com",
                    FirstName = "admin",
                    LastName = "admin",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                };
                var userEntity = context.Entry(user);
                userEntity.State = EntityState.Added;

                var operationClaimList = new List<OperationClaim>();
                operationClaimList.Add(new OperationClaim() { Name = "Admin" });
                operationClaimList.Add(new OperationClaim() { Name = "Manager" });
                operationClaimList.Add(new OperationClaim() { Name = "Admin" });


                foreach (var item in operationClaimList)
                {
                    var operationClaimEntity = context.Entry(item);
                    operationClaimEntity.State = EntityState.Added;
                }

                context.SaveChanges();

                var adminOperationClaim = context.OperationClaims.FirstOrDefault(x => x.Name == "Admin");
                var adminUser = context.Users.FirstOrDefault(x => x.Email == "admin@admin.com");

                var userOperationClaim = new UserOperationClaim { OperationClaimId = adminOperationClaim.Id, UserId = adminUser.Id };
                var userOperationClaimEntity = context.Entry(userOperationClaim);
                userOperationClaimEntity.State = EntityState.Added;

                context.SaveChanges();

            }

            services.AddControllersWithViews();

            services.AddControllers();

            services.AddCors();

            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

            services.AddDependencyResolvers(new ICoreModule[] {
               new CoreModule()
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.ConfigureCustomExceptionMiddleware();

            app.UseCors(builder => builder.WithOrigins("http://localhost:5000").AllowAnyHeader());

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Register}/{action=Index}/{id?}");

            });

        }
    }
}
