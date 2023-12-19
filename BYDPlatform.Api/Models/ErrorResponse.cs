using System.Net;
using System.Text.Json;

namespace BYDPlatform.Api.Models;

public class ErrorResponse
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    public string Message { get; set; } = "An unexpected error occured";
    public string ToJsonString() => JsonSerializer.Serialize(this);
}