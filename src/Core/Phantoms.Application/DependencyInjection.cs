using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Phantoms.Application.Common.Behaviours;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Events.Services;
using Phantoms.Application.Announcements.Services;
using System.Reflection;

namespace Phantoms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(assembly);

        // Auto-discover and register all AbstractValidator<T> implementations
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IEventServices, EventServices>();
        services.AddScoped<IAnnouncementServices, AnnouncementServices>();

        // Wire the validation pipeline: every MediatR request goes through validation first
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

