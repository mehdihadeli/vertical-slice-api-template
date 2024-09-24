using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Abstractions.Persistence.Ef;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shared.EF;

// Ref: https://github.com/thangchung/clean-architecture-dotnet/blob/main/src/N8T.Infrastructure.EfCore/TxBehavior.cs
public class EfTxBehavior<TRequest, TResponse>(
    IDbFacadeResolver dbFacadeResolver,
    ILogger<EfTxBehavior<TRequest, TResponse>> logger,
    IDomainEventPublisher domainEventPublisher,
    IDomainEventContext domainEventContext
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : notnull
{
    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message is not ITxRequest)
            return await next(message, cancellationToken);

        logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName
        );

        logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(message)
        );

        logger.LogInformation(
            "{Prefix} Open the transaction for {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName
        );

        var strategy = dbFacadeResolver.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            // https://www.thinktecture.com/en/entity-framework-core/use-transactionscope-with-caution-in-2-1/
            // https://github.com/dotnet/efcore/issues/6233#issuecomment-242693262
            var isInnerTransaction = dbFacadeResolver.Database.CurrentTransaction is not null;

            var transaction =
                dbFacadeResolver.Database.CurrentTransaction
                ?? await dbFacadeResolver.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next(message, cancellationToken);

                logger.LogInformation(
                    "{Prefix} Executed the {MediatrRequest} request",
                    nameof(EfTxBehavior<TRequest, TResponse>),
                    typeof(TRequest).FullName
                );

                var domainEvents = domainEventContext.GetAllUncommittedEvents();

                await domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

                if (isInnerTransaction == false)
                    await transaction.CommitAsync(cancellationToken);

                domainEventContext.MarkUncommittedDomainEventAsCommitted();

                return response;
            }
            catch
            {
                if (isInnerTransaction == false)
                    await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        });
    }
}
