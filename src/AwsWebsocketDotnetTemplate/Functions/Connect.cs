using System.Net;
using AwsWebsocketDotnetTemplate.Core;
using AwsWebsocketDotnetTemplate.Models;

namespace AwsWebsocketDotnetTemplate.Functions;

public class Connect
{
    private readonly ILambdaLogger _logger;
    private readonly IAmazonDynamoDB _dynamo;

    public Connect()
    {
        _logger = new Logger();
        _dynamo = new AmazonDynamoDBClient();
    }

    public Connect(ILambdaLogger logger, IAmazonDynamoDB dynamo)
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

        var connection = new ConnectionModel(connectionId);
        var success = await StoreConnection(connection);

        if (!success)
        {
            _logger.LogError("Failed to store connection");
            return ResponseHelpers.InternalError();
        }

        _logger.LogInformation($"Connected: {connectionId}");

        return ResponseHelpers.Ok();
    }

    private async Task<bool> StoreConnection(ConnectionModel connection)
    {
        try
        {
            var response = await _dynamo.PutItemAsync(Definitions.ConnectionsTable, connection.ToDynamo());
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
}
