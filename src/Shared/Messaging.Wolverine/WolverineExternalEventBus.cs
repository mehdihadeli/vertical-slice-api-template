using Shared.Abstractions.Core.Messaging;
using Wolverine;
using ApplicationMessage = Shared.Abstractions.Core.Messaging.IMessage;

namespace Shared.Messaging.Wolverine;

internal sealed class WolverineExternalEventBus(IMessageBus bus) : IExternalEventBus
{
    public Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, ApplicationMessage => bus.PublishAsync(message, CreateDeliveryOptions(metadataHeaders)).AsTask();

    public Task PublishAsync<TMessage>(
        TMessage message,
        string? exchangeOrTopic = null,
        string? queue = null,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, ApplicationMessage
    {
        var options = CreateDeliveryOptions(metadataHeaders);
        if (!string.IsNullOrWhiteSpace(exchangeOrTopic))
            options = options.WithHeader("exchange", exchangeOrTopic);
        if (!string.IsNullOrWhiteSpace(queue))
            options = options.WithHeader("queue", queue);

        return bus.SendAsync(message, options).AsTask();
    }

    private static DeliveryOptions CreateDeliveryOptions(IDictionary<string, object>? metadataHeaders)
    {
        var options = new DeliveryOptions();
        if (metadataHeaders is null)
            return options;

        foreach (var header in metadataHeaders)
            options = options.WithHeader(header.Key, Convert.ToString(header.Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);

        return options;
    }
}
