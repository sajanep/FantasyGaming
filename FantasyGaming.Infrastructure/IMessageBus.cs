using FantasyGaming.Domain.Commands;
using FantasyGaming.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyGaming.Infrastructure
{
    public interface IMessageBus
    {
        public void SendCommand<T>(T command) where T : BaseCommand;

        public void PublishEvent<T>(T reponse) where T : BaseEvent;
    }
}
