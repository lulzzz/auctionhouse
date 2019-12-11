﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Core.Common;
using Core.Common.Attributes;
using Core.Common.Command;
using Core.Common.Domain.Auctions;
using Core.Common.Domain.Users;
using Core.Common.EventBus;
using Core.Common.Exceptions.Command;
using MediatR;

namespace Core.Command.AddOrReplaceAuctionImage
{
    [AuthorizationRequired]
    public class UserAddAuctionImageCommand : ICommand
    {
        public Guid AuctionId { get; }
        [Required]
        public AuctionImageRepresentation Img { get; }

        [SignedInUser]
        public UserIdentity SignedInUser { get; set; }

        public UserAddAuctionImageCommand(Guid auctionId, AuctionImageRepresentation img)
        {
            if (auctionId.Equals(Guid.Empty)) { throw new InvalidCommandException($"Invalid field AuctionId = {auctionId}"); }
            AuctionId = auctionId;
            Img = img;
        }
    }
}
