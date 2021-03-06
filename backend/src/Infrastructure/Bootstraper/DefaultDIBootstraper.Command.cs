﻿using System;
using System.Linq;
using System.Reflection;
using Core.Command;
using Core.Command.CreateAuction;
using Core.Command.Handler;
using Core.Command.Mediator;
using Core.Common;
using Core.Common.ApplicationServices;
using Core.Common.Auth;
using Core.Common.Domain.AuctionCreateSession;
using Core.Common.Domain.Auctions;
using Core.Common.Domain.Categories;
using Core.Common.Domain.Users;
using Core.Common.EventBus;
using Core.Common.SchedulerService;
using Core.Common.Command;
using Core.Common.DomainServices;
using Infrastructure.Auth;
using Infrastructure.Repositories;
using Infrastructure.Repositories.AuctionImage;
using Infrastructure.Repositories.SQLServer;
using Infrastructure.Services;
using Infrastructure.Services.EventBus;
using Infrastructure.Services.SchedulerService;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RestEase;

namespace Infrastructure.Bootstraper
{
    public static partial class DefaultDIBootstraper
    {
        public static class Command
        {
            private static void ConfigureServiceSettings(
                IServiceCollection serviceCollection, MsSqlConnectionSettings sqlServerConnectionSettings,
                RabbitMqSettings rabbitMqSettings, TimeTaskServiceSettings timeTaskServiceSettings,
                ImageDbSettings imageDbSettings,
                UserAuthDbContextOptions userAuthDbContextOptions,
                CategoryNameServiceSettings categoryNameServiceSettings)
            {
                serviceCollection.AddSingleton(sqlServerConnectionSettings);
                serviceCollection.AddSingleton(rabbitMqSettings);
                serviceCollection.AddSingleton(categoryNameServiceSettings);
                serviceCollection.AddSingleton(timeTaskServiceSettings);
                serviceCollection.AddSingleton(imageDbSettings);
                serviceCollection.AddSingleton(userAuthDbContextOptions);
            }

            private static void
                ConfigureAuctionCreateSessionService<AuctionCreateSessionServiceT>(
                    IServiceCollection serviceCollection)
                where AuctionCreateSessionServiceT : class, IAuctionCreateSessionService
            {
                serviceCollection.AddTransient<IAuctionCreateSessionService, AuctionCreateSessionServiceT>();
            }

            private static void
                ConfigureUserIdentitySessionService<UserIdentityServiceImplT>(IServiceCollection serviceCollection)
                where UserIdentityServiceImplT : class, IUserIdentityService
            {
                serviceCollection.AddSingleton<IUserIdentityService, UserIdentityServiceImplT>();
            }

            private static void ConfigureAuthDbServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddScoped<IUserAuthenticationDataRepository, UserAuthenticationDataRepository>();
                serviceCollection.AddScoped<IResetPasswordCodeRepository, ResetPasswordCodeRepository>();
            }

            private static void ConfigureImageServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddSingleton<ImageDbContext>();
                serviceCollection.AddScoped<IAuctionImageRepository, AuctionImageRepository>();
                serviceCollection.AddSingleton<IAuctionImageConversionService, AuctionImageConversionService>();
                serviceCollection.AddScoped<AuctionImageService>();
            }

            private static void ConfigureAuctionShedulerService(IServiceCollection serviceCollection,
                TimeTaskServiceSettings timeTaskServiceSettings)
            {
                serviceCollection.AddSingleton<ITimeTaskClient>(provider =>
                    {
                        var client = RestClient.For<ITimeTaskClient>(timeTaskServiceSettings.ConnectionString);
                        client.ApiKey = timeTaskServiceSettings.ApiKey;
                        return client;
                    }
                );
                serviceCollection.AddSingleton<IAuctionSchedulerService, AuctionSchedulerService>();
            }

            private static void ConfigureDomainRepositories(IServiceCollection serviceCollection)
            {
                serviceCollection.AddScoped<IAuctionRepository, MsSqlAuctionRepository>();
                serviceCollection.AddScoped<IUserRepository, MsSqlUserRepository>();
            }


            private static void ConfigureDecoratedCommandHandlers(IServiceCollection serviceCollection)
            {
                var decoratedHandlerTypes = Assembly.Load("Core.Command")
                    .GetTypes()
                    .Where(type =>
                        type.BaseType != null && type.BaseType.IsGenericType &&
                        type.BaseType.GetGenericTypeDefinition() == typeof(DecoratedCommandHandlerBase<>));

                foreach (var handlerType in decoratedHandlerTypes)
                {
                    serviceCollection.AddScoped(handlerType);
                }
            }

            private static void ConfigureResetLinkSenderService<T>(IServiceCollection serviceCollection) where T : class, IResetLinkSenderService
            {
                serviceCollection.AddSingleton<IResetLinkSenderService, T>();
            }

            public static void Configure<UserIdentityServiceImplT, AuctionCreateSessionServiceImplT, ResetLinkSenderServiceImplT>(
                IServiceCollection serviceCollection,
                MsSqlConnectionSettings eventStoreConnectionSettings,
                RabbitMqSettings rabbitMqSettings,
                TimeTaskServiceSettings timeTaskServiceSettings,
                ImageDbSettings imageDbSettings,
                UserAuthDbContextOptions userAuthDbContextOptions,
                CategoryNameServiceSettings categoryNameServiceSettings
            )
                where UserIdentityServiceImplT : class, IUserIdentityService
                where AuctionCreateSessionServiceImplT : class, IAuctionCreateSessionService
                where ResetLinkSenderServiceImplT : class, IResetLinkSenderService
            {
                ConfigureServiceSettings(serviceCollection, eventStoreConnectionSettings, rabbitMqSettings,
                    timeTaskServiceSettings, imageDbSettings, userAuthDbContextOptions, categoryNameServiceSettings);
                ConfigureAuthDbServices(serviceCollection);
                ConfigureUserIdentitySessionService<UserIdentityServiceImplT>(serviceCollection);
                ConfigureAuctionCreateSessionService<AuctionCreateSessionServiceImplT>(serviceCollection);
                ConfigureImageServices(serviceCollection);
                ConfigureDomainRepositories(serviceCollection);
                ConfigureAuctionShedulerService(serviceCollection, timeTaskServiceSettings);
                ConfigureDecoratedCommandHandlers(serviceCollection);
                ConfigureResetLinkSenderService<ResetLinkSenderServiceImplT>(serviceCollection);
                serviceCollection.AddScoped<CreateAuctionCommandHandlerDepedencies>();


                serviceCollection.AddSingleton<IHTTPQueuedCommandStatusStorage, HTTPMemQueuedCommandStatusStorage>();
                serviceCollection.AddScoped<WSQueuedCommandHandler>();
                serviceCollection.AddScoped<HTTPQueuedCommandHandler>();
                serviceCollection.AddScoped<MediatRCommandHandlerMediator>();
                serviceCollection.AddScoped<EventBusCommandHandlerMediator>();
                serviceCollection.AddScoped<HTTPQueuedCommandHandlerMediator>();
                serviceCollection.AddScoped<ImmediateCommandMediator>();
                serviceCollection.AddScoped<WSQueuedCommandMediator>();
                serviceCollection.AddScoped<HTTPQueuedCommandMediator>();

                serviceCollection.AddScoped<HTTPQueuedCommandStatusService>();
            }


        }
    }
}