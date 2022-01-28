using System;

namespace Payment.Messages.Events.Waves
{
    public abstract class WavesEvent
    {
        public Object Payload { get;  }

        public WavesEvent(Object payload)
        {
            Payload = payload;
        }
    }
}