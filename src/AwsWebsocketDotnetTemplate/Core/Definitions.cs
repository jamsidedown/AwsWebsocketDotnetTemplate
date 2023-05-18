namespace AwsWebsocketDotnetTemplate.Core;

public static class Definitions
{
    private const string ConnectionsTableVariable = "CONNECTIONS_TABLE";
    private const string ConnectionsEndpointVariable = "CONNECTIONS_ENDPOINT";

    public static string ConnectionsTable => Environment.GetEnvironmentVariable(ConnectionsTableVariable) ?? string.Empty;
    public static string ConnectionsEndpoint => Environment.GetEnvironmentVariable(ConnectionsEndpointVariable) ?? string.Empty;
}