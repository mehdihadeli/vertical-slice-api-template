using Humanizer;
using MassTransit;
using RabbitMQ.Client;
using Shared.Abstractions.Core.Messaging;
using Shared.Core.Messaging;

namespace Shared.Messaging.MassTransit;

public class MasstransitExternalBus(IBus bus) : IExternalEventBus
{
    public async Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, IMessage
    {
        // https://github.com/MassTransit/MassTransit/blob/eb3c9ee1007cea313deb39dc7c4eb796b7e61579/src/MassTransit/SqlTransport/SqlTransport/ConnectionContextSupervisor.cs#L35
        await bus.Publish(
            message,
            envelopeWrapperContext =>
                FillMasstransitContextInformation(message, envelopeWrapperContext, metadataHeaders),
            cancellationToken
        );
    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        string? exchangeOrTopic = null,
        string? queue = null,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, IMessage
    {
        var bindExchangeName = typeof(TMessage).Name.Underscore();

        if (string.IsNullOrEmpty(exchangeOrTopic))
        {
            exchangeOrTopic = $"{bindExchangeName}{MessagingConstants.PrimaryExchangePostfix}";
        }

        // Ref: https://stackoverflow.com/a/60269493/581476
        string endpointAddress = GetEndpointAddress(
            exchangeOrTopic: exchangeOrTopic,
            queue: queue,
            bindExchange: bindExchangeName,
            exchangeType: ExchangeType.Direct
        );

        var sendEndpoint = await bus.GetSendEndpoint(new Uri(endpointAddress));
        // https://github.com/MassTransit/MassTransit/blob/eb3c9ee1007cea313deb39dc7c4eb796b7e61579/src/MassTransit/SqlTransport/SqlTransport/ConnectionContextSupervisor.cs#L53
        await sendEndpoint.Send(
            message,
            envelopeWrapperContext =>
                FillMasstransitContextInformation(message, envelopeWrapperContext, metadataHeaders),
            cancellationToken
        );
    }

    private static string GetEndpointAddress(
        string exchangeOrTopic,
        string? queue,
        string? bindExchange,
        string? exchangeType = ExchangeType.Direct,
        bool bindQueue = false
    )
    {
        // https://masstransit.io/documentation/concepts/producers#short-addresses
        // https://github.com/MassTransit/MassTransit/blob/ac44867da9d7a93bb7d330680586af123c1ee0b7/src/Transports/MassTransit.RabbitMqTransport/RabbitMqEndpointAddress.cs#L63
        // https://github.com/MassTransit/MassTransit/blob/ac44867da9d7a93bb7d330680586af123c1ee0b7/src/Transports/MassTransit.RabbitMqTransport/RabbitMqEndpointAddress.cs#L98
        // Start with the base address
        string endpoint = $"exchange:{exchangeOrTopic}?type={exchangeType}&durable=true";

        // If there is a bindExchange, add it to the query parameters
        if (!string.IsNullOrEmpty(bindExchange))
        {
            endpoint += $"&bindexchange={bindExchange}";
        }

        if (!string.IsNullOrEmpty(queue))
        {
            endpoint += $"&queue={queue}";
        }

        if (bindQueue)
        {
            endpoint += "&bind=true";
        }

        return endpoint;
    }

    private static void FillMasstransitContextInformation<TMessage>(
        TMessage message,
        SendContext<TMessage> envelopeWrapperContext,
        IDictionary<string, object>? headers
    )
        where TMessage : class, IMessage
    {
        // https://masstransit.io/documentation/concepts/messages#message-headers
        // https://www.enterpriseintegrationpatterns.com/patterns/messaging/EnvelopeWrapper.html
        // Just for filling masstransit related field, but we have a separated envelope message.
        envelopeWrapperContext.MessageId = message.MessageId;
        envelopeWrapperContext.CorrelationId = message.CorrelationId;

        if (headers is not null)
        {
            foreach (var header in headers)
            {
                envelopeWrapperContext.Headers.Set(header.Key, header.Value);
            }
        }
    }
}
