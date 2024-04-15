using Dysnomia.CoursFrontM1.GamesDb.Business.Implementations;
using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;
using Dysnomia.CoursFrontM1.GamesDb.DataAccess;
using Dysnomia.CoursFrontM1.GamesDb.DataAccess.Implementations;
using Dysnomia.CoursFrontM1.GamesDb.DataAccess.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.WebAPI.HealthCheck;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using NLog;
using NLog.Web;

using System.Text;
using System.Text.Json.Serialization;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI {
    public class Program {
        private const string CORS_POLICY = "CORS_POLICY";
        protected Program() { }
        public static void Main(string[] args) {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("Starting Program");

            try {
                var builder = WebApplication.CreateBuilder(args);

                var rawConfig = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets<Program>()
                    .Build();

                var appSettingsSection = rawConfig.GetSection("AppSettings");
                builder.Services.Configure<AppSettings>(appSettingsSection);

                // NLog: Setup NLog for Dependency injection
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // Context
                builder.Services.AddTransient<DatabaseContext>();

                // DataAccess
                builder.Services.AddTransient<IUserDataAccess, UserDataAccess>();

                // Services
                builder.Services.AddTransient<IGameService, GameService>();
                builder.Services.AddTransient<IUserService, UserService>();

                builder.Services.AddControllers().AddJsonOptions(opt => {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = rawConfig["AppSettings:JwtIssuer"],
                        ValidAudience = rawConfig["AppSettings:JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawConfig["AppSettings:JwtKey"] ?? throw new InvalidDataException("Invalid config: IssuerKey")))
                    };
                });

                builder.Services.AddCors(options => {
                    options.AddPolicy(name: CORS_POLICY,
                                      policy => {
                                          policy.AllowAnyOrigin()
                                                .AllowAnyMethod()
                                                .AllowAnyHeader();
                                      });
                });

                builder.Services.AddAuthorizationBuilder()
                    .AddPolicy(AuthPolicies.JWT_POLICY, policy => policy.RequireAuthenticatedUser()
);

                builder.Services.AddHealthChecks()
                    .AddCheck<DbHealthCheck>("Database");

                var app = builder.Build();

                using (var scope = app.Services.CreateScope()) {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    // Here is the migration executed
                    dbContext.Database.Migrate();
                }

                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();

                app.UseCors(CORS_POLICY);

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();
                app.MapHealthChecks("/health");

                app.Run();
            } catch (Exception exception) {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            } finally {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}
