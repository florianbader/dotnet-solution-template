namespace WebApi;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseProblemDetails();

        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }
        else
        {
            app.MigrateDatabase();
        }

        app.UseSerilog();

        app.UseAuthentication();

        app.UseOpenApi();

        app.UseRouting();

        app.UseCustomCors();

        // app UseAuthorization
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (env.IsProduction())
            {
                endpoints.MapHealthChecks();
            }
        });

        app.UseResponseCaching();

        app.UseResponseCompression();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddCors();

        services.AddOptions();

        services.AddHealthChecks();

        services.AddOpenApi();

        services.AddHttpClientServices();

        services.AddEntityFramework(Configuration);

        services.AddMapper();

        services.AddMediator();

        services.AddProblemDetails();

        services.AddApplicationInsightsTelemetry();

        services.AddResponseCaching();

        services.AddResponseCompression();
    }
}
