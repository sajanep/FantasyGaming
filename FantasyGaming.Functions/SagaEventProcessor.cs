using FantasyGaming.Domain.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public class SagaEventProcessor
    {
        [FunctionName(nameof(SagaEventProcessor))]
        public async Task Run([ServiceBusTrigger("%SagaReplyMessageQueue%", Connection = "ServiceBusConnection")] Message message,
            [DurableClient] IDurableOrchestrationClient orchestrationClient,
            ILogger logger)
        {
            try
            {
                // Get the message body from the incoming message object.
                string inputMessageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogInformation($"Saga Event Processor function recieved message: {inputMessageBody}");
                var baseEvent = JsonConvert.DeserializeObject<BaseEvent>(inputMessageBody);
                if (baseEvent.Header == null)
                {
                    logger.LogError("Invalid event header");
                }

                await orchestrationClient.RaiseEventAsync(baseEvent.Header.TransactionId,
                    baseEvent.Header.MessageType, eventData:inputMessageBody);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }
        
    }
}
