using System.Net;
using Amazon.Lambda.APIGatewayEvents;

namespace AwsWebsocketDotnetTemplate.Core;

public static class ResponseHelpers
{
    public static APIGatewayProxyResponse Ok() => new APIGatewayProxyResponse {StatusCode = (int)HttpStatusCode.OK};
    public static APIGatewayProxyResponse BadRequest() => new APIGatewayProxyResponse {StatusCode = (int)HttpStatusCode.BadRequest};
    public static APIGatewayProxyResponse InternalError() => new APIGatewayProxyResponse {StatusCode = (int)HttpStatusCode.InternalServerError};
}