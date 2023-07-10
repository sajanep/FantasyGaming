using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using FantasyGaming.Functions.Models;
using FantasyGaming.Functions.Utils;
using FantasyGaming.Infrastructure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
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

        private static readonly IMessageBus _messsageBus = new MessageBus();

        [FunctionName(nameof(SagaOrchestrator))]
        public static async Task SagaOrchestrator(
          [OrchestrationTrigger] IDurableOrchestrationContext context,
          ILogger logger)
        {
            TransactionItem item = context.GetInput<TransactionItem>();
            var orchestratorActivityFunction = nameof(OrchestratorActivity.SagaOrchestratorActivity);
            
            var result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);

            var creditCheckCommand = new UserCreditCheckCommand
            {
                Content = new UserCreditCheckCommandContent
                {
                    UserId = item.UserId
                }
            };
            _messsageBus.SendCommand(creditCheckCommand);

            var userCreditCheckedEvent = await DurableOrchestrationContextExtensions
              .WaitForExternalEventWithTimeout<UserCreditChecked>(context, Sources.User, TimeSpan.FromSeconds(30));

            if(!userCreditCheckedEvent.IsEnoughCredit)
            {
                logger.LogError("Saga Failed");
                item.State = nameof(SagaState.Fail);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
            }

            var gameLimitCheckCommand = new GameLimitCheckCommand
            {
                Content = new UserCreditCheckCommandContent
                {
                    UserId = item.UserId
                }
            };
            _messsageBus.SendCommand(gameLimitCheckCommand);

            var gameLimitCheckedEvent = await DurableOrchestrationContextExtensions
              .WaitForExternalEventWithTimeout<GameLimitChecked>(context, Sources.Game, TimeSpan.FromSeconds(30));
            if (gameLimitCheckedEvent.IsGameLimitExceeded)
            {
                logger.LogError("Saga Failed");
                item.State = nameof(SagaState.Fail);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
            }

            if(userCreditCheckedEvent.IsEnoughCredit && !gameLimitCheckedEvent.IsGameLimitExceeded)
            {
                logger.LogError("Saga Completed");
                item.State = nameof(SagaState.Success);
                result = await context.CallActivityWithRetryAsync<TransactionItem>(orchestratorActivityFunction, RetryOptions, item);
            }

        }
    }
}
