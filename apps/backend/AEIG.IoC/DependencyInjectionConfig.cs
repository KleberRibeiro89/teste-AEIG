using AEIG.Domain.AppService.Interfaces;
using AEIG.Domain.AppService.Mappings;
using AEIG.Domain.AppService.Services;
using AEIG.Domain.AppService.Validators;
using AEIG.Domain.Data;
using AEIG.Domain.Data.Repositories;
using AEIG.Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AEIG.IoC;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AEIGDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAssociadoRepository, AssociadoRepository>();
        services.AddScoped<IAssociadoService, AssociadoService>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<ICobrancaRepository, CobrancaRepository>();
        services.AddScoped<ICobrancaService, CobrancaService>();
        services.AddScoped<IParametroPagamentoRepository, ParametroPagamentoRepository>();
        services.AddScoped<IParametroPagamentoService, ParametroPagamentoService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddAutoMapper(cfg => { }, typeof(RequestMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<UserValidator>();

        return services;
    }
}
