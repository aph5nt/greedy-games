using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using Quartz;
using Akka.Serialization;

namespace Akka.Quartz.Actor
{
    /// <summary>
    /// Persistent Job
    /// </summary>
    public class QuartzPersistentJob : IJob
    {
        private const string MessageKey = "message";
        private const string ActorKey = "actor";
        public const string SysKey = "sys";

        public async Task Execute(IJobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            if (jdm.ContainsKey(MessageKey) && jdm.ContainsKey(ActorKey))
            {
                var actorPath = jdm[ActorKey] as string;
                var sys = context.Scheduler.Context[SysKey] as ActorSystem;

                if (actorPath != null && sys != null)
                {
                    ActorSelection selection = sys.ActorSelection(actorPath);
                    byte[] messageBytes = jdm[MessageKey] as byte[];
                    var message = sys.Serialization.FindSerializerForType(typeof(object)).FromBinary(messageBytes, typeof(object));
                    selection.Tell(message);
                }
            }

            await Task.CompletedTask;
        }

        public static JobBuilder CreateBuilderWithData(ActorPath actorPath, object message, ActorSystem system)
        {
            Serializer messageSerializer = system.Serialization.FindSerializerFor(message);
            var serializedMessage = messageSerializer.ToBinary(message);
            var serializedPath = actorPath.ToSerializationFormat();
            var jdm = new JobDataMap();
            jdm.AddAndReturn(MessageKey, serializedMessage).Add(ActorKey, serializedPath);
            return JobBuilder.Create<QuartzPersistentJob>().UsingJobData(jdm);
        }
    }
}