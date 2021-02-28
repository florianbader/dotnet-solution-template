using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
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

            app.UseSerilog();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

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

            services.AddHttpClientServices();

            services.AddProblemDetails(ProblemDetailsConfiguration.Configure);

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddResponseCaching();

            services.AddResponseCompression();
        }
    }
}
