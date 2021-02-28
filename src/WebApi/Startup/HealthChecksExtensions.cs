using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi
{
    public static class HealthChecksExtensions
    {
        public static void AddConnectHealthChecks(this IServiceCollection serviceCollection)
            => serviceCollection.AddHealthChecks();

        public static void MapHealthChecks(this IEndpointRouteBuilder endpoints)
            => endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = WriteResponse,
            });

        private static async Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            using var memoryStream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true });

            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", result.Status.ToString());
            jsonWriter.WriteStartArray("results");

            foreach (var entry in result.Entries)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteStartObject(entry.Key);
                jsonWriter.WriteString("status", entry.Value.Status.ToString());
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
            await jsonWriter.FlushAsync();

            var json = Encoding.UTF8.GetString(memoryStream.ToArray());
            await context.Response.WriteAsync(json);
        }
    }
}
