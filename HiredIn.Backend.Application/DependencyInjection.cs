using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Application.Services.Notification;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HiredIn.Backend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }
    }
}
