﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FantasyGaming.Domain.Commands;

namespace FantasyGaming.Domain.Events
{
    public class GameLimitChecked : BaseEvent
    {
        public bool IsGameLimitExceeded { get; set; }
    }
}