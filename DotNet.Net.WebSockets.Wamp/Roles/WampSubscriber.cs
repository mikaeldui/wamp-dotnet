using System.ComponentModel;

// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets.Wamp;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWampSubscriber : IWampRole<WampSubscriberMessageTypeCodes>
{
    Task SubscribeAsync(string topic, CancellationToken cancellationToken = default);
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWampSubscriber<TMessageTypeCodeEnum> : IWampSubscriber,
    IWampRole<WampSubscriberMessageTypeCodes, TMessageTypeCodeEnum>
    where TMessageTypeCodeEnum : struct, Enum
{
}

internal static class WampSubscriberMethodImplementations
{
    internal static async Task SubscribeAsyncInternal(this IWampSubscriber wampSubscriber, string topic,
        CancellationToken cancellationToken = default)
    {
        await wampSubscriber.SendAsync(new WampMessage(wampSubscriber.MessageCodes.Subscribe, topic),
            cancellationToken);
    }

    internal static async Task UnsubscribeAsyncInternal(this IWampSubscriber wampSubscriber, string topic,
        CancellationToken cancellationToken = default)
    {
        await wampSubscriber.SendAsync(new WampMessage(wampSubscriber.MessageCodes.Unsubscribe, topic),
            cancellationToken);
    }
}

/// <summary>
///     For use with a custom WebSocket. Consider <see cref="WampSubscriberClient" /> if you're not sure.
/// </summary>
public class WampSubscriber : WampRoleBase<WampSubscriberMessageTypeCodes>, IWampSubscriber
{
    public WampSubscriber(WebSocket webSocket, WampSubscriberMessageTypeCodes? messageCodes = null) :
        base(webSocket, messageCodes ?? WampSubscriberMessageTypeCodes.BasicProfile)
    {
    }

    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.SubscribeAsyncInternal(topic, cancellationToken);
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.UnsubscribeAsyncInternal(topic, cancellationToken);
    }

    //protected override WampResponseMessage OnMessageReceived(ushort messageCode, JsonElement[] elements) => this.OnMessageReceivedInternal(messageCode, elements) ?? base.OnMessageReceived(messageCode, elements);
}

/// <summary>
///     For use with a custom WebSocket. Consider <see cref="WampSubscriberClient{TMessageCodeEnum}" /> if you're not sure.
/// </summary>
/// <typeparam name="TMessageCodeEnum">Required to have "Subscribe", "Unsubscribe" and "Event". Type is Uint16.</typeparam>
public class WampSubscriber<TMessageCodeEnum> : WampRoleBase<WampSubscriberMessageTypeCodes, TMessageCodeEnum>,
    IWampSubscriber<TMessageCodeEnum>
    where TMessageCodeEnum : struct, Enum
{
    public WampSubscriber(WebSocket webSocket, WampSubscriberMessageTypeCodes? messageCodes = null) :
        base(webSocket, messageCodes ?? WampSubscriberMessageTypeCodes.BasicProfile)
    {
    }

    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.SubscribeAsyncInternal(topic, cancellationToken);
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.UnsubscribeAsyncInternal(topic, cancellationToken);
    }
}

/// <summary>
///     You're also able to supply your own message code enum with <see cref="WampSubscriberClient{TMessageCodeEnum}" />.
/// </summary>
public class WampSubscriberClient : WampRoleClientBase<WampSubscriberMessageTypeCodes>, IWampSubscriber
{
    public WampSubscriberClient(WampSubscriberMessageTypeCodes? messageCodes = null) : base(messageCodes ??
        WampSubscriberMessageTypeCodes.BasicProfile)
    {
    }

    /// <summary>
    ///     [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode" />.
    /// </summary>
    public WampSubscriberClient(Type subscriberMessageCodesEnum) : base(
        WampSubscriberMessageTypeCodes.FromEnum(subscriberMessageCodesEnum))
    {
    }

    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.SubscribeAsyncInternal(topic, cancellationToken);
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.UnsubscribeAsyncInternal(topic, cancellationToken);
    }

    //protected override WampResponseMessage OnMessageReceived(ushort messageCode, JsonElement[] elements) => this.OnMessageReceivedInternal(messageCode, elements) ?? base.OnMessageReceived(messageCode, elements);
}

/// <typeparam name="TMessageCodeEnum">Required to have "Subscribe", "Unsubscribe" and "Event". Type is Uint16.</typeparam>
public class WampSubscriberClient<TMessageCodeEnum> :
    WampRoleClientBase<WampSubscriberMessageTypeCodes, TMessageCodeEnum>, IWampSubscriber<TMessageCodeEnum>
    where TMessageCodeEnum : struct, Enum
{
    /// <summary>
    ///     [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode" />.
    /// </summary>
    public WampSubscriberClient() : base(WampSubscriberMessageTypeCodes.FromEnum<TMessageCodeEnum>())
    {
    }

    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.SubscribeAsyncInternal(topic, cancellationToken);
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        await this.UnsubscribeAsyncInternal(topic, cancellationToken);
    }
}