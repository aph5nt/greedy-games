//using System.Collections.Generic;
//using Microsoft.Azure.KeyVault;

//namespace Shared.Configuration
//{
//    public class KeyVaultConfigurationSource : IConfigurationSource
//    {
//        private readonly Dictionary<string, string> _configurations = new Dictionary<string, string>();

//        public KeyVaultConfigurationSource()
//        {
//            var appConfig = KeyVaultClientFactory.ApplicationConfiguration.Value;
//            var client = KeyVaultClientFactory.Create(appConfig.AuthType);
//            var vaultBaseUrl = $"https://{appConfig.Name}.vault.azure.net";
//            var configMetadata = client.GetSecretsAsync(vaultBaseUrl, 25).Result;

//            foreach (var metadata in configMetadata)
//            {
//                var config = client.GetSecretAsync(metadata.Id).Result;
//                _configurations[config.SecretIdentifier.Name] = config.Value;
//            }
//        }

//        public string GetValueOrNull(string key)
//        {
//            if (_configurations.ContainsKey(key))
//                return _configurations[key];

//            return null;
//        }
//    }
//}