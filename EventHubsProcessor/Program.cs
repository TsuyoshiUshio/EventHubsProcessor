using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsProcessor
{
    class Program
    {
        private static IConfiguration configuration;
        static void Main(string[] args)
        {
            var builer = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            configuration = builer.Build();
        }

        private static async Task GenerateLoad()
        {
            var eventHubsConnectionString = configuration["eventHubsConnectionString"];
            var eventHubName = configuration["eventHubName"];
            int batchSize = 100;
            int repeat = 1000;
            await using(var producer = new EventHubProducerClient(eventHubsConnectionString, eventHubName))
            {
                using EventDataBatch eventBatch = await producer.CreateBatchAsync();
                for (int i = 0; i < repeat; i++)
                {
                    for (int l = 0; l < batchSize; l++)
                    {
                        eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Rep {i} Message {l}")));
                    }
                    await producer.SendAsync(eventBatch);
                } 
            }
        }

    }
}
