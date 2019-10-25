﻿using System;
using Core.Common.Domain.Categories;
using Core.Common.Domain.Products;
using Core.Common.Domain.Users;

namespace Core.Common.Domain.Auctions
{
    public class AuctionArgs
    {
        public decimal? BuyNowPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public UserIdentity Creator { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
        public AuctionImage[] AuctionImages { get; set; } = null;

        public class Builder
        {
            private AuctionArgs args = new AuctionArgs();

            private void CheckCanBuild()
            {
                if (args.Product == default || args.StartDate == default || args.EndDate == default || args.Creator == default ||
                    args.Category == default)
                {
                    throw new DomainException("Invalid auctionArgs");
                }
            }

            public Builder From(AuctionArgs args)
            {
                this.args = args;
                return this;
            }
            public Builder SetBuyNow(decimal buyNowPrice)
            {
                args.BuyNowPrice = buyNowPrice;
                return this;
            }

            public Builder SetStartDate(DateTime startDate)
            {
                args.StartDate = startDate;
                return this;
            }

            public Builder SetEndDate(DateTime endDate)
            {
                args.EndDate = endDate;
                return this;
            }

            public Builder SetOwner(UserIdentity owner)
            {
                args.Creator = owner;
                return this;
            }

            public Builder SetProduct(Product product)
            {
                args.Product = product;
                return this;
            }

            public Builder SetCategory(Category category)
            {
                args.Category = category;
                return this;
            }

            public Builder SetImages(AuctionImage[] images)
            {
                args.AuctionImages = images;
                return this;
            }

            public AuctionArgs Build()
            {
                CheckCanBuild();
                return args;
            }
        }
    }
}