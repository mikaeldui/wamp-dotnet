namespace System.Net.WebSockets.Wamp
{
    public interface IWampSubscriber : IWampRole<WampSubscriberMessageCodes>, IWampRole
    {
        Task SubscribeAsync(string topic, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
    }

    public interface IWampSubscriber<TMessageCodeEnum> : IWampSubscriber, IWampRole<WampSubscriberMessageCodes, TMessageCodeEnum>, IWampRole
        where TMessageCodeEnum : struct, Enum
    {
    }

    internal static class WampSubscriberMethodImplementations
    {
        public static async Task SendSubscribeAsync(this IWampSubscriber wampSubscriber, string topic, CancellationToken cancellationToken = default) =>
            await wampSubscriber.SendAsync(new(wampSubscriber.MessageCodes.Subscribe, topic), cancellationToken);

        public static async Task SendUnsubscribeAsync(this IWampSubscriber wampSubscriber, string topic, CancellationToken cancellationToken = default) =>
            await wampSubscriber.SendAsync(new(wampSubscriber.MessageCodes.Unsubscribe, topic), cancellationToken);
    }

    public class WampSubscriber : WampRoleBase<WampSubscriberMessageCodes>, IWampSubscriber
    {
        public WampSubscriber(WebSocket webSocket, WampSubscriberMessageCodes? messageCodes = null) :
            base(webSocket, messageCodes ?? WampSubscriberMessageCodes.BasicProfile)
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendSubscribeAsync(topic, cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendUnsubscribeAsync(topic, cancellationToken);
    }

    /// <summary>
    /// For use with a custom WebSocket.
    /// </summary>
    /// <typeparam name="TMessageCodeEnum">Required to have "Subscribe", "Unsubscribe" and "Event". Type is Uint16.</typeparam>
    public class WampSubscriber<TMessageCodeEnum> : WampRoleBase<WampSubscriberMessageCodes, TMessageCodeEnum>, IWampSubscriber<TMessageCodeEnum>, IWampSubscriber
        where TMessageCodeEnum : struct, Enum
    {
        public WampSubscriber(WebSocket webSocket, WampSubscriberMessageCodes? messageCodes = null) :
            base(webSocket, messageCodes ?? WampSubscriberMessageCodes.BasicProfile)
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendSubscribeAsync(topic, cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendUnsubscribeAsync(topic, cancellationToken);
    }

    public class WampSubscriberClient : WampRoleClientBase<WampSubscriberMessageCodes>, IWampSubscriber
    {
        public WampSubscriberClient(WampSubscriberMessageCodes? messageCodes = null) : base(messageCodes ?? WampSubscriberMessageCodes.BasicProfile)
        {
        }

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public WampSubscriberClient(Type subscriberMessageCodesEnum) : base(WampSubscriberMessageCodes.FromEnum(subscriberMessageCodesEnum))
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendSubscribeAsync(topic, cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendUnsubscribeAsync(topic, cancellationToken);
    }

    /// <typeparam name="TMessageCodeEnum">Required to have "Subscribe", "Unsubscribe" and "Event". Type is Uint16.</typeparam>
    public class WampSubscriberClient<TMessageCodeEnum> : WampRoleClientBase<WampSubscriberMessageCodes, TMessageCodeEnum>, IWampSubscriber<TMessageCodeEnum>, IWampSubscriber
        where TMessageCodeEnum : struct, Enum
    {
        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public WampSubscriberClient() : base(WampSubscriberMessageCodes.FromEnum<TMessageCodeEnum>())
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendSubscribeAsync(topic, cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => await this.SendUnsubscribeAsync(topic, cancellationToken);
    }
}