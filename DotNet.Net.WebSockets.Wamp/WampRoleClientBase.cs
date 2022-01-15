using System.ComponentModel;
using System.Text.Json;

namespace System.Net.WebSockets.Wamp
{
    public interface IWampRoleClient : IWampRole
    {
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default);
        Task ConnectAsync(string uri, CancellationToken cancellationToken = default);
    }

    public interface IWampRoleClient<TMessageTypeCodes> : IWampRoleClient, IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
    {
    }

    public interface IWampRoleClient<TMessageTypeCodes, TMessageTypeCodeEnum> :
        IWampRoleClient<TMessageTypeCodes>, IWampRoleClient,
        IWampRole<TMessageTypeCodes, TMessageTypeCodeEnum>, IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
        where TMessageTypeCodeEnum : struct, Enum
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleClientBase<TMessageTypeCodes> : 
        WampRoleBase<TMessageTypeCodes>, IWampRoleClient<TMessageTypeCodes>, IWampRoleClient, 
        IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
    {
        protected new ClientWebSocket WebSocket;

        internal protected WampRoleClientBase(TMessageTypeCodes messageCodes) : base(new ClientWebSocket(), messageCodes) => 
            WebSocket = (ClientWebSocket)base.WebSocket;

        public ClientWebSocketOptions Options => ((ClientWebSocket)WebSocket).Options;

        public virtual async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default) => 
            await WebSocket.ConnectAsync(uri, cancellationToken);

        public virtual async Task ConnectAsync(string uri, CancellationToken cancellationToken = default) => 
            await WebSocket.ConnectAsync(uri, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleClientBase<TMessageTypeCodes, TMessageTypeCodeEnum> :
        WampRoleClientBase<TMessageTypeCodes>, IWampRoleClient<TMessageTypeCodes, TMessageTypeCodeEnum>, IWampRoleClient<TMessageTypeCodes>, IWampRoleClient,
        IWampRole<TMessageTypeCodes, TMessageTypeCodeEnum>, IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
        where TMessageTypeCodeEnum : struct, Enum
    {
        internal protected WampRoleClientBase(TMessageTypeCodes messageCodes) : base(messageCodes)
        {
        }

        public virtual async Task SendAsync(WampRequestMessage<TMessageTypeCodeEnum> message, CancellationToken cancellationToken = default) =>
            await base.SendAsync(message, cancellationToken);


#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the other OnMessageReceived. This method is NOT called in this class.", true)]
        protected override WampResponseMessage OnMessageReceived(ushort messageCode, JsonElement[] elements) => throw new NotImplementedException();
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        /// <summary>
        /// Override this method to do custom logic on messages received.
        /// </summary>
        protected virtual WampResponseMessage<TMessageTypeCodeEnum> OnMessageReceived(TMessageTypeCodeEnum messageCode, JsonElement[] elements) => new(messageCode, elements);

        public new async Task<WampResponseMessage<TMessageTypeCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await ReceiveJsonArrayAsync(cancellationToken);
            return OnMessageReceived((TMessageTypeCodeEnum)Enum.ToObject(typeof(TMessageTypeCodeEnum), array[0].GetUInt16()), array.Skip(1).ToArray());
        }

        async Task<WampResponseMessage> IWampRole.ReceiveAsync(CancellationToken cancellationToken) => await base.ReceiveAsync(cancellationToken);
    }
}