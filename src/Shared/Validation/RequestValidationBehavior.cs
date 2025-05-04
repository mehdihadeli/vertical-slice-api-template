using System.Text.Json;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Validation.Extensions;

namespace Shared.Validation;

/// <summary>
/// A MediatR pipeline behavior that automatically validates a request using FluentValidation
/// before invoking its handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request message.</typeparam>
/// <typeparam name="TResponse">The type of the response message returned by the handler.</typeparam>
public class RequestValidationBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider,
    ILogger<RequestValidationBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    /// <summary>
    /// Handles the incoming request by validating it (if a validator is registered)
    /// and then invoking the next handler in the pipeline.
    /// </summary>
    /// <param name="message">The request message to be handled.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <param name="next">The delegate representing the next step in the pipeline.</param>
    /// <returns>The response returned by the request handler.</returns>
    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next
    )
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>()!;
        if (validator is null)
            return await next(message, cancellationToken);

        logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        logger.LogDebug(
            "Handling {FullName} with content {Request}",
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(message)
        );

        await validator.HandleValidationAsync(message, cancellationToken);

        var response = await next(message, cancellationToken);

        logger.LogInformation("Handled {FullName}", typeof(TRequest).FullName);
        return response;
    }
}

/// <summary>
/// A MediatR stream pipeline behavior that automatically validates a streaming request
/// using FluentValidation before yielding responses from its handler.
/// </summary>
/// <typeparam name="TRequest">The type of the streaming request message.</typeparam>
/// <typeparam name="TResponse">The type of each response element yielded by the handler.</typeparam>
public class StreamRequestValidationBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider,
    ILogger<StreamRequestValidationBehavior<TRequest, TResponse>> logger
) : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<StreamRequestValidationBehavior<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// Handles the incoming streaming request by validating it (if a validator is registered)
    /// and then invoking the next handler in the pipeline to yield responses.
    /// </summary>
    /// <param name="message">The streaming request message to be handled.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <param name="next">The delegate representing the next step in the streaming pipeline.</param>
    /// <returns>An asynchronous stream of responses yielded by the request handler.</returns>
    public async IAsyncEnumerable<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        StreamHandlerDelegate<TRequest, TResponse> next
    )
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>()!;

        if (validator is null)
        {
            await foreach (var response in next(message, cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        _logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(StreamRequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        _logger.LogDebug(
            "Handling {FullName} with content {Request}",
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(message)
        );

        await validator.HandleValidationAsync(message, cancellationToken: cancellationToken);

        await foreach (var response in next(message, cancellationToken))
        {
            yield return response;
            _logger.LogInformation("Handled {FullName}", typeof(TRequest).FullName);
        }
    }
}
