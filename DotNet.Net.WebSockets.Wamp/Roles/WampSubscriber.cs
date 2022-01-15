namespace System.Net.WebSockets.Wamp
{
    public interface IWampSubscriber : IWampRole
    {
        Task SubscribeAsync(string topic, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
    }

    public interface IWampSubscriber<TMessageCodeEnum> : IWampSubscriber, IWampRole<TMessageCodeEnum>
    where TMessageCodeEnum : struct, Enum
    {
    }

    //internal class WampSubscriberMethods
    //{
    //    public async Task SendSubscribeAsync(this IWampSubscriber wampSubscriber, string topic, CancellationToken cancellationToken = default) =>
    //        await wampSubscriber.SendAsync(new(wampSubscriber.MessageCodes.Subscribe, topic), cancellationToken);

    //    public async Task SendUnsubscribeAsync(string topic, CancellationToken cancellationToken = default) =>
    //        await SendAsync(new(MessageCodes.Unsubscribe, topic), cancellationToken);
    //}

    public class WampSubscriber : WampRoleBase<WampSubscriberMessageCodes>, IWampSubscriber
    {
        public WampSubscriber(WebSocket webSocket, WampSubscriberMessageCodes? messageCodes = null) :
            base(webSocket, messageCodes ?? WampSubscriberMessageCodes.BasicProfile)
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new(MessageCodes.Subscribe, topic), cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new(MessageCodes.Unsubscribe, topic), cancellationToken);
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

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new(MessageCodes.Subscribe, topic), cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new(MessageCodes.Unsubscribe, topic), cancellationToken);
    }

    public class WampSubscriberClient<TMessageCodeEnum> : WampRoleClientBase<WampSubscriberMessageCodes, TMessageCodeEnum>, IWampSubscriber<TMessageCodeEnum>, IWampSubscriber
        where TMessageCodeEnum : struct, Enum
    {
        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public WampSubscriberClient() : base(WampSubscriberMessageCodes.FromEnum<TMessageCodeEnum>())
        {
        }

        public async Task SubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new WampRequestMessage(MessageCodes.Subscribe, topic), cancellationToken);

        public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default) => 
            await SendAsync(new WampRequestMessage(MessageCodes.Unsubscribe, topic), cancellationToken);
    }
}