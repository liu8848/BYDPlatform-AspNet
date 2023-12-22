using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime LifeTime { get; set; } = ServiceLifetime.Transient;
}