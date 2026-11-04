using Microsoft.AspNetCore.Http;
using RealtorApi.Infrastructure.Results;

namespace RealtorApiTests.Infrastructure.Results;

public class ResultProblemDetailsMapperTests
{
    [Fact]
    public void MapeaErrorResultAProblemDetails_ConStatusYPayloadCorrectos()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/properties/42";

        var error = new ResultError(
            Code: "not_found",
            Title: "No encontrado",
            Status: StatusCodes.Status404NotFound,
            Detail: "No existe el recurso",
            Extensions: new Dictionary<string, object?>
            {
                ["code"] = "not_found"
            });

        var problemDetails = ResultProblemDetailsMapper.ToProblemDetails(error, context);

        Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("No encontrado", problemDetails.Title);
        Assert.Equal("No existe el recurso", problemDetails.Detail);
        Assert.Equal("/properties/42", problemDetails.Instance);
        Assert.Equal("not_found", problemDetails.Extensions["code"]);
    }
}
