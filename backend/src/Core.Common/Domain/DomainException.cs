﻿using System;

namespace Core.Common.Domain
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}