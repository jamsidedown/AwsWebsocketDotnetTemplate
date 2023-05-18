using System.Net;
using System.Text;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace AwsWebsocketDotnetTemplate.Tests.Functions;

public class DefaultTests
{
    private Mock<ILambdaLogger> _mockLogger;
    private Mock<IAmazonApiGatewayManagementApi> _mockApiGateway;

    private Default SetupLambda()
    {
        _mockLogger = new Mock<ILambdaLogger>();
        _mockApiGateway = new Mock<IAmazonApiGatewayManagementApi>();
        return new Default(_mockLogger.Object, _mockApiGateway.Object);
    }

    private APIGatewayProxyRequest GetRequest(string connectionId, string message = "") =>
        new()
        {
            Body = message,
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext {ConnectionId = connectionId}
        };

    [Fact]
    public async Task ShouldReturnOkWhenConnectionIdIsPresent()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ShouldLogConnectionIdWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogInformation("Responded to: 123456"), Times.Once);
    }

    [Fact]
    public async Task ShouldLogMessageWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456", "Hello");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogInformation("Message: Hello"), Times.Once);
    }

    [Fact]
    public async Task ShouldNotLogAnyErrorsWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenConnectionIdIsEmpty()
    {
        var lambda = SetupLambda();

        var request = GetRequest(string.Empty);

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ShouldLogErrorsWhenConnectionIdIsEmpty()
    {
        var lambda = SetupLambda();

        var request = GetRequest(string.Empty);

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.AtLeast(2));
        _mockLogger.Verify(logger => logger.LogError("Empty connection id"), Times.Once);
    }

    [Fact]
    public async Task ShouldSendMessageWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456", "Hello");

        await lambda.Handler(request);

        _mockApiGateway
            .Verify(apiGateway => apiGateway.PostToConnectionAsync(
                It.Is<PostToConnectionRequest>(r => r.ConnectionId == "123456" && Encoding.UTF8.GetString(r.Data.ToArray()) == "Hello"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenSendMessageFails()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PostToConnectionResponse {HttpStatusCode = HttpStatusCode.InternalServerError});

        var request = GetRequest("123456", "Hello");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenSendMessageThrows()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var request = GetRequest("123456", "Hello");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldLogErrorWhenSendMessageThrows()
    {
        var lambda = SetupLambda();

        _mockApiGateway
            .Setup(m => m.PostToConnectionAsync(
                It.IsAny<PostToConnectionRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var request = GetRequest("123456", "Hello");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.AtLeast(2));
        _mockLogger.Verify(logger => logger.LogError("Failed to send message"), Times.Once);
    }
}