using BarberBoss.Application.Services.Mappings;
using BarberBoss.Application.UseCases.Billings.Register;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        AddUseCases(services);
        AddMapperConfigurations();
    }

    public static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterBillingUseCase, RegisterBillingUseCase>();
    }

    public static void AddMapperConfigurations()
    {
        MapConfigurations.Configure();
    }
}
