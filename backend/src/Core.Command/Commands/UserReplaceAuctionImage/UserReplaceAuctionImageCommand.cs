﻿using System;
using System.ComponentModel.DataAnnotations;
using Core.Command.Exceptions;
using Core.Common;
using Core.Common.Attributes;
using Core.Common.Command;
using Core.Common.Common;
using Core.Common.Domain.Auctions;

namespace Core.Command.Commands.UserReplaceAuctionImage
{
    [AuthorizationRequired]
    [SaveTempAuctionImage]
    public class UserReplaceAuctionImageCommand : CommandBase
    {
        public Guid AuctionId { get; }

        [AuctionImage]
        public IFileStreamAccessor Img { get; set; }

        [SaveTempPath]
        public string TempPath { get; set; }

        [Required]
        [MaxLength(5)]
        [ValidAuctionImageExtension]
        public string Extension { get; }

        [Range(0, AuctionConstantsFactory.DEFAULT_MAX_IMAGES - 1)]
        public int ImgNum { get; }

        public UserReplaceAuctionImageCommand(Guid auctionId, IFileStreamAccessor img, int imgNum, string extension)
        {
            if (auctionId.Equals(Guid.Empty)) { throw new InvalidCommandException($"Invalid field AuctionId = {auctionId}"); }
            AuctionId = auctionId;
            Img = img;
            ImgNum = imgNum;
            Extension = extension;
        }
    }
}