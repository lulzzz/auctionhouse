﻿using MongoDB.Bson.Serialization.Attributes;

namespace Core.Query.Views
{
    public class TopAuctionImage
    {
        public string Size1Id { get; set; }
        public string Size2Id { get; set; }
        public string Size3Id { get; set; }
    }

    public class TopAuction
    {
        public string AuctionId { get; set; }
        public string AuctionName { get; set; }
        public TopAuctionImage AuctionImage { get; set; }
    }

    public class TopAuctionsInTag
    {
        [BsonId]
        public string Tag { get; set; }
        public int Total { get; set; }
        public TopAuction[] Auctions { get; set; }
    }
}
