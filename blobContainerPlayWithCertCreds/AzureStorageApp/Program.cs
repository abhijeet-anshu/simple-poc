using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        
        var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(@"C:\.secret\appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        // Load configuration

        var tenantId = config["AzureAd:TenantId"];
        var clientId = config["AzureAd:ClientId"];
        var clientSecret = config["AzureAd:ClientSecret"];

        string storageAccountName = config["StorageAccount:AccountName"] ?? "";
        string containerName = config["StorageAccount:ContainerName"] ?? "";


        try
        {

            
            


            // Authenticate with credentials
            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                clientSecretCredential);

            // Container operations
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            Console.WriteLine($"Container '{containerName}' ready");
            Console.WriteLine("Blobs in container:");

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine($" - {blobItem.Name}");
            }

            // Generate User Delegation SAS URI
            // Generate User Delegation SAS URI
            var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.All);

            var sasToken = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName).ToString();
            var sasUri = new Uri($"{containerClient.Uri}?{sasToken}");
            Console.WriteLine($"SAS URI for container: {sasUri}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
