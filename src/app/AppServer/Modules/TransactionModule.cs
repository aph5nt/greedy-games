using Autofac;
using AutoMapper;
using Payment.Actors;
using Payment.Actors.Jobs;
using Payment.Contracts.DataTransfer;
using Payment.Providers;
using Payment.Services;
using Payment.Services.Impl;
using Persistance.Model.Payments;

namespace AppServer.Modules
{
    public class TransactionModule : Module
    {
        private readonly string _connectionString;
        private readonly IWavesApiFactory _wavesApiFactory;

        public TransactionModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TransactionModule(string connectionString, IWavesApiFactory wavesApiFactory) : this(connectionString)
        {
            _wavesApiFactory = wavesApiFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var mapper = new Mapper(new MapperConfiguration(mapping =>
            {
                mapping.CreateMap<DepositActor, DepositDto>();
                mapping.CreateMap<DividendWithdraw, DividendWithdrawDto>();
                mapping.CreateMap<UserWithdraw, UserWithdrawDto>();
            }));

            builder.RegisterInstance(mapper).AsImplementedInterfaces();

            builder.RegisterType<TransactionManagerActor>()
                .WithParameter("connectionString", _connectionString)
                .AsSelf()
                .SingleInstance();

            if (_wavesApiFactory == null)
            {
                builder.RegisterType<WavesApiFactory>()
                    .AsImplementedInterfaces()
                    .PropertiesAutowired()
                    .InstancePerDependency();
            }
            else
            {
                builder.RegisterInstance(_wavesApiFactory)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.RegisterType<WavesActorProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TransactionActorProvider>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<WavesGatewayActor>()
                .PropertiesAutowired()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<UserWithdrawActor>()
                .PropertiesAutowired()
                .AsSelf();

            base.Load(builder);
        }
    }
}