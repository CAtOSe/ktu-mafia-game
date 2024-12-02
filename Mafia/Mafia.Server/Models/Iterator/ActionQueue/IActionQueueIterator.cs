﻿using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Iterator.ActionQueue
{
    public interface IActionQueueIterator
    {
        ActionQueueEntry First();
        ActionQueueEntry Next();
        bool HasMore();
        ActionQueueEntry Current { get; }
    }

}