using AwsWebsocketDotnetTemplate.Core;

namespace AwsWebsocketDotnetTemplate.Functions;

public class Disconnect
{
    private readonly ILambdaLogger _logger;

    public Disconnect()
    {
        _logger = new Logger();
    }

    public Disconnect(ILambdaLogger logger)
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

        _logger.LogInformation($"Disconnected: {connectionId}");

        return ResponseHelpers.Ok();
    }
}