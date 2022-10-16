namespace WebApi;

public static class HttpServices
{
    public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        => services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddHttpClient();
}
