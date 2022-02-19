using System.ComponentModel;
using System.Text.Json;

// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets.Wamp;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWampRoleClient : IWampRole
{
    Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default);
    Task ConnectAsync(string uri, CancellationToken cancellationToken = default);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWampRoleClient<out TMessageTypeCodes> : IWampRoleClient, IWampRole<TMessageTypeCodes>
    where TMessageTypeCodes : WampMessageTypeCodes
{
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWampRoleClient<out TMessageTypeCodes, TMessageTypeCodeEnum> :
    IWampRoleClient<TMessageTypeCodes>,
    IWampRole<TMessageTypeCodes, TMessageTypeCodeEnum>
    where TMessageTypeCodes : WampMessageTypeCodes
    where TMessageTypeCodeEnum : struct, Enum
{
}

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WampRoleClientBase<TMessageTypeCodes> :
    WampRoleBase<TMessageTypeCodes>, IWampRoleClient<TMessageTypeCodes>
    where TMessageTypeCodes : WampMessageTypeCodes
{
    private Action<ClientWebSocketOptions>? _useOptions;

    protected internal WampRoleClientBase(TMessageTypeCodes messageCodes) : base(new ClientWebSocket(), messageCodes)
    {
        WebSocket.Options.SetRequestHeader("User-Agent", WampRoleClientUserAgent.USER_AGENT);
    }

    protected new ClientWebSocket WebSocket => (ClientWebSocket) base.WebSocket;

    [Obsolete(
        "Use client.UseClientWebSocketOptions(options => {}) instead, since the ClientWebSocket is being recreated on disconnects.",
        true)]
    public ClientWebSocketOptions Options => WebSocket.Options;

    public virtual async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        if (WebSocket.State != WebSocketState.None)
        {
            WebSocket.Dispose();
            base.WebSocket = new ClientWebSocket();
        }

        _useOptions?.Invoke(WebSocket.Options);

        await WebSocket.ConnectAsync(uri, cancellationToken);
    }

    public virtual async Task ConnectAsync(string uri, CancellationToken cancellationToken = default)
    {
        if (WebSocket.State != WebSocketState.None)
        {
            WebSocket.Dispose();
            base.WebSocket = new ClientWebSocket();
        }

        _useOptions?.Invoke(WebSocket.Options);

        await WebSocket.ConnectAsync(uri, cancellationToken);
    }

    /// <summary>
    ///     The options are only applied before connect.
    /// </summary>
    public void UseClientWebSocketOptions(Action<ClientWebSocketOptions> callback)
    {
        _useOptions = callback;
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WampRoleClientBase<TMessageTypeCodes, TMessageTypeCodeEnum> :
    WampRoleClientBase<TMessageTypeCodes>, IWampRoleClient<TMessageTypeCodes, TMessageTypeCodeEnum>
    where TMessageTypeCodes : WampMessageTypeCodes
    where TMessageTypeCodeEnum : struct, Enum
{
    protected internal WampRoleClientBase(TMessageTypeCodes messageCodes) : base(messageCodes)
    {
    }

    public virtual async Task SendAsync(WampMessage<TMessageTypeCodeEnum> message,
        CancellationToken cancellationToken = default)
    {
        await base.SendAsync(message, cancellationToken);
    }

    public new async Task<WampMessage<TMessageTypeCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        var array = await ReceiveJsonArrayAsync(cancellationToken);
        return OnMessageReceived(
            (TMessageTypeCodeEnum) Enum.ToObject(typeof(TMessageTypeCodeEnum), array[0].GetUInt16()),
            array.Skip(1).ToArray());
    }

    async Task<WampMessage> IWampRole.ReceiveAsync(CancellationToken cancellationToken)
    {
        return await base.ReceiveAsync(cancellationToken);
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the other OnMessageReceived. This method is NOT called in this class.", true)]
    protected override WampMessage OnMessageReceived(ushort messageCode, JsonElement[] elements)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    ///     Override this method to do custom logic on messages received.
    /// </summary>
    protected virtual WampMessage<TMessageTypeCodeEnum> OnMessageReceived(TMessageTypeCodeEnum messageCode,
        JsonElement[] elements)
    {
        return new(messageCode, elements);
    }
}