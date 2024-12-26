using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchItemFromKeyVault
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string keyVaultName = "abaranwal-test";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var token = new DefaultAzureCredential();

            var client = new SecretClient(new Uri(kvUri), token);

            Console.WriteLine(token.GetType().ToString());
            
            Console.WriteLine("Retrieving secrets from Key Vault...");

            var secretProperties = client.GetPropertiesOfSecrets().ToList();
            foreach (var secret in secretProperties)
            {
                var secretValue = client.GetSecretAsync(secret.Name).Result;
                Console.WriteLine($"Secret Name: {secret.Name}, Secret Value: {secretValue.Value}");
            }

            Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
