using Amazon.Lambda.APIGatewayEvents;

namespace AwsWebsocketDotnetTemplate.Core;

public class ResponseHelpers
{
    public static class StatusCodes
    {
        public const int Ok  = 200;
        public const int BadRequest = 400;
        public const int InternalError = 500;
    }

    public static APIGatewayProxyResponse Ok() => new APIGatewayProxyResponse {StatusCode = StatusCodes.Ok};
    public static APIGatewayProxyResponse BadRequest() => new APIGatewayProxyResponse {StatusCode = StatusCodes.BadRequest};
    public static APIGatewayProxyResponse InternalError() => new APIGatewayProxyResponse {StatusCode = StatusCodes.InternalError};
}