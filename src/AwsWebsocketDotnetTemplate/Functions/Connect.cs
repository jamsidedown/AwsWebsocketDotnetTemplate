using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using AwsWebsocketDotnetTemplate.Core;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace AwsWebsocketDotnetTemplate.Functions;

public class Connect
{
    private readonly ILambdaLogger _logger;

    public Connect()
    {
        _logger = new Logger();
    }

    public Connect(ILambdaLogger logger)
    {
        _logger = logger;
    }

    public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest request)
    {
        var connectionId = request.RequestContext.ConnectionId;

        if (string.IsNullOrEmpty(connectionId))
        {
            _logger.LogError("Empty connection id");
            _logger.LogError(JsonSerializer.Serialize(request));
            return ResponseHelpers.BadRequest();
        }

        _logger.LogInformation($"Connected: {connectionId}");

        return ResponseHelpers.Ok();
    }
}
