using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Shared.Configuration;
using Shared.Model;

namespace Game.Minefield.Storage.Impl
{
    public abstract class BaseStorage
    {
        protected   CloudTableClient CloudTableClient;

        public AppServerSettings Settings { get; set; }

        protected BaseStorage(AppServerSettings serverSettings)
        {
            Settings = serverSettings;

            var cloudStorageAccount = CloudStorageAccount.Parse(Settings.ConnectionString.Storage);
            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        public abstract string TableName { get; }

        public string GetTableName(Network network)
        {
            return $"{TableName}{network.ToString().ToLower()}";
        }

        public virtual async Task InitAsync()
        {
            foreach (Network network in Enum.GetValues(typeof(Network)))
            {
                await CloudTableClient
                    .GetTableReference(GetTableName(network))
                    .CreateIfNotExistsAsync();
            }
               
        }
    }
}