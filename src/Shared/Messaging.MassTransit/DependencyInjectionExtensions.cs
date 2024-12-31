using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Core.Messaging;
using Shared.Core.Extensions;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Core.Messaging;

namespace Shared.Messaging.MassTransit;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddMasstransitEventBus(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies
    )
    {
        builder.Services.AddSingleton<IExternalEventBus, MasstransitExternalBus>();
        builder.Services.AddConfigurationOptions<MessagingOptions>(nameof(MessagingOptions));

        if (assemblies.Length == 0)
        {
            // Find assemblies that reference the current assembly
            var referencingAssemblies = Assembly.GetExecutingAssembly().GetReferencingAssemblies();
            assemblies = referencingAssemblies.ToArray();
        }

        var messagingOptions = builder.Configuration.BindOptions<MessagingOptions>(nameof(MessagingOptions));

        builder.Services.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();

            foreach (var assembly in assemblies)
            {
                busRegistrationConfigurator.AddConsumers(assembly);
            }

            switch (messagingOptions.BrokerType)
            {
                case BrokerType.InMemory:
                    {
                        ConfigInMemoryBroker(busRegistrationConfigurator);
                    }

                    break;
                case BrokerType.RabbitMQ:
                    {
                        ConfigRabbitMQBroker(messagingOptions, busRegistrationConfigurator);
                    }

                    break;
                case BrokerType.Kafka:
                    {
                        ArgumentNullException.ThrowIfNull(messagingOptions.KafkaOptions);
                    }

                    break;
            }
        });

        return builder;
    }

    private static void ConfigRabbitMQBroker(
        MessagingOptions messagingOptions,
        IBusRegistrationConfigurator busRegistrationConfigurator
    )
    {
        ArgumentNullException.ThrowIfNull(messagingOptions.RabbitMQOptions);

        busRegistrationConfigurator.UsingRabbitMq(
            (context, configurator) =>
            {
                configurator.Host(
                    messagingOptions.RabbitMQOptions.Host,
                    h =>
                    {
                        h.Username(messagingOptions.RabbitMQOptions.Username);
                        h.Password(messagingOptions.RabbitMQOptions.Password);
                    }
                );

                configurator.ConfigureEndpoints(context);
            }
        );
    }

    private static void ConfigInMemoryBroker(IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        busRegistrationConfigurator.UsingInMemory(
            (context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
            }
        );
    }
}
