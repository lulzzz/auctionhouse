﻿using System.Collections.Generic;
using Core.Common.Domain;
using Core.Common.EventBus;
using Core.Common.Interfaces;

namespace Core.Common.ApplicationServices
{
    public class EventBusService
    {
        private readonly IEventBus _eventBus;
        private readonly IAppEventBuilder _appEventBuilder;

        public EventBusService(IEventBus eventBus, IAppEventBuilder appEventBuilder)
        {
            _eventBus = eventBus;
            _appEventBuilder = appEventBuilder;
        }

        public virtual void Publish<T>(T @event, CorrelationId correlationId, ICommand command) where T : Event
        {
            var appEvent = _appEventBuilder
                .WithCommand(command)
                .WithEvent(@event)
                .WithCorrelationId(correlationId)
                .Build<T>();
            _eventBus.Publish(appEvent);
        }

        public virtual void Publish(IEnumerable<Event> events, CorrelationId correlationId, ICommand command)
        {
            foreach (var @event in events)
            {
                Publish(@event, correlationId, command);
            }
        }
    }
}
