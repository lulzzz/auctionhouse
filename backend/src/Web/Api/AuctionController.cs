﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Command.AuctionCreateSession_AddAuctionImage;
using Core.Command.AuctionCreateSession_RemoveImage;
using Core.Command.AuctionCreateSession_StartAuctionCreateSession;
using Core.Command.Bid;
using Core.Command.CreateAuction;
using Core.Command.EndAuction;
using Core.Common.Domain.Auctions;
using Core.Common.Domain.Categories;
using Core.Common.Domain.Products;
using Core.Common.EventBus;
using Core.Query.Queries.Auction.AuctionImage;
using Core.Query.Queries.Auction.Auctions;
using Core.Query.Queries.Auction.Categories;
using Core.Query.Queries.Auction.SingleAuction;
using Core.Query.ReadModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Dto.Commands;
using Web.Dto.Queries;

namespace Web.Api
{
    [ApiController]
    [Route("api")]
    public class AuctionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuctionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "User"), HttpPost("bid")]
        public async Task<ActionResult> Bid([FromBody] BidCommandDto commandDto)
        {
            var guid = Guid.Parse(commandDto.AuctionId);
            await _mediator.Send(new BidCommand(guid, commandDto.Price, new CorrelationId(commandDto.CorrelationId)));
            return Ok();
        }

        [Authorize(Roles = "User"), HttpPost("createAuction")]
        public async Task<ActionResult> Bid([FromBody] CreateAuctionCommandDto commandDto)
        {
            var command = new CreateAuctionCommand(commandDto.BuyNowPrice, commandDto.Product, commandDto.StartDate,
                commandDto.EndDate,
                commandDto.Category, new CorrelationId(commandDto.CorrelationId));
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("endAuction"), Authorize(AuthenticationSchemes = "X-API-Key", Roles = "TimeTaskService")]
        public async Task<ActionResult> EndAuction([FromBody] EndAuctionCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("auctions")]
        public async Task<ActionResult<IEnumerable<AuctionsQueryResult>>> Auctions(
            [FromQuery] AuctionsQueryDto queryDto)
        {
            var auctions = await _mediator.Send(new AuctionsQuery()
                {Page = queryDto.Page, CategoryNames = queryDto.Categories.ToList()});
            return Ok(auctions);
        }

        [HttpGet("auction")]
        public async Task<ActionResult<AuctionReadModel>> Auction([FromQuery] AuctionQueryDto queryDto)
        {
            var auction = await _mediator.Send(new AuctionQuery(queryDto.AuctionId));
            return Ok(auction);
        }

        [HttpGet("categories"), ResponseCache(Duration = 31536000, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<CategoryTreeNode>> CategoriesQuery()
        {
            var categoriesTree = await _mediator.Send(new CategoriesQuery());
            return Ok(categoriesTree);
        }

        [Authorize(Roles = "User"), HttpPost("startCreateSession")]
        public async Task<ActionResult> StartCreateSession([FromBody] StartAuctionCreateSessionCommandDto commandDto)
        {
            var correlationId = new CorrelationId(commandDto.CorrelationId);
            var command = new StartAuctionCreateSessionCommand(correlationId);
            await _mediator.Send(command);
            return Ok();
        }

        [Authorize(Roles = "User"), HttpPost("removeAuctionImage")]
        public async Task<ActionResult> RemoveAuctionImage([FromQuery] RemoveImageCommandDto commandDto)
        {
            var command = new RemoveImageCommand()
            {
                ImgNum = commandDto.ImgNum
            };
            await _mediator.Send(command);
            return Ok();
        }

        [Authorize(Roles = "User"), HttpPost("addAuctionImage")]
        public async Task<ActionResult> AddAuctionImage([FromForm] AddAuctionImageCommandDto commandDto)
        {
            var correlationId = new CorrelationId(commandDto.CorrelationId);
            var buffer = new byte[1024 * 1024 * 5];
            using (var stream = new MemoryStream(buffer))
            {
                commandDto.Img.CopyTo(stream);
                var imageRepresentation = new AuctionImageRepresentation()
                {
                    Img = stream.ToArray()
                };
                var command = new AddAuctionImageCommand(imageRepresentation, correlationId, commandDto.ImgNum);
                await _mediator.Send(command);
                return Ok();
            }
            
        }

        [HttpGet("auctionImage")]
        public async Task<ActionResult<AuctionImageQueryResult>> AuctionImage([FromQuery] AuctionImageQueryDto queryDto)
        {
            var query = new AuctionImageQuery()
            {
                ImageId = queryDto.ImageId
            };
            var result = await _mediator.Send(query);
            return File(result.Img.Img, "image/jpeg");
        }

        [Authorize(Roles = "User"), HttpPost("test")]
        public async Task Test()
        {
            await _mediator.Send(new CreateAuctionCommand(20, new Product() {Description = "desc", Name = "name"},
                DateTime.Today, DateTime.Today.AddDays(1), new List<string>(), new CorrelationId("test")));
        }
    }
}