namespace WebApi;

public static partial class OpenApi
{
    public static void AddOpenApi(this IServiceCollection services)
        => services.AddSwaggerGen(c =>
            c.CustomSchemaIds(t => t.IsNested
                ? t.FullName?.Replace(t.Namespace + ".", string.Empty)?.Replace("+", string.Empty)
                : t.Name));
}
