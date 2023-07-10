using System;
using System.Text;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FantasyGaming.Domain.Commands;
using Microsoft.Azure.Documents;
using FantasyGaming.Functions.Models;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using FantasyGaming.Domain.Events;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace FantasyGaming.Functions
{
    public class SagaEventProcessor
    {
        [FunctionName(nameof(SagaEventProcessor))]
        public async Task Run([ServiceBusTrigger("%SagaReplyMessageQueue%", Connection = "ServiceBusConnection")] Message message,
            MessageReceiver messageReceiver,
            [CosmosDB(
                databaseName: @"%CosmosDbDatabaseName%",
                containerName: @"%OrchestratorCollection%",
                Connection = "CosmosDbConnectionString")] CosmosClient client,
            [DurableClient] IDurableOrchestrationClient orchestrationClient,
            ILogger logger)
        {
            try
            {
                // Get the message body from the incoming message object.
                string inputMessageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogInformation($"C# ServiceBus queue trigger function processed message: {inputMessageBody}");
                var baseEvent = JsonConvert.DeserializeObject<BaseEvent>(inputMessageBody);
                if (baseEvent.Header == null)
                {
                    logger.LogError("Invalid event header");
                }
                
                var container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbDatabaseName"), 
                    Environment.GetEnvironmentVariable("OrchestratorCollection"));
                var transactionItem = container.GetItemLinqQueryable<TransactionItem>().
                    Where(x => x.Id == baseEvent.Header.TransactionId).FirstOrDefault();

                await orchestrationClient.RaiseEventAsync(baseEvent.Header.TransactionId,
                    baseEvent.Header.Source,
                    GetConcreteEvent(baseEvent));

                await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                await messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken);
            }
        }

        object GetConcreteEvent(BaseEvent baseEvent) 
        {
            switch(baseEvent.Header.MessageType)
            {
                case nameof(GameLimitChecked):
                    {
                        return (GameLimitChecked)baseEvent;
                    }
                case nameof(UserCreditChecked):
                    {
                        return (UserCreditChecked)baseEvent;
                    }
            }
            return null;
        }
    }
}
