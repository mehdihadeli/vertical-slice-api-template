// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.Item;
using Vertical.Slice.Template.ContractTests.KiotaApiClient.Models;
namespace Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products
{
    /// <summary>
    /// Builds and executes requests for operations under \api\v1\products
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class ProductsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Vertical.Slice.Template.ContractTests.KiotaApiClient.api.v1.products.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.Item.ProductsItemRequestBuilder"/></returns>
        public global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.Item.ProductsItemRequestBuilder this[Guid position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("id", position);
                return new global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.Item.ProductsItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ProductsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/products{?Filters*,PageNumber*,PageSize*,SortOrder*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ProductsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/products{?Filters*,PageNumber*,PageSize*,SortOrder*}", rawUrl)
        {
        }
        /// <summary>
        /// Get products by page
        /// </summary>
        /// <returns>A <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.GetProductsByPageResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.HttpValidationProblemDetails">When receiving a 400 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.GetProductsByPageResponse?> GetAsync(Action<RequestConfiguration<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.GetProductsByPageResponse> GetAsync(Action<RequestConfiguration<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "400", global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.HttpValidationProblemDetails.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.GetProductsByPageResponse>(requestInfo, global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.GetProductsByPageResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Create product
        /// </summary>
        /// <returns>A <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductResponse"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.HttpValidationProblemDetails">When receiving a 400 status code</exception>
        /// <exception cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.ProblemDetails">When receiving a 401 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductResponse?> PostAsync(global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductRequest body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductResponse> PostAsync(global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductRequest body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "400", global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.HttpValidationProblemDetails.CreateFromDiscriminatorValue },
                { "401", global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.ProblemDetails.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductResponse>(requestInfo, global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Get products by page
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Create product
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductRequest body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.CreateProductRequest body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Api.V1.Products.ProductsRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Get products by page
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class ProductsRequestBuilderGetQueryParameters 
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public string? Filters { get; set; }
#nullable restore
#else
            public string Filters { get; set; }
#endif
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public string? SortOrder { get; set; }
#nullable restore
#else
            public string SortOrder { get; set; }
#endif
        }
    }
}
#pragma warning restore CS0618