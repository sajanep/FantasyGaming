using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Domain.Commands
{
    public class UserCreditCheckCommand: BaseCommand
    {
        public UserCreditCheckCommandContent Content { get; set; }
    }

    public class UserCreditCheckCommandContent
    {
        public string UserId { get; set; }
    }
}
