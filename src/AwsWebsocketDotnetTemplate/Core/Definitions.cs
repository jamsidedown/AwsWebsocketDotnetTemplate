namespace AwsWebsocketDotnetTemplate.Core;

public static class Definitions
{
    private const string ConnectionsTableVariable = "CONNECTIONS_TABLE";

    public static string ConnectionsTable => Environment.GetEnvironmentVariable(ConnectionsTableVariable) ?? string.Empty;
}