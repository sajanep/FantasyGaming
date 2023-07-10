using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Domain.Commands
{
    public class GameLimitCheckCommand : BaseCommand
    {
        public UserCreditCheckCommandContent Content { get; set; }
    }

    public class GameLimitCheckCommandContent
    {
        public string UserId { get; set; }

        public string GameId { get; set; }
    }
}
