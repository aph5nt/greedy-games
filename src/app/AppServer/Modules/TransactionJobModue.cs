using Autofac;
using Microsoft.EntityFrameworkCore;
using Payment.Actors.Jobs;
using Persistance;
using Persistance.Repositories.Impl;

namespace AppServer.Modules
{
    public class TransactionJobModue : Module
    {
        private readonly string _connectionString;

        public TransactionJobModue(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(_connectionString)
                .Options)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DataContext>().AsSelf().InstancePerDependency();
            builder.RegisterType<WithdrawRepository>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<DepositRepository>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AccountRepository>().AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<DepositActor>()
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterType<DepositConfirmationActor>()
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterType<DividendActor>()
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterType<DividendConfirmationActor>()
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterType<UserWithdrawConfirmationActor>()
                .PropertiesAutowired()
                .AsSelf();

            base.Load(builder);
        }
    }
}