using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using FantasyGaming.Domain.Commands;
using FantasyGaming.Functions.Models;
using FantasyGaming.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FantasyGaming.Functions
{
    public class UserService
    {
        private readonly ILogger<GameService> _logger;

        public UserService(ILogger<GameService> log)
        {
            _logger = log;
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
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string gameId = req.Query["gameId"];
            string userId = req.Query["userId"];

            var transactionItem = new TransactionItem
            {
                GameId = gameId,
                UserId = userId
            };

            string instanceId = await client.StartNewAsync(nameof(Orchestrator.SagaOrchestrator), transactionItem.Id, transactionItem);

            string responseMessage = string.IsNullOrEmpty(gameId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {gameId}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        
    }
}

