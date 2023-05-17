using AwsWebsocketDotnetTemplate.Core;

namespace AwsWebsocketDotnetTemplate.Models;

public class ConnectionModel
{
    public string ConnectionId { get; set; }
    public DateTime ConnectedAt { get; set; }

    public ConnectionModel(string connectionId)
    {
        ConnectionId = connectionId;
        ConnectedAt = DateTime.Now;
    }

    public Dictionary<string, AttributeValue> ToDynamo()
    {
        var connectionId = ConnectionId.ToAttribute();
        var ttl = ConnectedAt.AddDays(1).ToUnixTime();

        return new Dictionary<string, AttributeValue>
        {
            {"Pk", connectionId},
            {"Sk", connectionId},
            {"ConnectionId", connectionId},
            {"ConnectedAt", ConnectedAt.ToAttribute()},
            {"Ttl", ttl.ToAttribute()}
        };
    }

    public static Dictionary<string, AttributeValue> Key(string connectionId)
    {
        var attribute = connectionId.ToAttribute();

        return new Dictionary<string, AttributeValue>
        {
            {"Pk", attribute},
            {"Sk", attribute},
        };
    }
}
