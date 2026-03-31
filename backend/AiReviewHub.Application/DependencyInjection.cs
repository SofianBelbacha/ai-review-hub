using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services) 
        {
            // MediatR handlers
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
            });

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
