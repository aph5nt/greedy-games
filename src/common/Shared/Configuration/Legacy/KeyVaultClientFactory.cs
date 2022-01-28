//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Microsoft.Azure.KeyVault;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Serilog;

//namespace Shared.Configuration
//{
//    public static class KeyVaultClientFactory
//    {
//        public static readonly Lazy<KeyVaultAuth> ApplicationConfiguration = new Lazy<KeyVaultAuth>(() =>
//        {
//            //var configuration = 
//            //    ? WebConfigurationManager.OpenWebConfiguration("~")
//            //    : ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

//            //var appConfig = (KeyVaultAuth) configuration.Sections["KeyVaultAuth"];
//            return null;//appConfig;
//        });

//        private static async Task<string> AuthenticationSecretCallback(string authority, string resource, string scope)
//        {
//            var adCredential = new ClientCredential(ApplicationConfiguration.Value.Id, ApplicationConfiguration.Value.Secret);
//            var authenticationContext = new AuthenticationContext(authority);
//            var token = await authenticationContext.AcquireTokenAsync(resource, adCredential);

//            return token.AccessToken;
//        }

//        private static async Task<string> AuthenticationCertCallback(string authority, string resource, string scope)
//        {
//            try
//            {
//                var clientAssertionCertPfx =
//                    CertificateHelper.FindCertificateByThumbprint(ApplicationConfiguration.Value.CertThumb);
//                var assertionCert =
//                    new ClientAssertionCertificate(ApplicationConfiguration.Value.Id, clientAssertionCertPfx);
//                var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
//                var token = await context.AcquireTokenAsync(resource, assertionCert);
//                return token.AccessToken;
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex, "Failed to acquire the certificate");
//                return string.Empty;
//            }
//        }

//        public static KeyVaultClient Create(KeyVaultClientAuthType authType)
//        {
//            switch (authType)
//            {
//                case KeyVaultClientAuthType.Secret:
//                    return new KeyVaultClient(AuthenticationSecretCallback);
//                case KeyVaultClientAuthType.Certificate:
//                    return new KeyVaultClient(AuthenticationCertCallback);
//                default:
//                    throw new NotImplementedException();
//            }
//        }
//    }

//    public static class CertificateHelper
//    {
//        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
//        {
//            var thumbprint = Regex.Replace(findValue, @"[^\da-zA-z]", string.Empty).ToUpper();

//            var stores = new[]
//            {
//                new X509Store(StoreName.My, StoreLocation.LocalMachine),
//                new X509Store(StoreName.My, StoreLocation.CurrentUser)
//            };

//            foreach (var store in stores)
//                try
//                {
//                    store.Open(OpenFlags.ReadOnly);
//                    Log.Debug("Availiable certificates in {@Store}: {@Certificates}", $"{store.Name}.{store.Location}",
//                        Certificates(store.Certificates));
//                    var col = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
//                    if (col.Count != 0)
//                        return col[0];
//                }
//                finally
//                {
//                    store.Close();
//                }

//            Log.Error("Requested certificate with {@Thumbprint} not found.", thumbprint);
//            return null;
//        }

//        private static IEnumerable<string> Certificates(X509Certificate2Collection certificates)
//        {
//            return from X509Certificate2 certificate in certificates
//                select $"{certificate.FriendlyName}:{certificate.Thumbprint}\n";
//        }
//    }
//}