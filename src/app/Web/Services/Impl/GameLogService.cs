using Game.Minefield.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Shared.Model;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;

namespace Web.Services.Impl
{
    public class GameLogService : IGameLogService
    {
        private readonly CloudTableClient _cloudTableClient;

        public GameLogService(WebSettings settings)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(settings.Storage);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }
        public async Task<LogDto> GetAsync(Network network, string userName, string gameId)
        {
            var query = new TableQuery<LogEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userName),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId)))
                .Take(1);

            var logTable = _cloudTableClient.GetTableReference(GetTableName("log", network));
            var log = await logTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            var gameTable = _cloudTableClient.GetTableReference(GetTableName("games", network));
            var retrieveOperation = TableOperation.Retrieve<GameEntity>(userName, gameId);
            var game = await gameTable.ExecuteAsync(retrieveOperation);

            var result = LogDto.Map(log.FirstOrDefault());
            result.UserState = ((GameEntity)game.Result)?.State.UserState;
            
            return result;
        }

        public string GetTableName(string tableName, Network network)
        {
            return $"{tableName}{network.ToString().ToLower()}";
        }
    }
}
