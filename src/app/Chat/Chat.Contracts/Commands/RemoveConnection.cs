namespace Chat.Contracts.Commands
{
    public class RemoveConnection
    {
        public RemoveConnection(string connectionId)
        {

            ConnectionId = connectionId;
        }
        
        public string ConnectionId { get;}
    }
}