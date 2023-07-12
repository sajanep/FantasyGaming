using FantasyGaming.Functions.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyGaming.Functions
{
    public static class OrchestratorActivity
    {
        [FunctionName(nameof(SagaOrchestratorActivity))]
        public static async Task<TransactionItem> SagaOrchestratorActivity(
          [ActivityTrigger] TransactionItem item,
         [CosmosDB(
        databaseName: @"%CosmosDbDatabaseName%",
        containerName: @"%OrchestratorCollection%",
        Connection = "CosmosDbConnectionString")]
      IAsyncCollector<TransactionItem> documentCollector,
          [CosmosDB(
        databaseName: @"%CosmosDbDatabaseName%",
        containerName: @"%OrchestratorCollection%",
        Connection = "CosmosDbConnectionString")] CosmosClient client,
          ILogger logger)
        {
            if (item.State == SagaState.Pending.ToString())
            {
                await documentCollector.AddAsync(item);
                return item;
            }

            var container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbDatabaseName"),
                    Environment.GetEnvironmentVariable("OrchestratorCollection"));
            var transactionItem = container.GetItemLinqQueryable<TransactionItem>(true).
                Where(x => x.Id == item.Id).
                AsEnumerable().FirstOrDefault();
            transactionItem.State = item.State;

            await container.ReplaceItemAsync(transactionItem, transactionItem.Id);
            
            return item;
        }
    }
}
