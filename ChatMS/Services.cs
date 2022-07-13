using Alfa.ChatMS.Business;
using Alfa.ChatMS.Data;
using Alfa.ChatMS.Helper;
using Alfa.ChatMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace Alfa.ChatMS
{
    public static class Services
    {
        public static IServiceCollection ConfigureServicesExt(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                                builder =>
                                {
                                    builder.AllowAnyHeader()
                                           .AllowAnyMethod()
                                           .SetIsOriginAllowed((host) => true)
                                           .AllowCredentials();
                                }));
            services.AddControllers();
            services.AddSignalR();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat-MS", Version = "v1" });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                //////Add Operation Specific Authorization///////
                c.OperationFilter<AuthOperationFilter>();
                ////////////////////////////////////////////////



            });

            services.RegisterDependencies();

            return services;
        }

        private static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddDbContext<ApplicationDbContext>();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.ConfigureIdentity();

            services.AddSingleton<ITokenHelper, TokenHelper>();
            return services.AddTransient<IMessageHandler, MessageHandler>();
        }

        private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                            .AddEntityFrameworkStores<ApplicationDbContext>();
            var secret = GetAppSettings().GetValue<string>("Secret");

            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var usrID = context.Principal.Identity.Name;
                        var user = ValidateUser(userService, usrID);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if ((!path.StartsWithSegments("/chat/negotiate") && (path.StartsWithSegments("/chat"))))
                        {
                            var accessToken = context.Request.Query["access_token"];
                            // Read the token out of the query string
                            if (string.IsNullOrEmpty(accessToken))
                            {
                                context.Fail("Unauthorized");
                            }
                            else
                            {
                                context.Token = accessToken;
                                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                                var usrID = context?.Principal?.Identity?.Name;
                                if (string.IsNullOrEmpty(usrID))
                                {
                                    //context.Fail("Unauthorized");
                                }
                                else
                                {
                                    var user = ValidateUser(userService, usrID);
                                    if (user == null)
                                    {
                                        // return unauthorized if user no longer exists
                                        context.Fail("Unauthorized");
                                    }
                                }
                            }
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();

            return services;
        }

        private static User ValidateUser(IUserService userService, string usrId)
        {

            var userId = int.Parse(usrId);
            var user = userService.GetById(userId);
            return user;
        }

        private static IConfiguration GetAppSettings()
        {
            var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            return configurationBuilder.Build();
        }
    }
}