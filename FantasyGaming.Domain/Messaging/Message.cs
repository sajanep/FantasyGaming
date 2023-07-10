using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Domain.Messaging
{
    public abstract class Message
    {
        public MessageHeader Header { get; set; }
    }
}
