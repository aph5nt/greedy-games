using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using Quartz;

namespace Akka.Quartz.Actor
{
    [DisallowConcurrentExecution]
    public class QuartzJob : IJob
    {
        private const string MessageKey = "message";
        private const string ActorKey = "actor";

        public async Task Execute(IJobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            if (jdm.ContainsKey(MessageKey) && jdm.ContainsKey(ActorKey))
            {
                var actor = jdm[ActorKey] as IActorRef;
                actor?.Tell(jdm[MessageKey]);
            }

            await Task.CompletedTask;
        }

        public static JobBuilder CreateBuilderWithData(IActorRef actorRef, object message)
        {
            var jdm = new JobDataMap();
            jdm.AddAndReturn(MessageKey, message).Add(ActorKey, actorRef);
            return JobBuilder.Create<QuartzJob>().UsingJobData(jdm);
        }
 
    }
}