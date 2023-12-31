using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using FantasyGaming.Domain.Messaging;
using FantasyGaming.Functions.Models;
using FantasyGaming.Functions.Utils;
using FantasyGaming.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public class PaymentService
    {
        private readonly IMessageBus _messageBus;

        public PaymentService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [FunctionName("CheckPaymentCredit")]
        public async Task Run([ServiceBusTrigger("%PaymentSvcMessageQueue%", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            [CosmosDB(
                databaseName: @"%CosmosDbDatabaseName%",
                containerName: @"%PaymentCollection%",
                Connection = "CosmosDbConnectionString")] CosmosClient client,
            ILogger logger)
        {
            try
            {
                // Get the message body from the incoming message object.
                string inputMessageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogInformation($"CheckPaymentCredit function recieved message: {inputMessageBody}");
                var creditCheckCommand = JsonConvert.DeserializeObject<UserCreditCheckCommand>(inputMessageBody);

                // Credit check logic goes here
                var container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbDatabaseName"),
                  Environment.GetEnvironmentVariable("PaymentCollection"));
                var paymentInfo = container.GetItemLinqQueryable<PaymentInfo>(true).
                    Where(x => x.UserId == creditCheckCommand.Content.UserId)
                    .AsEnumerable().FirstOrDefault();

                if (paymentInfo != null)
                {
                    var userCreditCheckedEvent = BuildUserCreditCheckedEvent(creditCheckCommand);
                    if (paymentInfo.Balance > 10)
                    {
                        userCreditCheckedEvent.IsEnoughCredit = true;
                    }
                    else
                    {
                        userCreditCheckedEvent.IsEnoughCredit = false;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    _messageBus.PublishEvent(userCreditCheckedEvent);
                }
                else
                {
                    logger.LogError("Payment info not found in Db for user {userId}", creditCheckCommand.Content.UserId);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }

        [FunctionName("SeedPaymentData")]
        [OpenApiOperation(operationId: "SeedData", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> SeedData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payment/seeddata")] HttpRequest req,
            [CosmosDB(
                databaseName: @"%CosmosDbDatabaseName%",
                containerName: @"%PaymentCollection%",
                Connection = "CosmosDbConnectionString")]
                IAsyncCollector<PaymentInfo> documentCollector)
        {
            var paymentSeedData = new List<PaymentInfo> 
            {
                new PaymentInfo
                {
                    UserId = "579859",
                    Balance = 20
                },
                new PaymentInfo
                {
                    UserId = "123456",
                    Balance = 5
                },
                new PaymentInfo
                {
                    UserId = "456123",
                    Balance = 11
                },
                new PaymentInfo
                {
                    UserId = "987654",
                    Balance = 4
                }
            };

            foreach (PaymentInfo paymentInfo in paymentSeedData)
            {
                await documentCollector.AddAsync(paymentInfo);
            }
            return new OkObjectResult(null);
        }

        private static MessageHeader BuildEventHeader(string transactionId, string messageType)
        {
            return new MessageHeader(transactionId, messageType, Sources.Payment.ToString());
        }

        private static UserCreditChecked BuildUserCreditCheckedEvent(UserCreditCheckCommand creditCheckCommand)
        {
            return new UserCreditChecked()
            {
                UserId = creditCheckCommand.Content.UserId,
                Header = BuildEventHeader(creditCheckCommand.Header.TransactionId, nameof(UserCreditChecked))
            };
        }
    }
}
