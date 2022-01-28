using System;
using System.Net;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR.Client;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payment.Helpers;
using Shared.Configuration;
using Logging = Akka.Event.Logging;

namespace Payment.Actors
{
    public class HubNotificationActor<TMessage> : ReceiveActor where TMessage : class
    {
        private readonly AppServerSettings _settings;
        private readonly string _hubName;
        private HubConnection _hubConnection;
        private ICancelable _scheduler;
        private IActorRef _selfReference;
        
        public HubNotificationActor(AppServerSettings settings, string hubName)
        {
            _settings = settings;
            _hubName = hubName;
        }

        protected override void PreStart()
        {
            Become(UnConnected);

            _selfReference = Context.Self;
        }

        private ICancelable StartScheduler()
        {
            return Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 10000, Context.Self, NotificationMessages.Connect, Context.Self);
        }
        
        private void UnConnected()
        {
            _scheduler = StartScheduler();

            Receive<string>(command =>
            {
                Become(Connected);
            }, s => s == NotificationMessages.Connected);
            
            Receive<string>(s =>
            {
                var tokenResponse = TokenHelper.GetToken(_settings);
                if (tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    var token = JsonConvert.DeserializeObject<dynamic>(tokenResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                        .token.ToString();
                    /*
                    _hubConnection = new HubConnectionBuilder()
                        .WithTransport(TransportType.WebSockets)
                        .WithUrl(new Uri($"{_settings.Notifications.Url}/{_hubName}?hubToken={token}"))
                        .WithConsoleLogger(LogLevel.Debug)
                        .Build();

                    _hubConnection.Closed += async exception => _selfReference.Tell(NotificationMessages.Closed);
                    _hubConnection.Connected += async () => _selfReference.Tell(NotificationMessages.Connected);
                    */
                    try
                    {
                        _hubConnection.StartAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        Logging.GetLogger(Context).Warning($"Connection with {_hubName} hub failed.", ex);
                    }
                }
            }, s => s == NotificationMessages.Connect);

            Receive<string>(any =>
            {
                Logging.GetLogger(Context).Warning($"Received message not supported by {nameof(UnConnected)} state: " + any);
            });
        }

        private void Connected()
        {
            _scheduler.Cancel();
            
            Receive<string>(command => { Become(UnConnected); }, s => s == NotificationMessages.Closed);
            
            Receive<TMessage>(message => { _hubConnection.SendAsync("Notify", message).Wait(); });
            
            Receive<string>(any =>
            {
                Logging.GetLogger(Context).Warning($"Received message not supported by {nameof(Connected)} state: " + any);
            });
        }
    }
}