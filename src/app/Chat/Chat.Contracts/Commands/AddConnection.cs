namespace Chat.Contracts.Commands
{
    public class AddConnection
    {
        public AddConnection(string connectionId)
        {
            ConnectionId = connectionId;
        }
        
        public string ConnectionId { get; }
    }
}