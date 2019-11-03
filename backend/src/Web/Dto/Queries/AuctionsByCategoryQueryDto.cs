﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Query.Queries.Auction.Auctions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Dto.Queries
{
    public class AuctionsByCategoryQueryDto
    {
        [FromQuery(Name = "page")]
        public int Page { get; set; }
        [FromQuery(Name = "categories")] public string[] CategoryNames { get; set; }

        [FromQuery(Name = "cond")]
        public ConditionQuery ConditionQuery { get; set; } = ConditionQuery.All;
        [FromQuery(Name = "minbpr")]
        public decimal MinBuyNowPrice { get; set; } = 0;
        [FromQuery(Name = "maxbpr")]
        public decimal MaxBuyNowPrice { get; set; } = 0;
        [FromQuery(Name = "minapr")]
        public decimal MinAuctionPrice { get; set; } = 0;
        [FromQuery(Name = "maxapr")]
        public decimal MaxAuctionPrice { get; set; } = 0;
        [FromQuery(Name = "type")]
        public AuctionTypeQuery AuctionTypeQuery { get; set; } = AuctionTypeQuery.All;
    }
}