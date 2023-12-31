using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FantasyGaming.Functions.Utils
{
    public static class DurableOrchestrationContextExtensions
    {
        public static async Task<T> WaitForExternalEventWithTimeout<T>(IDurableOrchestrationContext ctx, string name, TimeSpan timeout)
        {
            if (ctx is null) throw new ArgumentNullException(nameof(ctx));
            if (timeout < TimeSpan.MinValue) throw new ArgumentOutOfRangeException(nameof(timeout));

            var cts = new CancellationTokenSource();
            var state = default(T);

            DateTime timeoutAt = ctx.CurrentUtcDateTime.Add(timeout);
            Task<T> timeoutTask = ctx.CreateTimer<T>(timeoutAt, state, cts.Token);
            Task<T> waitForEventTask = ctx.WaitForExternalEvent<T>(name);

            Task<T> winner = await Task.WhenAny<T>(waitForEventTask, timeoutTask);

            //The Durable Task Framework will not change an orchestration's status to "completed"
            //until all outstanding tasks are completed or canceled.
            if (winner == waitForEventTask) cts.Cancel();
 
            return winner.Result;
        }
    }

    public enum Sources
    {
        User,
        Voting,
        Game,
        Payment,
        Orchestrator,
    }
}