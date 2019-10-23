﻿using System;
using Core.Common.Domain.Auctions;
using Core.Common.Domain.Bids;
using Core.Common.Domain.Categories;
using Core.Common.Domain.Products;
using Core.Common.Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Query.ReadModel
{
    public class AuctionReadModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string AuctionId { get; set; }


        public UserIdentity Creator { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
        public AuctionImage[] AuctionImages { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime EndDate { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public int TotalBids { get; set; }


        [BsonDefaultValue(false)]
        public bool Completed { get; set; }
        [BsonDefaultValue(false)]
        public bool Canceled { get; set; }
        public bool Bought { get; set; }
        public UserIdentity Buyer { get; set; }
        public Bid WinningBid { get; set; }

        [BsonDefaultValue(0)]
        public long Version { get; set; }
    }
}
