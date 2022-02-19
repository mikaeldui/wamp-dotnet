namespace System.Net.WebSockets.Wamp;

/// <summary>
///     Provides a base exception for WAMP. It does NOT inherit from the sealed type <see cref="WebSocketException" />.
/// </summary>
public class WampResponseException : Exception
{
    public WampResponseException(string message) : base(message)
    {
    }

    public WampResponseException(string message, string webSocketMessage) : this(message)
    {
        WebSocketMessage = webSocketMessage;
    }

    public string? WebSocketMessage { get; }
}