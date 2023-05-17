namespace AwsWebsocketDotnetTemplate.Core;

public class Logger : ILambdaLogger
{
    public void Log(string message) => LambdaLogger.Log(message);

    public void LogLine(string message) => LambdaLogger.Log(message);
}