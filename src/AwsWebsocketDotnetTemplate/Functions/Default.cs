using System.Net;
using System.Text;
using Amazon.ApiGatewayManagementApi.Model;
using AwsWebsocketDotnetTemplate.Core;

namespace AwsWebsocketDotnetTemplate.Functions;

public class Default
{
    private readonly ILambdaLogger _logger;
    private readonly IAmazonApiGatewayManagementApi _apiGateway;

    public Default()
    {
        _logger = new Logger();
        _apiGateway = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig {ServiceURL = Definitions.ConnectionsEndpoint});
    }

    public Default(ILambdaLogger logger, IAmazonApiGatewayManagementApi apiGateway)
    {
        _logger = logger;
        _apiGateway = apiGateway;
    }

    public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest request)
    {
        var connectionId = request.RequestContext.ConnectionId;
        var message = request.Body;

        if (string.IsNullOrEmpty(connectionId))
        {
            _logger.LogError("Empty connection id");
            _logger.LogError(JsonSerializer.Serialize(request));
            return ResponseHelpers.BadRequest();
        }

        _logger.LogInformation($"Message: {message}");

        var success = await Send(connectionId, message);

        if (!success)
        {
            _logger.LogError("Failed to send message");
            return ResponseHelpers.InternalError();
        }

        _logger.LogInformation($"Responded to: {connectionId}");

        return ResponseHelpers.Ok();
    }

    private async Task<bool> Send(string connectionId, string message)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            using var stream = new MemoryStream(bytes);
            var response = await _apiGateway.PostToConnectionAsync(new PostToConnectionRequest {ConnectionId = connectionId, Data = stream});
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
}