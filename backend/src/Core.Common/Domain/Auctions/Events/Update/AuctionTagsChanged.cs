﻿using System;

namespace Core.Common.Domain.Auctions.Events.Update
{
    public class AuctionTagsChanged : UpdateEvent
    {
        public Guid AuctionId { get; }
        public Tag[] Tags { get; }

        public AuctionTagsChanged(Guid auctionId, Tag[] tags) : base(EventNames.AuctionTagsChanged)
        {
            AuctionId = auctionId;
            Tags = tags;
        }
    }
}
