using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using FantasyGaming.Domain.Messaging;
using FantasyGaming.Functions.Models;
using FantasyGaming.Functions.Utils;
using FantasyGaming.Infrastructure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public static class Orchestrator
    {
        private static readonly RetryOptions RetryOptions = new RetryOptions(
         firstRetryInterval: TimeSpan.FromSeconds(5),
         maxNumberOfAttempts: 3);

        private static readonly IMessageBus _messageBus = new MessageBus();

        [FunctionName(nameof(SagaOrchestrator))]
        public static async Task SagaOrchestrator(
          [OrchestrationTrigger] IDurableOrchestrationContext context,
          ILogger logger)
        {
            TransactionItem item = context.GetInput<TransactionItem>();
            logger.LogInformation("Starting Saga orchestration for the trassanction with {id}", context.InstanceId);

            var orchestratorActivityFunction = nameof(OrchestratorActivity.SagaOrchestratorActivity);
            var result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);

            //var creditCheckCommand = new UserCreditCheckCommand
            //{
            //    Content = new UserCreditCheckCommandContent
            //    {
            //        UserId = item.UserId
            //    },
            //    Header = BuildHeader(item.Id, nameof(UserCreditCheckCommand), Sources.User.ToString())
            //};
            //_messageBus.SendCommand(creditCheckCommand);

            var userCreditCheckedEventStr = await DurableOrchestrationContextExtensions
              .WaitForExternalEventWithTimeout<string>(context, nameof(UserCreditChecked), TimeSpan.FromSeconds(60));

            var userCreditCheckedEvent = JsonConvert.DeserializeObject<UserCreditChecked>(userCreditCheckedEventStr);
            if (userCreditCheckedEvent != null && !userCreditCheckedEvent.IsEnoughCredit)
            {
                logger.LogError("Saga Failed since User does not have enough credit");
                item.State = nameof(SagaState.Fail);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
            }

            //var gameLimitCheckCommand = new GameLimitCheckCommand
            //{
            //    Content = new GameLimitCheckCommandContent
            //    {
            //        UserId = item.UserId,
            //        GameId = item.GameId,
            //    },
            //    Header = BuildHeader(item.Id, nameof(GameLimitCheckCommand), Sources.Game.ToString())
            //};
            //_messageBus.SendCommand(gameLimitCheckCommand);

            var gameLimitCheckedEventStr = await DurableOrchestrationContextExtensions
              .WaitForExternalEventWithTimeout<string>(context, nameof(GameLimitChecked), TimeSpan.FromSeconds(60));
            var gameLimitCheckedEvent = JsonConvert.DeserializeObject<GameLimitChecked>(gameLimitCheckedEventStr);
            if (gameLimitCheckedEvent!= null && gameLimitCheckedEvent.IsGameLimitExceeded)
            {
                logger.LogError("Saga Failed since User exceeded Game Limit");
                item.State = nameof(SagaState.Fail);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
            }

            if(userCreditCheckedEvent.IsEnoughCredit && !gameLimitCheckedEvent.IsGameLimitExceeded)
            {
                item.State = nameof(SagaState.Success);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
                logger.LogInformation("Saga Completed successfully");
            }
        }

        private static MessageHeader BuildHeader(string transactionId, string messageType, string source)
        {
            return new MessageHeader(transactionId, messageType, source);
        }
    }
}
