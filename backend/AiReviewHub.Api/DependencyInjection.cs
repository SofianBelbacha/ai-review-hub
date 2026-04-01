using AiReviewHub.Api.Services;
using AiReviewHub.Application;
using AiReviewHub.Application.Abstractions;
using AiReviewHub.Infrastructure;

namespace AiReviewHub.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();


            return services;
        }

    }
}
