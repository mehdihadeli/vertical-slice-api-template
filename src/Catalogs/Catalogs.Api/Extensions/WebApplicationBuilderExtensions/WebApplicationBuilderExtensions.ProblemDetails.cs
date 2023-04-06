using Microsoft.AspNetCore.Diagnostics;

namespace ECommerce.Services.Catalogs.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(x =>
        {
            // customization problem details should go here
            x.CustomizeProblemDetails = ctx =>
            {
                if (ctx.HttpContext.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    var exceptionHandlerFeature = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>();
                }

                var problemCorrelationId = Guid.NewGuid().ToString();
                // log problemCorrelationId into logging system
                ctx.ProblemDetails.Instance = problemCorrelationId;
            };
            // x.Map<ConflictException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = StatusCodes.Status409Conflict,
            //             Detail = ex.Message,
            //             Type = "https://somedomain/application-rule-validation-error"
            //         }
            // );
            //
            // // Exception will produce and returns from our FluentValidation RequestValidationBehavior
            // x.Map<ValidationException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = StatusCodes.Status400BadRequest,
            //             Detail = JsonConvert.SerializeObject(ex.ValidationResultModel.Errors),
            //             Type = "https://somedomain/input-validation-rules-error"
            //         }
            // );
            // x.Map<ArgumentException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = StatusCodes.Status400BadRequest,
            //             Detail = ex.Message,
            //             Type = "https://somedomain/argument-error"
            //         }
            // );
            // x.Map<BadRequestException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = StatusCodes.Status400BadRequest,
            //             Detail = ex.Message,
            //             Type = "https://somedomain/bad-request-error"
            //         }
            // );
            // x.Map<NotFoundException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = (int)ex.StatusCode,
            //             Detail = ex.Message,
            //             Type = "https://somedomain/not-found-error"
            //         }
            // );
            // x.Map<AppException>(
            //     ex =>
            //         new ProblemDetails
            //         {
            //             Title = ex.GetType().Name,
            //             Status = (int)ex.StatusCode,
            //             Detail = ex.Message,
            //             Type = "https://somedomain/application-error"
            //         }
            // );
            // x.Map<HttpResponseException>(ex =>
            // {
            //     var pd = new ProblemDetails
            //     {
            //         Status = (int?)ex.StatusCode,
            //         Title = ex.GetType().Name,
            //         Detail = ex.Message,
            //         Type = "https://somedomain/http-error"
            //     };
            //
            //     return pd;
            // });
            // x.Map<HttpRequestException>(ex =>
            // {
            //     var pd = new ProblemDetails
            //     {
            //         Status = (int?)ex.StatusCode,
            //         Title = ex.GetType().Name,
            //         Detail = ex.Message,
            //         Type = "https://somedomain/http-error"
            //     };
            //
            //     return pd;
            // });
            //
            // x.MapToStatusCode<ArgumentNullException>(StatusCodes.Status400BadRequest);
            // x.MapStatusCode = context => new StatusCodeProblemDetails(context.Response.StatusCode);
        });

        return builder;
    }
}
