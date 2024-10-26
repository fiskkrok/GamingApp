namespace GamingApp.ApiService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddScoped<ExceptionMiddleware>();
            return services;
        }
    }
}
