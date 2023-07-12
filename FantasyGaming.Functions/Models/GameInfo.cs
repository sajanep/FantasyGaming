using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Functions.Models
{
    public class GameInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("gamesregistered")]
        public int GamesRegistered { get; set; }
    }
}
