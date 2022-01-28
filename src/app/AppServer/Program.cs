using System;
using System.Threading;

namespace AppServer
{
    class Program
    {
        static readonly AppService AppService = new AppService();
        static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
        
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (ConsoleCancelEventHandler) ((o, e) =>
            {
                AppService.Stop();
                WaitHandle.Set();
            });
            
            AppService.Start();
            WaitHandle.WaitOne();
        }
    }
}
