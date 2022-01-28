using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Chat.Contracts.Commands;
using Chat.Contracts.DataTransfer;
using Chat.Contracts.Queries;

namespace Chat
{
    public class ChatHubActor : ReceiveActor
    {
        private readonly HashSet<string> _connections = new HashSet<string>();
        private readonly Queue<ChatMessage> _messages = new Queue<ChatMessage>();

        public ChatHubActor()
        {
            Receive<AddConnection>(command => { _connections.Add(command.ConnectionId); });

            Receive<RemoveConnection>(command => { _connections.Remove(command.ConnectionId); });

            Receive<GetConnectionCount>(command =>
            {
                Context.Sender.Tell(new ConnectionCount()
                {
                    Count = _connections.Count
                });
            });

            Receive<ChatMessage>(command =>
            {
                if (_messages.Count == 50)
                    _messages.Dequeue();

                _messages.Enqueue(command);
            });

            Receive<GetChatArchive>(command =>
            {
                Context.Sender.Tell(new ChatArchive
                {
                    Messages = _messages.ToList()
                });
            });
        }

    }
}