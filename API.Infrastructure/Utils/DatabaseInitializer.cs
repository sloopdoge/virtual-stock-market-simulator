using API.Identity.Entities;
using API.Identity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Utils;

public class DatabaseInitializer(ILogger<DatabaseInitializer> logger, IServiceProvider serviceProvider)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await InitializeDefaultRoles();
            await InitializeDefaultUsers();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            await StartAsync(cancellationToken);
        }
    }

    private async Task InitializeDefaultRoles()
    {
        try
        {
            logger.LogInformation("--- Start initializing default roles ---");
            
            List<string> defaultRoles =
            [
                Roles.Guest.ToString(),
                Roles.User.ToString(),
                Roles.Admin.ToString(),
                Roles.ApiUser.ToString(),
            ];
            
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var existingRoles = (await roleManager.Roles.ToListAsync()).Select(r => r.Name).ToList();
            defaultRoles = defaultRoles.Where(d => !existingRoles.Contains(d)).ToList();

            if (defaultRoles.Any())
            {
                logger.LogInformation($"Need to initialize {defaultRoles.Count} roles");

                if (defaultRoles.Count != 0)
                    foreach (var role in defaultRoles)
                    {
                        await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    }
            }
            
            logger.LogInformation("--- Finished initializing default roles ---");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
    
    private async Task InitializeDefaultUsers()
    {
        try
        {
            logger.LogInformation("--- Start initializing default user ---");
            
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string defaultPassword = "Password1!";
            var defaultUser = new ApplicationUser()
            {
                Id = Guid.Parse("0656AE1D-2662-48CE-A3EF-693C3BD3CCB5"),
                FirstName = $"{Roles.Admin.ToString()}",
                LastName = $"{Roles.Admin.ToString()}",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                Email = $"{Roles.Admin.ToString().ToLower()}@{Roles.Admin.ToString().ToLower()}.com",
                UserName = $"{Roles.Admin.ToString().ToLower()}",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            
            var defaultUserExist = await userManager.FindByIdAsync(defaultUser.Id.ToString()) is not null;

            if (!defaultUserExist)
            {
                logger.LogInformation($"Need to initialize ADMIN user");

                await userManager.CreateAsync(defaultUser, defaultPassword);
                
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
            }
            
            logger.LogInformation("--- Finished initializing default user ---");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}