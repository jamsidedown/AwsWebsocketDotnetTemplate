using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Net;

namespace AwsWebsocketDotnetTemplate.Tests.Functions;

public class ConnectTests
{
    private Mock<ILambdaLogger> _mockLogger;
    private Mock<IAmazonDynamoDB> _mockDynamo;

    private Connect SetupLambda()
    {
        _mockLogger = new Mock<ILambdaLogger>();
        _mockDynamo = new Mock<IAmazonDynamoDB>();
        return new Connect(_mockLogger.Object, _mockDynamo.Object);
    }

    private APIGatewayProxyRequest GetRequest(string connectionId) =>
        new()
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext {ConnectionId = connectionId}
        };

    [Fact]
    public async Task ShouldReturnOkWhenConnectionIdIsPresent()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.Is<Dictionary<string, AttributeValue>>(x => x["ConnectionId"].S == "123456"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutItemResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ShouldLogConnectionIdWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.Is<Dictionary<string, AttributeValue>>(x => x["ConnectionId"].S == "123456"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutItemResponse {HttpStatusCode = HttpStatusCode.OK});

        var request = GetRequest("123456");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogInformation("Connected: 123456"), Times.Once);
    }

    [Fact]
    public async Task ShouldNotLogAnyErrorsWhenSuccessful()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, AttributeValue>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutItemResponse {HttpStatusCode = HttpStatusCode.OK});

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
    public async Task ShouldStoreToDynamoOnConnect()
    {
        var lambda = SetupLambda();

        var dynamoResponse = new PutItemResponse {HttpStatusCode = HttpStatusCode.OK};
        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.Is<Dictionary<string, AttributeValue>>(x => x["ConnectionId"].S == "123456"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(dynamoResponse);

        var request = GetRequest("123456");

        await lambda.Handler(request);

        _mockDynamo
            .Verify(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.Is<Dictionary<string, AttributeValue>>(d => d["ConnectionId"].S == "123456"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenDynamoFails()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, AttributeValue>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutItemResponse {HttpStatusCode = HttpStatusCode.BadRequest});

        var request = GetRequest("123456");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenDynamoThrowsException()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, AttributeValue>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var request = GetRequest("123456");

        var response = await lambda.Handler(request);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldLogErrorWhenDynamoThrowsException()
    {
        var lambda = SetupLambda();

        _mockDynamo
            .Setup(m => m.PutItemAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, AttributeValue>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var request = GetRequest("123456");

        await lambda.Handler(request);

        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.AtLeast(2));
        _mockLogger.Verify(logger => logger.LogError("Failed to store connection"), Times.Once);
    }
}
