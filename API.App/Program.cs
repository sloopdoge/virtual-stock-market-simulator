using API.App.SwaggerConfig;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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

            #region Services

            builder.Services.AddAuthorization();

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
            
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"/var/www/dataprotection-keys"))
                .SetApplicationName("api.vsms");
            builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);
            builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(option =>
                        {
                            option.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        }
                    );
                }
            );

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
                // endpoints.MapHub<WebUIHub>("/WebUIHub", options =>
                // {
                //     options.TransportMaxBufferSize = 456000;
                //     options.ApplicationMaxBufferSize = 456000;
                //     options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(15);
                //     options.LongPolling.PollTimeout = TimeSpan.FromSeconds(15);
                // });
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