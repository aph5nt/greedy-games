using Autofac;
using GreedyGames.Domain;
using GreedyGames.Game.Minefield.Commands;
using GreedyGames.Game.Minefield.Storage.Impl;
using GreedyGames.Services.Various;

namespace Game.MineField.ProofOfProfitability
{
    public class ProofModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppDataContext>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            var assembly = typeof(Move).Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Handler"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Strategy"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(typeof(IBalanceService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Storage"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterType<EmptyBalanceUpdateDispatcher>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            new GameStorage().Init();
            new LogStorage().Init();
            new StatisticStorage().Init();
        }
    }
}