using System;

namespace Akka.Quartz.Actor.Exceptions
{
    public class JobNotFoundException: Exception
    {
        public JobNotFoundException() : base("job not found")
        {            
        }
    }
}
