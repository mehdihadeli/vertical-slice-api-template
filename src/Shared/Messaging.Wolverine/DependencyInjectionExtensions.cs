using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Extensions;
using Shared.Core.Messaging;
using Shared.EF;
using Shared.Abstractions.Core.Messaging;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Shared.Messaging.Wolverine;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddWolverineEventBus(this WebApplicationBuilder builder)
    {
        var messagingOptions = builder.Configuration.BindOptions<MessagingOptions>(nameof(MessagingOptions));
        var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>(nameof(PostgresOptions));

        builder.Services.AddWolverine(options =>
        {
            var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq");

            if (!postgresOptions.UseInMemory)
            {
                options.PersistMessagesWithPostgresql(postgresOptions.ConnectionString);
                options.UseEntityFrameworkCoreTransactions();
            }

            if (messagingOptions.BrokerType == BrokerType.RabbitMQ)
            {
                ArgumentNullException.ThrowIfNull(messagingOptions.RabbitMQOptions);
                if (!string.IsNullOrWhiteSpace(rabbitMqConnectionString))
                {
                    options.UseRabbitMqUsingNamedConnection("rabbitmq").AutoProvision();
                }
                else
                {
                    options.UseRabbitMq(rabbitMq =>
                    {
                        rabbitMq.HostName = messagingOptions.RabbitMQOptions.Host;
                        rabbitMq.Port = messagingOptions.RabbitMQOptions.Port;
                        rabbitMq.UserName = messagingOptions.RabbitMQOptions.Username;
                        rabbitMq.Password = messagingOptions.RabbitMQOptions.Password;
                    }).AutoProvision();
                }
            }
        });

        builder.Services.AddScoped<IExternalEventBus, WolverineExternalEventBus>();
        return builder;
    }
}
