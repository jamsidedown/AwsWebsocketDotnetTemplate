using Amazon.Lambda.APIGatewayEvents;

namespace AwsWebsocketDotnetTemplate.Tests.Functions;

public class ConnectTests
{
    private Connect _lambda;

    public ConnectTests()
    {
        _lambda = new Connect();
    }

    [Fact]
    public async Task ShouldReturnOkWhenConnectionIdIsPresent()
    {
        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        var response = await _lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenConnectionIdIsEmpty()
    {
        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = string.Empty
            }
        };

        var response = await _lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
}
