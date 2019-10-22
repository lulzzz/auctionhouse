﻿using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FunctionalTests")]
namespace Core.Common.EventBus
{
    internal interface IEventConsumer
    {
        Type MessageType { get; }
        void Dispatch(object message);
    }
}