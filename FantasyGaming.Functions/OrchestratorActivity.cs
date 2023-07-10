using FantasyGaming.Functions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FantasyGaming.Domain.Events;
using Microsoft.Azure.Cosmos;

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
        Connection = @"CosmosDbConnectionString")]
      IAsyncCollector<TransactionItem> documentCollector,
          [CosmosDB(
        databaseName: @"%CosmosDbDatabaseName%",
        containerName: @"%OrchestratorCollection%",
        Connection = @"CosmosDbConnectionString")] CosmosClient client,
          ILogger logger)
        {
            if (item.State == SagaState.Pending.ToString())
            {
                await documentCollector.AddAsync(item);
                return item;
            }

            var container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbDatabaseName"),
                    Environment.GetEnvironmentVariable("OrchestratorCollection"));
            var transactionItem = container.GetItemLinqQueryable<TransactionItem>().
                Where(x => x.Id == item.Id).FirstOrDefault();
            transactionItem.State = item.State;

            await container.ReplaceItemAsync(transactionItem, transactionItem.Id);
            
            return item;
        }
    }
}
