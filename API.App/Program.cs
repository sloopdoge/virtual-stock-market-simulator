using System.Text;
using API.App.SwaggerConfig;
using API.Identity.Entities;
using API.Identity.Interfaces;
using API.Identity.Repositories;
using API.Identity.Services;
using API.Identity.Structures;
using API.Infrastructure.Hubs;
using API.Infrastructure.Hubs.Interfaces;
using API.Infrastructure.Hubs.Services;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Services;
using API.Infrastructure.Services.Algorithms;
using API.Infrastructure.Services.Background;
using API.Infrastructure.Utils;
using API.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace API.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Serilog Logger

        var logsConnectionString = builder.Configuration.GetConnectionString("LogsConnection");

        if (builder.Environment.IsDevelopment())
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
        }

        if (builder.Environment.IsProduction())
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.MSSqlServer(
                    connectionString: logsConnectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    },
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                .CreateLogger();
        }

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();

        #endregion

        Log.Warning("Starting web host");

        try
        {
            #region Identity

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                        ValidAudience = jwtSettings.GetValue<string>("Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
    
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            Log.Information("Access Token: {accessToken}", accessToken);
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error("Authentication failed: {Exception}", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Log.Information("Token validated successfully for user: {ConnectionId}", context.HttpContext.Connection.Id);
                            return Task.CompletedTask;
                        },
                    };
                    
                    options.IncludeErrorDetails = true;
                });

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPermissions", policy =>
                {
                    policy.RequireRole(RoleNames.Admin);
                });
            });

            #endregion

            #region Services
            
            builder.Services.AddLogging();
            
            builder.Services.AddHostedService<DatabaseInitializer>();
            builder.Services.AddHostedService<StockMarketBackgroundService>();
            
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IStockService, StockService>();
            
            builder.Services.AddTransient<RandomWalkWithDriftAlgorithm>();
            builder.Services.AddTransient<MeanReversionAlgorithm>();
            builder.Services.AddTransient<MomentumAlgorithm>();
            builder.Services.AddTransient<ExponentialMovingAverageAlgorithm>();
            
            builder.Services.AddScoped<IAlgorithmFactory, AlgorithmFactory>();
            
            builder.Services.AddSingleton<StocksHub>();
            
            builder.Services.AddScoped<IStocksRealTimeHub, StocksRealTimeHub>();

            #endregion

            #region DataBase

            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(defaultConnectionString));

            builder.Services.AddScoped<CompanyDbRepository>();
            builder.Services.AddScoped<StockDbRepository>();
            builder.Services.AddScoped<UserProfilesDbRepository>();

            #endregion
            
            #region Swagger/OpenAPI
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API - Virtual Stock Market Simulator", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                
                c.OperationFilter<SwaggerFileOperationFilter>();
            });
            
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        }
                    );
                }
            );
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSignalR();
            
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"/var/www/dataprotection-keys"))
                .SetApplicationName("api.vsms");
            builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            
            app.UseCors();
            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Authentication started for request: {Path}", context.Request.Path);

                await next();

                logger.LogInformation("Authentication completed for request: {Path}", context.Request.Path);
            });
            app.UseAuthorization();


#pragma warning disable ASP0014
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<StocksHub>("/StocksHub", options =>
                {
                    options.TransportMaxBufferSize = 456000;
                    options.ApplicationMaxBufferSize = 456000;
                    options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(15);
                    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(15);
                });
            });
#pragma warning restore ASP0014

            app.Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, e.Message);
        }
        finally
        {
            Log.Warning("Web host shutdown");
            Log.CloseAndFlush();
        }
    }
}