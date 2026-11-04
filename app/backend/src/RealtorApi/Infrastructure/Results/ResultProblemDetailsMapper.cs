using Microsoft.AspNetCore.Mvc;

namespace RealtorApi.Infrastructure.Results;

public static class ResultProblemDetailsMapper
{
    public static ProblemDetails ToProblemDetails(ResultError error, HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Title,
            Detail = error.Detail,
            Status = error.Status,
            Type = $"https://httpstatuses.com/{error.Status}",
            Instance = httpContext.Request.Path
        };

        if (error.Extensions is null)
        {
            return problemDetails;
        }

        foreach (var extension in error.Extensions)
        {
            problemDetails.Extensions[extension.Key] = extension.Value;
        }

        return problemDetails;
    }
}
