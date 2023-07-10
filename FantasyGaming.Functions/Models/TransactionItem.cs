using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Functions.Models
{
    public class TransactionItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("gameId")]
        public string GameId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; } = nameof(SagaState.Pending);

        //[JsonProperty("accountFromId")]
        //public string AccountFromId { get; set; }

        //[JsonProperty("accountToId")]
        //public string AccountToId { get; set; }

        //[JsonProperty("amount")]
        //public decimal Amount { get; set; }
    }
}
