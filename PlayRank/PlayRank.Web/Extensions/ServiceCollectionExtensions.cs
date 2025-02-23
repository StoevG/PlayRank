using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlayRank.Application.Core.Interfaces.Abstract;
using PlayRank.Application.Core.Matches.MapperProfiles;
using PlayRank.Application.Core.Services;
using PlayRank.Data;
using PlayRank.Data.Repositories.Abstract;
using PlayRank.Domain.Interfaces.Abstract;

namespace PlayRank.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlayRankContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services.AddScopedServiceTypes(typeof(BaseRepository).Assembly, typeof(IRepository));
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            return services.AddScopedServiceTypes(typeof(MatchService).Assembly, typeof(IService));
        }

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(MatchProfile));
        }

        public static IServiceCollection AddCustomSwaggerExtension(this IServiceCollection services)
        {
            var xmlDocsPath = Path.Combine(AppContext.BaseDirectory, typeof(Program).Assembly.GetName().Name + ".xml");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play Rank API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            return services;
        }

        private static IServiceCollection AddScopedServiceTypes(this IServiceCollection services, Assembly assembly, Type fromType)
        {
            var types = assembly.GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.IsClass && !x.IsAbstract && fromType.IsAssignableFrom(x))
                .Select(x => new
                {
                    Interface = x.GetInterface($"I{x.Name}"),
                    Implementation = x
                });

            foreach (var type in types)
            {
                services.AddScoped(type.Interface, type.Implementation);
            }

            return services;
        }
    }
}
