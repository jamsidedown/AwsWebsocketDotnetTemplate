namespace AwsWebsocketDotnetTemplate.Core;

public static class DynamoHelpers
{
    public static AttributeValue ToAttribute(this string s) => new() {S = s};
    public static AttributeValue ToAttribute(this DateTime dt) => new() {S = dt.ToString("s")};
    public static AttributeValue ToAttribute(this long l) => new() {N = l.ToString()};

    public static long ToUnixTime(this DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();
}