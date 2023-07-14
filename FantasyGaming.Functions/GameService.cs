using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using FantasyGaming.Domain.Messaging;
using FantasyGaming.Functions.Models;
using FantasyGaming.Functions.Utils;
using FantasyGaming.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public class GameService
    {
        private readonly ILogger<GameService> _logger;

        private readonly IMessageBus _messageBus;

        public GameService(ILogger<GameService> log, 
            IMessageBus messageBus)
        {
            _logger = log;
            _messageBus = messageBus;
        }

        [FunctionName("StartActivity")]
        [OpenApiOperation(operationId: "StartActivity", tags: new[] { "name" })]
        [OpenApiParameter(name: "gameId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **GameId** parameter")]
        [OpenApiParameter(name: "activityId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ActivityId** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> StartActivity(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string gameId = req.Query["gameId"];
            string activityId = req.Query["activityId"];

            string responseMessage = string.IsNullOrEmpty(gameId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {gameId}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("StopActivity")]
        [OpenApiOperation(operationId: "StopActivity", tags: new[] { "name" })]
        [OpenApiParameter(name: "gameId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **GameId** parameter")]
        [OpenApiParameter(name: "activityId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ActivityId** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> StopActivity(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string gameId = req.Query["gameId"];
            string activityId = req.Query["activityId"];

            string responseMessage = string.IsNullOrEmpty(gameId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {gameId}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("CheckGameLimit")]
        public async Task Run([ServiceBusTrigger("%GameSvcMessageQueue%", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            [CosmosDB(
                databaseName: @"%CosmosDbDatabaseName%",
                containerName: @"%GameCollection%",
                Connection ="CosmosDbConnectionString")] CosmosClient client,
            ILogger logger)
        {
            try
            {
                // Get the message body from the incoming message object.
                string inputMessageBody = Encoding.UTF8.GetString(message.Body);
                logger.LogInformation($"CheckGameLimit function received message: {inputMessageBody}");
                var gameLimitCheckCommand = JsonConvert.DeserializeObject<GameLimitCheckCommand>(inputMessageBody);

                // Game limit check logic goes here
                var container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbDatabaseName"),
                 Environment.GetEnvironmentVariable("GameCollection"));
                var gameInfo = container.GetItemLinqQueryable<GameInfo>(true).
                    Where(x => x.UserId == gameLimitCheckCommand.Content.UserId)
                    .AsEnumerable().FirstOrDefault();

                var gameLimitCheckedEvent = BuildGameLimitCheckedEvent(gameLimitCheckCommand);
                if (gameInfo.GamesRegistered > 5)
                {
                    gameLimitCheckedEvent.IsGameLimitExceeded = true;
                }
                else
                {
                    gameLimitCheckedEvent.IsGameLimitExceeded = false;
                }
                await Task.Delay(TimeSpan.FromSeconds(30));
                _messageBus.PublishEvent(gameLimitCheckedEvent);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }

        [FunctionName("SeedGameData")]
        [OpenApiOperation(operationId: "SeedData", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> SeedData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "game/seeddata")] HttpRequest req,
            [CosmosDB(
                databaseName: @"%CosmosDbDatabaseName%",
                containerName: @"%GameCollection%",
                Connection = "CosmosDbConnectionString")]
                IAsyncCollector<GameInfo> documentCollector)
        {
            var gameSeedData = new List<GameInfo> 
            {
                new GameInfo
                {
                    UserId = "579859",
                    GamesRegistered = 2
                },
                new GameInfo
                {
                    UserId = "123456",
                    GamesRegistered = 3
                },
                new GameInfo
                {
                    UserId = "456123",
                    GamesRegistered = 6
                },
                new GameInfo
                {
                    UserId = "987654",
                    GamesRegistered = 8
                }
            };

            foreach (GameInfo gameInfo in gameSeedData)
            {
                await documentCollector.AddAsync(gameInfo);
            }
            return new OkObjectResult(null);
        }

        private static MessageHeader BuildEventHeader(string transactionId, string messageType)
        {
            return new MessageHeader(transactionId, messageType, Sources.Game.ToString());
        }

        private static GameLimitChecked BuildGameLimitCheckedEvent(GameLimitCheckCommand gameLimitCheckCommand)
        {
            return new GameLimitChecked()
            {
                UserId = gameLimitCheckCommand.Content.UserId,
                Header = BuildEventHeader(gameLimitCheckCommand.Header.TransactionId, nameof(GameLimitChecked))
            };
        }
    }
}

