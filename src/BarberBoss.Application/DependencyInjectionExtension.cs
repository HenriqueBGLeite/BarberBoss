using BarberBoss.Application.Services.Mappings;
using BarberBoss.Application.UseCases.Billings.Delete;
using BarberBoss.Application.UseCases.Billings.GetAll;
using BarberBoss.Application.UseCases.Billings.GetById;
using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Application.UseCases.Billings.Update;
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
        services.AddScoped<IGetAllBillingUseCase, GetAllBillingUseCase>();
        services.AddScoped<IGetBillingByIdUseCase, GetBillingByIdUseCase>();
        services.AddScoped<IUpdateBillingUseCase, UpdateBillingUseCase>();
        services.AddScoped<IDeleteBillingUseCase, DeleteBillingUseCase>();
    }

    public static void AddMapperConfigurations()
    {
        MapConfigurations.Configure();
    }
}
