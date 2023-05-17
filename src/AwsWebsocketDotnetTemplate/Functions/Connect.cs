using Amazon.Lambda.APIGatewayEvents;
using AwsWebsocketDotnetTemplate.Core;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace AwsWebsocketDotnetTemplate.Functions;

public class Connect
{
    public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest request)
    {
        var connectionId = request.RequestContext.ConnectionId;

        if (string.IsNullOrEmpty(connectionId))
            return ResponseHelpers.BadRequest();

        return ResponseHelpers.Ok();
    }
}
