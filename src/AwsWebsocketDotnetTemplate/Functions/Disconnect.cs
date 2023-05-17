using System.Net;
using AwsWebsocketDotnetTemplate.Core;
using AwsWebsocketDotnetTemplate.Models;

namespace AwsWebsocketDotnetTemplate.Functions;

public class Disconnect
{
    private readonly ILambdaLogger _logger;
    private readonly IAmazonDynamoDB _dynamo;

    public Disconnect()
    {
        _logger = new Logger();
        _dynamo = new AmazonDynamoDBClient();
    }

    public Disconnect(ILambdaLogger logger, IAmazonDynamoDB dynamo)
    {
        _logger = logger;
        _dynamo = dynamo;
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

        var success = await DeleteConnection(connectionId);

        if (!success)
        {
            _logger.LogError("Failed to delete connection");
            return ResponseHelpers.InternalError();
        }

        _logger.LogInformation($"Disconnected: {connectionId}");

        return ResponseHelpers.Ok();
    }

    private async Task<bool> DeleteConnection(string connectionId)
    {
        try
        {
            var response = await _dynamo.DeleteItemAsync(Definitions.ConnectionsTable, ConnectionModel.Key(connectionId));
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
}
