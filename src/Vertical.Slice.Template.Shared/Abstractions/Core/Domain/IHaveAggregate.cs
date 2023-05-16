using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.Domain;

public interface IHaveAggregate : IHaveDomainEvents, IHaveAggregateVersion { }
