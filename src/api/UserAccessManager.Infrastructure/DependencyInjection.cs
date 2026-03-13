using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;
using UserAccessManager.Infrastructure.Repositories;

namespace UserAccessManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UserManagement")
            ?? throw new InvalidOperationException("Connection string 'UserManagement' not found.");

        services.AddSingleton(new DapperContext(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStagingUserRepository, StagingUserRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IAccessRequestRepository, AccessRequestRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        return services;
    }
}
