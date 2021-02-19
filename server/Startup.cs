using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Orchestrate.API.Authorization;
using Orchestrate.API.Data;
using Orchestrate.API.Services;
using Orchestrate.API.Services.Interfaces;
using System.Text;

namespace Orchestrate.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.Configure<AdminOptions>(Configuration);
            services.Configure<JwtOptions>(Configuration);
            services.Configure<PasswordHashOptions>(Configuration);

            services.AddDbContext<OrchestrateContext>(builder => builder.UseNpgsql(Configuration.GetConnectionString("OrchestrateDb")));
            services.AddScoped<OrchestrateDbInitializer>();

            services.AddAuthentication(c =>
            {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(c =>
            {
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSecret")))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(GroupRolesPolicy.AdministratorOnly, policy => policy.RequireRole(Configuration.GetValue<string>("AdminRoleName")));

                options.AddPolicy(GroupRolesPolicy.MemberOnly, policy => policy.AddRequirements(new GroupRolesRequirement(GroupRoles.Member)));
                options.AddPolicy(GroupRolesPolicy.DirectorOnly, policy => policy.AddRequirements(new GroupRolesRequirement(GroupRoles.Director)));
                options.AddPolicy(GroupRolesPolicy.DirectorOrPlayer, policy => policy.AddRequirements(new GroupRolesRequirement(GroupRoles.Director | GroupRoles.Member)));
                options.AddPolicy(GroupRolesPolicy.ManagerOnly, policy => policy.AddRequirements(new GroupRolesRequirement(GroupRoles.Manager)));
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, GroupAuthorizationHandler>();

            services.AddMvc(o => o.EnableEndpointRouting = false);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orchestrate.API", Version = "v1" });
            });

            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordProvider, PasswordProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orchestrate.API v1"));
            }

            app.UseExceptionHandler("/api/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc();
        }
    }
}
