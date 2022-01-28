namespace Shared.Model
{
    public class Identity
    {
        public Identity()
        {
        }

        public Identity(Network network, string userName)
        {
            Network = network;
            UserName = userName;
        }

        public Identity(long id, Network network, string userName) : this(network, userName)
        {
            Id = id;
        }

        public long Id { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
    }
}