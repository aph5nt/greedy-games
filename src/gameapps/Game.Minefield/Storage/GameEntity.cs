using Game.Minefield.Contracts.Model;
using Game.Minefield.Storage.Impl;
using Microsoft.WindowsAzure.Storage.Table;

namespace Game.Minefield.Storage
{
    public class GameEntity : TableEntity
    {
        public GameEntity(string userName, string gameId)
        {
            PartitionKey = userName;
            RowKey = gameId;
        }

        public GameEntity()
        {
        }

        public string Data { get; set; }

        public State State
        {
            get => GameStateSerializer.Deserialize(Data);
            set => Data = GameStateSerializer.Serialize(value);
        }
    }
}