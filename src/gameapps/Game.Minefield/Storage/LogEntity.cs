using System;
using Game.Minefield.Contracts.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace Game.Minefield.Storage
{
    public class LogEntity : TableEntity
    {
        public LogEntity(string userName, string gameId)
        {
            PartitionKey = userName;
            RowKey = gameId;
            CreatedAt = DateTime.UtcNow;
        }

        public LogEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public string ServerSeed { get; set; }
        public string ClientSeed { get; set; }
        public string Value { get; set; }
        public UserState UserState { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class GameLog
    {
        public string UserName { get; set; }
        public string GameId { get; set; }
        public string ServerSeed { get; set; }
        public string ClientSeed { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }

       

        public static GameLog Map(LogEntity entity)
        {
            return new GameLog
            {
                ClientSeed = entity.ClientSeed,
                GameId = entity.RowKey,
                ServerSeed = entity.ServerSeed,
                UserName = entity.PartitionKey,
                Value = entity.Value,
                CreatedAt = entity.CreatedAt
            };
        }

        public UserState UserState { get; set; }
    }
}