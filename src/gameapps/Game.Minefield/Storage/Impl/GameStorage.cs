using Game.Minefield.Contracts.Model;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using Shared.Configuration;
using Shared.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Minefield.Storage.Impl
{
    // https://docs.microsoft.com/en-us/azure/storage/storage-dotnet-how-to-use-tables
    public class GameStorage : BaseStorage, IGameStorage
    {
        public override string TableName => "games";

        public GameStorage(AppServerSettings serverSettings) : base(serverSettings)
        {
        }

        public async Task<State> GetAsync(Network network, string userName, string gameId)
        {
            var table = CloudTableClient.GetTableReference(GetTableName(network));
            var retrieveOperation = TableOperation.Retrieve<GameEntity>(userName, gameId);
            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return ((GameEntity) retrievedResult.Result)?.State;
        }
        public async Task<State> GetLastStateAsync(Network network, string userName)
        {
            var query = new TableQuery<GameEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userName))
                .Take(1);
             
            var table = CloudTableClient.GetTableReference(GetTableName(network));
            var resultSet = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            var result = resultSet.FirstOrDefault();

            return result?.State;
        }
        
        public async Task<UserState> GetLastAsync(Network network, string userName)
        {
            var query = new TableQuery<GameEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userName))
                .Take(1);

            var table = CloudTableClient.GetTableReference(GetTableName(network));
            var resultSet = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            var result = resultSet.FirstOrDefault();

            return result?.State.UserState;
        }

        public async Task UpdateAsync(Network network, string userName, string gameId, State state)
        {
            try
            {
                var table = CloudTableClient.GetTableReference(GetTableName(network));
                var entity = new GameEntity(userName, gameId)
                {
                    State = state
                };
                var insertOperation = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to update user state.", ex);
                throw;
            }
            
        }

        public async Task InsertAsync(Network network, string userName, string gameId, State state)
        {
            try
            {
                var table = CloudTableClient.GetTableReference(GetTableName(network));
                var entity = new GameEntity(userName, gameId)
                {
                    State = state
                };

                var insertOperation = TableOperation.Insert(entity);
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to store user state.", ex);
                throw;
            }
        }
    }
}