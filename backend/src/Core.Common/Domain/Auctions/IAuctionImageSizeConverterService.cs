﻿namespace Core.Common.Domain.Auctions
{
    public interface IAuctionImageSizeConverterService
    {
        AuctionImageRepresentation ConvertTo(AuctionImageRepresentation imageRepresentation, AuctionImageSize size);
    }
}