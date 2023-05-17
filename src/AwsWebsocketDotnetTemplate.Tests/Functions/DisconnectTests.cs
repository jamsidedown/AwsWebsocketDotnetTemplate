using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace AwsWebsocketDotnetTemplate.Tests.Functions;

public class DisconnectTests
{
    private Mock<ILambdaLogger> _mockLogger;

    private Disconnect SetupLambda()
    {
        _mockLogger = new Mock<ILambdaLogger>();
        return new Disconnect(_mockLogger.Object);
    }

    [Fact]
    public async Task ShouldReturnOkWhenConnectionIdIsPresent()
    {
        var lambda = SetupLambda();

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ShouldLogConnectionIdWhenSuccessful()
    {
        var lambda = SetupLambda();

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogInformation("Disconnected: 123456"), Times.Once);
    }

    [Fact]
    public async Task ShouldNotLogAnyErrorsWhenSuccessful()
    {
        var lambda = SetupLambda();

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenConnectionIdIsEmpty()
    {
        var lambda = SetupLambda();

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = string.Empty
            }
        };

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ShouldLogErrorsWhenConnectionIdIsEmpty()
    {
        var lambda = SetupLambda();

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = string.Empty
            }
        };

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.AtLeast(2));
        _mockLogger.Verify(logger => logger.LogError("Empty connection id"), Times.Once);
    }
}