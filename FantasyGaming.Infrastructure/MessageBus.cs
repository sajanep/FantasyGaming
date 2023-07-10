using Azure.Messaging.ServiceBus;
using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using System.Text.Json;

namespace FantasyGaming.Infrastructure
{
    public class MessageBus : IMessageBus
    {
        public void SendCommand<T>(T command) where T : BaseCommand
        {
            string cs = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
            var client = new ServiceBusClient(cs);
            var queueSender = client.CreateSender(GetQueueName(command));
            string jsonEntity = JsonSerializer.Serialize(command);
            ServiceBusMessage serializedContents = new ServiceBusMessage(jsonEntity);
            queueSender.SendMessageAsync(serializedContents);
        }

        public void PublishEvent<T>(T @event) where T : BaseEvent
        {
            string cs = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
            var client = new ServiceBusClient(cs);
            var queueSender = client.CreateSender(Environment.GetEnvironmentVariable("SagaReplyMessageQueue"));
            string jsonEntity = JsonSerializer.Serialize(@event);
            ServiceBusMessage serializedContents = new ServiceBusMessage(jsonEntity);
            queueSender.SendMessageAsync(serializedContents);
        }

        private string GetQueueName(BaseCommand command)
        {
            switch (command)
            {
                case UserCreditCheckCommand creditCheckCommand:
                    return Environment.GetEnvironmentVariable("PaymentSvcMessageQueue");
                case GameLimitCheckCommand gameLimitCheckCommand:
                    return Environment.GetEnvironmentVariable("GameSvcMessageQueue");
            }
            return string.Empty;
        }
    }
}