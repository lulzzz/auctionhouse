﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core.Common.Command;
using Core.Common.Domain;

namespace Core.Common.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(IAppEvent<T> @event) where T : Event;
        void Publish<T>(IEnumerable<IAppEvent<T>> events) where T : Event;
        void Send<T>(T command) where T : ICommand;
    }
}