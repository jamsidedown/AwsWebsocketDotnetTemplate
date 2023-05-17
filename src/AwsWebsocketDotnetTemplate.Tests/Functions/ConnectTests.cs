using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace AwsWebsocketDotnetTemplate.Tests.Functions;

public class ConnectTests
{
    [Fact]
    public async Task ShouldReturnOkWhenConnectionIdIsPresent()
    {
        var mockLogger = new Mock<ILambdaLogger>();
        var lambda = new Connect(mockLogger.Object);

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
        var mockLogger = new Mock<ILambdaLogger>();
        var lambda = new Connect(mockLogger.Object);

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        await lambda.Handler(request);

        mockLogger.Verify(logger => logger.LogInformation("Connected: 123456"), Times.Once);
    }

    [Fact]
    public async Task ShouldNotLogAnyErrorsWhenSuccessful()
    {
        var mockLogger = new Mock<ILambdaLogger>();
        var lambda = new Connect(mockLogger.Object);

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = "123456"
            }
        };

        await lambda.Handler(request);

        mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenConnectionIdIsEmpty()
    {
        var mockLogger = new Mock<ILambdaLogger>();
        var lambda = new Connect(mockLogger.Object);

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
        var mockLogger = new Mock<ILambdaLogger>();
        var lambda = new Connect(mockLogger.Object);

        var request = new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = string.Empty
            }
        };

        await lambda.Handler(request);

        mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        mockLogger.Verify(logger => logger.LogError("Empty connection id"), Times.Once);
    }
}
