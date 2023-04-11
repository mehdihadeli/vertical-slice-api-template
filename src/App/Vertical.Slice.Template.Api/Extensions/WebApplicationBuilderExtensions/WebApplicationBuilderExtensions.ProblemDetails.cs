using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Core.Exceptions;
using Shared.Validation.Extensions;
using Shared.Web.ProblemDetail;

namespace Vertical.Slice.Template.Api.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomProblemDetails(problemDetailsOptions =>
        {
            // customization problem details should go here
            problemDetailsOptions.CustomizeProblemDetails = problemDetailContext =>
            {
                // with help of capture exception middleware for capturing actual exception
                if (problemDetailContext.HttpContext.Features.Get<IExceptionHandlerFeature>() is { } exceptionFeature)
                {
                    // switch (exceptionFeature.Error)
                    // {
                    //     case ConflictException conflictException:
                    //         problemDetailContext.ProblemDetails.Status = conflictException.StatusCode;
                    //         break;
                    //     case ValidationException validationException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             validationException.ValidationResultModel.StatusCode,
                    //             validationException
                    //         );
                    //         break;
                    //     case ArgumentException argumentException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             StatusCodes.Status400BadRequest,
                    //             argumentException
                    //         );
                    //         break;
                    //     case BadRequestException badRequestException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             badRequestException.StatusCode,
                    //             badRequestException
                    //         );
                    //         break;
                    //     case NotFoundException notFoundException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             notFoundException.StatusCode,
                    //             notFoundException
                    //         );
                    //         break;
                    //     case HttpResponseException httpResponseException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             httpResponseException.StatusCode,
                    //             httpResponseException
                    //         );
                    //         break;
                    //     case HttpRequestException httpRequestException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             (int)httpRequestException.StatusCode,
                    //             httpRequestException
                    //         );
                    //         break;
                    //     case AppException appException:
                    //         PopulateNewProblemDetail(
                    //             problemDetailContext.ProblemDetails,
                    //             appException.StatusCode,
                    //             appException
                    //         );
                    //         break;
                    // }
                }
            };
        });

        return builder;
    }

    // private static void PopulateNewProblemDetail(
    //     ProblemDetails existingProblemDetails,
    //     int statusCode,
    //     Exception exception
    // )
    // {
    //     var newProblemDetail = TypedResults
    //         .Problem(
    //             new ProblemDetails
    //             {
    //                 Title = exception.GetType().Name,
    //                 Status = statusCode,
    //                 Detail = exception.Message
    //             }
    //         )
    //         .ProblemDetails;
    //
    //     existingProblemDetails.Type = newProblemDetail.Type;
    //     existingProblemDetails.Title = newProblemDetail.Title;
    //     existingProblemDetails.Detail = newProblemDetail.Detail;
    //     existingProblemDetails.Status = newProblemDetail.Status;
    // }
}
