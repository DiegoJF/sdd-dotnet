namespace RealtorApi.Infrastructure.Results;

public static class ResultHttpExtensions
{
    public static IResult ToIResult(this Result result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return TypedResults.NoContent();
        }

        ArgumentNullException.ThrowIfNull(result.Error);

        var problemDetails = ResultProblemDetailsMapper.ToProblemDetails(result.Error, httpContext);
        return TypedResults.Problem(problemDetails);
    }

    public static IResult ToIResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        ArgumentNullException.ThrowIfNull(result.Error);

        var problemDetails = ResultProblemDetailsMapper.ToProblemDetails(result.Error, httpContext);
        return TypedResults.Problem(problemDetails);
    }
}
