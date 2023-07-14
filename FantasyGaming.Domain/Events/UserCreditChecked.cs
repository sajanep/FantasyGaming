using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FantasyGaming.Domain.Commands;

namespace FantasyGaming.Domain.Events
{
    public class UserCreditChecked : BaseEvent
    {
        public string UserId { get; set; }

        public bool IsEnoughCredit { get; set; }
    }
}
