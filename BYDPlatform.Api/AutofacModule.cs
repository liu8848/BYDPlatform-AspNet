using Autofac;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Infrastructure.Repositories;

namespace BYDPlatform.Api;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(IRepository<>))
            .As(typeof(RepositoryBase<>))
            .InstancePerLifetimeScope();
    }
}