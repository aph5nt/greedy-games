using Autofac;
using Game.Minefield.Actors;
using Game.Minefield.Services;
using Game.Minefield.Storage.Impl;

namespace AppServer.Modules
{
    public class MinefielGameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GameStorage>().PropertiesAutowired().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LogStorage>().PropertiesAutowired().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Persistance.Repositories.Impl.GameStatisticRepository>().PropertiesAutowired().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CreateStateStrategy>().AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<GameManagerActor>()
                .PropertiesAutowired()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<GameActor>()
                .PropertiesAutowired()
                .AsSelf()
                .InstancePerDependency();

            base.Load(builder);
        }
    }
}