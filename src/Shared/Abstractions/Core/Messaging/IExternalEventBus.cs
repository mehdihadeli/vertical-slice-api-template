namespace Shared.Abstractions.Core.Messaging;

public interface IExternalEventBus
{
    public Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, IMessage;

    public Task PublishAsync<TMessage>(
        TMessage message,
        string? exchangeOrTopic = null,
        string? queue = null,
        IDictionary<string, object>? metadataHeaders = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : class, IMessage;
}
