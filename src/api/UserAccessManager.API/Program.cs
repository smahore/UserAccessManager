using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using UserAccessManager.API.Middleware;
using UserAccessManager.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting UserAccessManager API");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services));

    // Controllers
    builder.Services.AddControllers();

    // Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "UserAccessManager API",
            Version = "v1",
            Description = "Backend API for the User Access Management system"
        });
    });

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<UserAccessManager.Core.Validators.CreateUserRequestValidator>();

    // Infrastructure (Dapper + Repositories)
    builder.Services.AddInfrastructure(builder.Configuration);

    // CORS
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader());
    });

    var app = builder.Build();

    // Global exception handler
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAccessManager API v1"));
    }

    app.UseSerilogRequestLogging();
    app.UseCors("ApiCorsPolicy");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
