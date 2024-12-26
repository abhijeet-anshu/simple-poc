using System;
using Azure.Identity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Reflection.Metadata;
using Azure;
using System.Runtime.CompilerServices;
using Azure.Core;

namespace EventGridHTTPSend
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            //await SendingEvent(args);

            await TaskExecutorAsync();
            
            Console.WriteLine("Press any key to exit");

            Console.ReadKey(true);
        }

        public static async Task TaskExecutorAsync()
        {
            try
            {
                await DoTaskAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task DoTaskAsync()
        {
            //await GetTokenAsync();
            HttpSender httpClient =  GetHttpClientTokenBased();
            await SendEventAsync(httpClient);
        }

        /*public static async Task<string> GetTokenAsync()
        {
            //var tokenRequestContext = new TokenRequestContext(new[] { "https://eventgrid.azure.net/.default" });
            var tokenRequestContext = new TokenRequestContext(new[] { "https://eventgrid.azure.net" });
            var token = await new DefaultAzureCredential().GetTokenAsync(tokenRequestContext);
            return token.Token;
        }*/

        static async Task SendingEvent(string[] args)
        {
            var client = GetHttpClientSasKeyBased();
            await SendEventAsync(client);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        protected const string AegSasKey = "aeg-sas-key";
        private const string TopicKey = "";

        static readonly string TopicEndpoint =
            "https://abaranwal-eg-domain-test.eastus-1.eventgrid.azure.net/api/events";



        static HttpSender GetHttpClientSasKeyBased()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add(AegSasKey, TopicKey);

            return new HttpSender(httpClient);
        }

        static HttpSender GetHttpClientTokenBased()
        {
            HttpClient httpClient = new HttpClient();

            HttpSender httpSender = new HttpSender(httpClient, new DefaultAzureCredential());

            return httpSender;
        }

        static async Task SendEventAsync(HttpSender httpSender)
        {
            var events = new[]
            {
                new GridEvent
                {
                    Topic = "topic1017",
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Contoso.Items.ItemReceived",
                    Data = new ItemReceivedEventData
                    {
                        ItemSku = "Contoso Item SKU #1"
                    },
                    EventTime = DateTime.Now,
                    Subject = "Door1",
                    DataVersion = "2.0"
                },
                new GridEvent
                {
                    Topic = "top009",
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Contoso.Items.ItemReceived",
                    Data = new ItemReceivedEventData
                    {
                        ItemSku = "Contoso Item SKU #2"
                    },
                    EventTime = DateTime.Now,
                    Subject = "Door1",
                    DataVersion = "2.0"
                }
            };

            string body = JsonConvert.SerializeObject(events);

            using (HttpResponseMessage response = await httpSender.PostAsync(new Uri(TopicEndpoint),
                new StringContent(body)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode + 
                        $"Code: {response.StatusCode}. " +
                        $"ResponsePhrase: {response.ReasonPhrase}");
                }

                Console.WriteLine("Response: " + response.StatusCode);
                Console.WriteLine("Response Message: " + await response.Content.ReadAsStringAsync());
            }

            Console.WriteLine("Events sent successfully");
        }
    }
}