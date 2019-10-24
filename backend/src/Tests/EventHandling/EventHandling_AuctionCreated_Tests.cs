﻿using System;
using System.Collections.Generic;
using System.Threading;
using Core.Command.CreateAuction;
using Core.Common.Auth;
using Core.Common.Domain.Auctions.Events;
using Core.Common.Domain.Categories;
using Core.Common.Domain.Products;
using Core.Common.Domain.Users;
using Core.Common.EventBus;
using Core.Common.EventSignalingService;
using Core.Query.Handlers;
using Core.Query.ReadModel;
using FluentAssertions;
using FunctionalTests.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace FunctionalTests.EventHandling
{
    public class EventHandling_AuctionCreated_Tests
    {
        private User user;
        private Product product;
        private CorrelationId correlationId;
        private CreateAuctionCommandHandler commandHandler;

        [SetUp]
        public void SetUp()
        {
            var services = TestDepedencies.Instance.Value;
            user = new User();
            user.Register("testUserName");
            user.MarkPendingEventsAsHandled();
            product = new Product() { Name = "test product", Description = "desc" };
            correlationId = new CorrelationId("test_correlationId");
            services.DbContext.UsersReadModel.InsertOne(new UserReadModel()
                { UserIdentity = new UserIdentityReadModel(user.UserIdentity) });
        }

        private void SetUpCommandHandler()
        {
            var services = TestDepedencies.Instance.Value;
            var userIdService = new Mock<IUserIdentityService>();
            userIdService.Setup(f => f.GetSignedInUserIdentity())
                .Returns(user.UserIdentity);

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(f => f.FindUser(It.IsAny<UserIdentity>()))
                .Returns(user);

            var session = user.UserIdentity.GetAuctionCreateSession();
            commandHandler =
                new CreateAuctionCommandHandler(services.AuctionRepository, userIdService.Object,
                    services.SchedulerService, services.EventBus,
                    Mock.Of<ILogger<CreateAuctionCommandHandler>>(),
                    new CategoryBuilder(services.CategoryTreeService), userRepository.Object,
                    services.GetAuctionCreateSessionService(session));
        }

        CreateAuctionCommand GetCreateCommand()
        {
            var categories = new List<string>()
            {
                "Fake category", "Fake subcategory", "Fake subsubcategory 0"
            };
            return new CreateAuctionCommand(20.0m, product, DateTime.UtcNow.AddMinutes(20),
                DateTime.UtcNow.AddDays(12),
                categories, correlationId);
        }

        private bool VerifyEvent(AuctionCreated auctionCreated, CreateAuctionCommand command)
        {
            var v1 = auctionCreated != null;
            var v2 = auctionCreated.AuctionId != Guid.Empty;
            var v3 = auctionCreated.AuctionArgs.Product.Name.Equals(command.Product.Name) &&
                     auctionCreated.AuctionArgs.Product.Description.Equals(command.Product.Description);
            var v4 = auctionCreated.AuctionArgs.BuyNowPrice.Equals(command.BuyNowPrice);
            var v5 = auctionCreated.AuctionArgs.StartDate.CompareTo(command.StartDate) == 0;
            var v6 = auctionCreated.AuctionArgs.Creator.UserId.Equals(user.UserIdentity.UserId);
            return v1 && v2 && v3 && v4 && v5 && v6;
        }

        [Test]
        public void CreateAuctionCommand_handled_and_readmodel_is_created()
        {
            var services = TestDepedencies.Instance.Value;
            var sem = new SemaphoreSlim(0, 1);

            var command = GetCreateCommand();

            string idFromHandler = "";
            CorrelationId correlationIdFromHandler = null;

            var eventHandler = new Mock<AuctionCreatedHandler>(
                services.AppEventBuilder, services.DbContext,
                Mock.Of<IEventSignalingService>());
            eventHandler
                .Setup(f => f.Consume(
                    It.Is<IAppEvent<AuctionCreated>>(v => VerifyEvent(v.Event, command)))
                )
                .Callback((IAppEvent<AuctionCreated> ev) =>
                {
                    idFromHandler = ev.Event.AuctionId.ToString();
                    correlationIdFromHandler = ev.CorrelationId;
                    sem.Release();
                })
                .CallBase();

            services.SetupEventBus(eventHandler.Object);
            SetUpCommandHandler();

            //act
            commandHandler.Handle(command, CancellationToken.None);
            sem.Wait(TimeSpan.FromSeconds(5));
            Thread.Sleep(4000);

            var auctionReadModel = services.DbContext.AuctionsReadModel.Find(a => a.AuctionId == idFromHandler)
                .FirstOrDefault();
            UserReadModel userReadModel = services.DbContext.UsersReadModel
                .Find(f => f.UserIdentity.UserId == user.UserIdentity.UserId.ToString())
                .FirstOrDefault();


            eventHandler.Verify(f => f.Consume(It.Is<IAppEvent<AuctionCreated>>(v => VerifyEvent(v.Event, command))),
                Times.Once);

            auctionReadModel.Should()
                .NotBeNull();
            auctionReadModel.Product.Should()
                .BeEquivalentTo(product);
            auctionReadModel.Creator.Should()
                .BeEquivalentTo(userReadModel.UserIdentity);

            userReadModel.CreatedAuctions.Count.Should()
                .Be(1);

            correlationId.Value.Should()
                .BeEquivalentTo(correlationIdFromHandler.Value);
        }
    }
}