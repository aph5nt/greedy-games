using Autofac;
using Chat;

namespace AppServer.Modules
{
    public class ChatModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ChatHubActor>()
                .AsSelf()
                .SingleInstance();

            base.Load(builder);
        }
    }
}

 