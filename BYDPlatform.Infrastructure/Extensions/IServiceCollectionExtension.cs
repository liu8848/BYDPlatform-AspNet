using System.Reflection;
using BYDPlatform.Application.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BYDPlatform.Infrastructure.Extensions;


/// <summary>
/// 批量注入Service扩展方法
/// </summary>
public static class IServiceCollectionExtension
{
    public static void BatchRegisterService(this IServiceCollection services)
    {
        Serilog.Log.Information("===================开始注入Services===================");
        var allAssembly = GetAllAssembly();
        
        services.RegisterServiceByAttribute(ServiceLifetime.Singleton, allAssembly);
        services.RegisterServiceByAttribute(ServiceLifetime.Scoped, allAssembly);
        services.RegisterServiceByAttribute(ServiceLifetime.Transient, allAssembly);

        services.RegisterBackgroundService(allAssembly);
    }

    /// <summary>
    /// 通过Attribute注解批量注入Service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceLifetime"></param>
    private static void RegisterServiceByAttribute(this IServiceCollection services,
        ServiceLifetime serviceLifetime,List<Assembly> allAssembly)
    {
        List<Type> types = allAssembly.SelectMany(
                t => t.GetTypes())
            .Where(t =>
                t.GetCustomAttributes(typeof(ServiceAttribute), false).Length > 0
                && t.GetCustomAttribute<ServiceAttribute>()?.LifeTime == serviceLifetime
                &&t.IsClass&&!t.IsAbstract).ToList();

        foreach (var type in types)
        {
            Type? typeInterface = type.GetInterfaces().FirstOrDefault();
            if (typeInterface == null)
            {
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(type);
                        break;
                }
            }
            else
            {
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(typeInterface, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(typeInterface,type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(typeInterface,type);
                        break;
                }
            }
        }
    }

    private static void RegisterBackgroundService(this IServiceCollection services, List<Assembly> allAssembly)
    {
        List<Type> types = allAssembly.SelectMany(t => t.GetTypes())
            .Where(t => typeof(BackgroundService).IsAssignableFrom(t)
                        && t.IsClass && !t.IsAbstract).ToList();
        foreach (var type in types)
        {
            services.AddSingleton(typeof(IHostedService), type);
        }
    }


    /// <summary>
    /// 获取全部Assembly
    /// </summary>
    /// <returns></returns>
    private static List<Assembly> GetAllAssembly()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        HashSet<string> loadedAssemblies = new();

        foreach (var item in allAssemblies)
        {
            loadedAssemblies.Add(item.FullName!);
        }

        Queue<Assembly> assembliesToCheck = new();
        assembliesToCheck.Enqueue(Assembly.GetEntryAssembly()!);

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();
            foreach (var reference in assemblyToCheck!.GetReferencedAssemblies())
            {
                if (!loadedAssemblies.Contains(reference.FullName))
                {
                    var assembly = Assembly.Load(reference);
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    allAssemblies.Add(assembly);
                }
            }
        }

        return allAssemblies;
    }


}