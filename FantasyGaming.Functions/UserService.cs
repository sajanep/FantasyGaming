using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Messaging;
using FantasyGaming.Functions.Models;
using FantasyGaming.Functions.Utils;
using FantasyGaming.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IMessageBus _messageBus;

        public UserService(ILogger<UserService> log, IMessageBus messageBus)
        {
            _logger = log;
            _messageBus = messageBus;
        }

        [FunctionName("Register")]
        [OpenApiOperation(operationId: "Register", tags: new[] { "name" })]
        [OpenApiParameter(name: "gameId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **GameId** parameter")]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **UserId** parameter")]

        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            string gameId = req.Query["gameId"];
            string userId = req.Query["userId"];
            _logger.LogInformation("Register function received a request with gameid: {gameId} and userid:{userId}.", gameId, userId);

            var item = new TransactionItem
            {
                GameId = gameId,
                UserId = userId
            };

            var creditCheckCommand = new UserCreditCheckCommand
            {
                Content = new UserCreditCheckCommandContent
                {
                    UserId = item.UserId
                },
                Header = BuildHeader(item.Id, nameof(UserCreditCheckCommand), Sources.User.ToString())
            };
            _messageBus.SendCommand(creditCheckCommand);

            var gameLimitCheckCommand = new GameLimitCheckCommand
            {
                Content = new GameLimitCheckCommandContent
                {
                    UserId = item.UserId,
                    GameId = item.GameId,
                },
                Header = BuildHeader(item.Id, nameof(GameLimitCheckCommand), Sources.Game.ToString())
            };
            _messageBus.SendCommand(gameLimitCheckCommand);

            string instanceId = await client.StartNewAsync(nameof(Orchestrator.SagaOrchestrator), item.Id, item);

            string responseMessage = string.Format("Saga triggered with instance id {0}", instanceId);
            _logger.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }

        private static MessageHeader BuildHeader(string transactionId, string messageType, string source)
        {
            return new MessageHeader(transactionId, messageType, source);
        }
    }
}

