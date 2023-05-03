using ApiClient.Catalogs.Dtos;
using AutoMapper;
using Catalogs.ApiClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Shared.Core.Exceptions;
using Shared.Core.Extensions;
using Shared.Core.Paging;
using Shared.Web;

namespace ApiClient.Catalogs;

public class CatalogsService : ICatalogsService
{
    private readonly ICatalogsApiClient _catalogsApiClient;
    private readonly IMapper _mapper;
    private readonly AsyncPolicyWrap _combinedPolicy;

    public CatalogsService(ICatalogsApiClient catalogsApiClient, IOptions<PolicyOptions> policyOptions, IMapper mapper)
    {
        _catalogsApiClient = catalogsApiClient;
        _mapper = mapper;
        var retryPolicy = Policy.Handle<ApiException>().RetryAsync(policyOptions.Value.RetryCount);

        var timeoutPolicy = Policy.TimeoutAsync(policyOptions.Value.TimeOutDuration, TimeoutStrategy.Pessimistic);

        // at any given time there will 3 parallel requests execution for specific service call and another 6 requests for other services can be in the queue. So that if the response from customer service is delayed or blocked then we don’t use too many resources
        var bulkheadPolicy = Policy.BulkheadAsync(3, 6);

        var circuitBreakerPolicy = Policy
            .Handle<ApiException>()
            .CircuitBreakerAsync(
                policyOptions.Value.RetryCount + 1,
                TimeSpan.FromSeconds(policyOptions.Value.BreakDuration)
            );

        var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, bulkheadPolicy);

        _combinedPolicy = combinedPolicy.WrapAsync(timeoutPolicy);
    }

    public async Task<CreateProductOutput> CreateProductAsync(
        CreateProductInput createProductInput,
        CancellationToken cancellationToken
    )
    {
        createProductInput.NotBeNull();

        // https://github.com/App-vNext/Polly#post-execution-capturing-the-result-or-any-final-exception
        var policyResult = await _combinedPolicy.ExecuteAndCaptureAsync(async () =>
        {
            // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
            // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
            var response = await _catalogsApiClient.CreateProductAsync(
                new CreateProductRequest(
                    createProductInput.CategoryId,
                    createProductInput.Description,
                    createProductInput.Name,
                    (double)createProductInput.Price
                ),
                cancellationToken
            );
            return response;
        });

        switch (policyResult.Outcome)
        {
            case OutcomeType.Successful:
                return new CreateProductOutput(policyResult.Result.Id);
            default:
            {
                if (
                    policyResult is
                    { ExceptionType: ExceptionType.HandledByThisPolicy, FinalException: ApiException apiException }
                )
                {
                    throw new HttpResponseException(
                        apiException.StatusCode,
                        apiException.Response!,
                        apiException.Headers,
                        apiException
                    );
                }

                throw policyResult.FinalException;
            }
        }
    }

    public async Task<GetGetProductsByPageOutput> GetProductByPageAsync(
        GetGetProductsByPageInput getProductsByPageInput,
        CancellationToken cancellationToken
    )
    {
        getProductsByPageInput.NotBeNull();

        try
        {
            // https://github.com/App-vNext/Polly#post-execution-capturing-the-result-or-any-final-exception
            var response = await _combinedPolicy.ExecuteAsync(async () =>
            {
                // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
                // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
                return await _catalogsApiClient.GetProductsByPageAsync(
                    getProductsByPageInput.PageSize,
                    getProductsByPageInput.PageNumber,
                    getProductsByPageInput.Filters,
                    getProductsByPageInput.SortOrder,
                    cancellationToken
                );
            });

            var items = _mapper.Map<List<Dtos.ProductDto>>(response.Products!.Items);
            return new GetGetProductsByPageOutput(
                PageList<Dtos.ProductDto>.Create(
                    items,
                    response.Products.PageNumber,
                    response.Products.PageSize,
                    response.Products.TotalCount
                )
            );
        }
        catch (ApiException apiException)
        {
            throw new HttpResponseException(
                apiException.StatusCode,
                apiException.Response!,
                apiException.Headers,
                apiException
            );
        }
    }

    public async Task<GetProductByIdOutput> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        id.NotBeNull();

        try
        {
            // https://github.com/App-vNext/Polly#post-execution-capturing-the-result-or-any-final-exception
            var result = await _combinedPolicy.ExecuteAsync(async () =>
            {
                // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
                // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
                return await _catalogsApiClient.GetProductByIdAsync(id, cancellationToken);
            });

            return new GetProductByIdOutput(new Dtos.ProductLiteDto(result.Product.Id, result.Product.Name));
        }
        catch (ApiException apiException)
        {
            throw new HttpResponseException(
                apiException.StatusCode,
                apiException.Response!,
                apiException.Headers,
                apiException
            );
        }
    }
}
