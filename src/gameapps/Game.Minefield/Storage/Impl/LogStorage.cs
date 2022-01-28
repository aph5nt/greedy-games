using Game.Minefield.Contracts.Model;
using Microsoft.WindowsAzure.Storage.Table;
using Shared.Configuration;
using Shared.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Minefield.Storage.Impl
{
    public class LogStorage : BaseStorage, ILogStorage
    {
        public override string TableName => "log";

        public LogStorage(AppServerSettings serverSettings) : base(serverSettings)
        {
        }

        public async Task InsertAsync(Settings settings)
        {
            var table = CloudTableClient.GetTableReference(GetTableName(settings.Network));

            var insertOperation = TableOperation.InsertOrReplace(new LogEntity(settings.UserName, settings.Id)
            {
                ClientSeed = settings.Seed.ClientGuid.ToString(),
                ServerSeed = settings.Seed.ServerGuid.ToString(),
                Value = settings.Seed.Value.ToString()
            });

            await table.ExecuteAsync(insertOperation);
        }

        public async Task<LogEntity> GetAsync(Network network, string userName, string gameId)
        {
            var query = new TableQuery<LogEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userName),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId)))
                .Take(1);

            var table = CloudTableClient.GetTableReference(GetTableName(network));
            var result = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            return result.FirstOrDefault();
        }
    }
}