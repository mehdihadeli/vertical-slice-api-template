using Shared.Abstractions.Core.Messaging;

namespace Shared.Core.Messaging;

public abstract record IntegrationEvent : Message, IIntegrationEvent;
