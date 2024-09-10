using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Text;

// Create a blob container client that the event processor will use
// TODO: Replace <STORAGE_ACCOUNT_NAME> and <BLOB_CONTATINAER_NAME> with actual names
BlobContainerClient storageClient = new BlobContainerClient(
    new Uri("https://abaranwalehcheckpointsa.blob.core.windows.net/blob-hub01"),
    new DefaultAzureCredential());

// Create an event processor client to process events in the event hub
// TODO: Replace the <EVENT_HUBS_NAMESPACE> and <HUB_NAME> placeholder values
var processor = new EventProcessorClient(
    storageClient,
    EventHubConsumerClient.DefaultConsumerGroupName,
    "abaranwal-eh-test.servicebus.windows.net",
    "hub01",
    new DefaultAzureCredential());

// Register handlers for processing events and handling errors
processor.ProcessEventAsync += ProcessEventHandler;
processor.ProcessErrorAsync += ProcessErrorHandler;

// Start the processing
await processor.StartProcessingAsync();

Console.WriteLine("sleeping for 30 seconds");

// Wait for 30 seconds for the events to be processed
await Task.Delay(TimeSpan.FromSeconds(30));

Console.WriteLine("sleep over");

// Stop the processing
await processor.StopProcessingAsync();

Console.WriteLine("signalled for stop execution");

Task ProcessEventHandler(ProcessEventArgs eventArgs)
{
    // Write the body of the event to the console window
    Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
    //Console.ReadLine();
    return Task.CompletedTask;
}

Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
{
    // Write details about the error to the console window
    Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
    Console.WriteLine(eventArgs.Exception.Message);
    Console.ReadLine();
    return Task.CompletedTask;
}