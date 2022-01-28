using System;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class Seed
    {
        public Seed(Guid clientGuid)
        {
            ClientGuid = clientGuid;
            ServerGuid = Guid.NewGuid();
        }

        public Guid ClientGuid { get; set; }
        public Guid ServerGuid { get; set; }
        public int Value => ClientGuid.GetHashCode() + ServerGuid.GetHashCode();
    }
}