﻿using System;

namespace Core.Common.Domain.Auctions.Events
{
    public class AuctionRemoved : Event
    {
        public Guid AuctionId { get; }

        public AuctionRemoved(Guid auctionId) : base(EventsNames.AuctionRemovedEventName)
        {
            AuctionId = auctionId;
        }
    }
}
