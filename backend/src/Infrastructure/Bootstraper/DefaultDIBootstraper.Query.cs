﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using Core.Common;
using Core.Common.ApplicationServices;
using Core.Common.Domain;
using Core.Common.Domain.Auctions;
using Core.Common.Domain.Categories;
using Core.Common.EventBus;
using Core.Common.Query;
using Core.Query.Handlers;
using Core.Query.Handlers.AuctionUpdateHandlers;
using Core.Query.ReadModel;
using Infrastructure.Repositories.AuctionImage;
using Infrastructure.Services;
using Infrastructure.Services.EventBus;

namespace Infrastructure.Bootstraper
{
    public static partial class DefaultDIBootstraper
    {
        public static class Query
        {
            private static void ConfigureEventHandlers(IServiceCollection serviceCollection)
            {
                var handlerTypes = Assembly.Load("Core.Query")
                    .GetTypes()
                    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EventConsumer<>));

                foreach (var handlerType in handlerTypes)
                {
                    serviceCollection.AddScoped(handlerType);
                }
            }

            private static void ConfigureSettings(IServiceCollection serviceCollection, MongoDbSettings mongoDbSettings,
                CategoryNameServiceSettings categoryNameServiceSettings, ImageDbSettings imageDbSettings,
                RabbitMqSettings rabbitMqSettings)
            {
                serviceCollection.AddSingleton(rabbitMqSettings);
                serviceCollection.AddSingleton(mongoDbSettings);
                serviceCollection.AddSingleton(categoryNameServiceSettings);
                serviceCollection.AddSingleton(imageDbSettings);
            }

            public static void Configure(IServiceCollection serviceCollection,
                MongoDbSettings mongoDbSettings,
                CategoryNameServiceSettings categoryNameServiceSettings,
                ImageDbSettings imageDbSettings,
                RabbitMqSettings rabbitMqSettings)
            {
                serviceCollection.AddSingleton<ReadModelDbContext>();

                ConfigureSettings(serviceCollection, mongoDbSettings, categoryNameServiceSettings, imageDbSettings,
                    rabbitMqSettings);
                ConfigureEventHandlers(serviceCollection);
            }
        }
    }
}