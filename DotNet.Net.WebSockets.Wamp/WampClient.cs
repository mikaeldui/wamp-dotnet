using System.ComponentModel;
using System.Text.Json;

namespace System.Net.WebSockets.Wamp
{
    public interface IWampRole : IDisposable
    {
        Task SendAsync(WampRequestMessage message, CancellationToken cancellationToken = default);
        Task SendAsync(ushort messageCode, object[] elements, CancellationToken cancellationToken = default);
        Task<WampResponseMessage> ReceiveAsync(CancellationToken cancellationToken = default);
        Task CloseAsync(CancellationToken cancellationToken = default);
    }

    public interface IWampRole<TMessageCodeEnum> : IWampRole, IDisposable
        where TMessageCodeEnum : struct, Enum
    {
        Task SendAsync(WampRequestMessage<TMessageCodeEnum> message, CancellationToken cancellationToken = default);
        new Task<WampResponseMessage<TMessageCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default);
    }

    public interface IWampRoleClient : IWampRole
    {
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default);
        Task ConnectAsync(string uri, CancellationToken cancellationToken = default);
    }

    public interface IWampRoleClient<TMessageCodeEnum> : IWampRole<TMessageCodeEnum>, IWampRoleClient
        where TMessageCodeEnum : struct, Enum
    {
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleBase : IWampRole
    {
        protected readonly WebSocket WebSocket;

        internal protected WampRoleBase(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public virtual async Task CloseAsync(CancellationToken cancellationToken = default) => 
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);

        protected async Task<JsonElement[]> ReceiveJsonArrayAsync(CancellationToken cancellationToken = default)
        {
            string? json = "";

            // The League client sometimes sends empty strings. Unsure why.
            for (int i = 0; i < 100 && string.IsNullOrEmpty(json); i++)
                json = await WebSocket.ReceiveStringAsync();

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new WampResponseException("The returned message isn't a JSON array!", json);

            if (doc.RootElement.GetArrayLength() == 0)
                throw new WampResponseException("The returned JSON array didn't contain any values!", json);

            var enumerator = doc.RootElement.EnumerateArray();
            enumerator.MoveNext();
            if (enumerator.Current.ValueKind != JsonValueKind.Number)
                throw new WampResponseException("The first item in the returned array wasn't a message code!", json);

            return doc.RootElement.Clone().EnumerateArray().ToArray();
        }

        public virtual async Task<WampResponseMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await ReceiveJsonArrayAsync(cancellationToken);
            return new WampResponseMessage(array[0].GetUInt16(), array.Skip(1).ToArray());
        }

        public virtual async Task SendAsync(WampRequestMessage request, CancellationToken cancellationToken = default)
        {
            var array = new object[request.Elements.Count + 1];
            array[0] = request.MessageCode;
            request.Elements.CopyTo(array, 1);

            var json = JsonSerializer.Serialize(array);

            await WebSocket.SendStringAsync(json);
        }

        public virtual async Task SendAsync(ushort messageCode, object[] elements, CancellationToken cancellationToken = default) =>
            await SendAsync(new(messageCode, elements), cancellationToken);

        public WebSocketState State => WebSocket.State;

        public void Dispose() => WebSocket.Dispose();
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleBase<TMessageCodes> : WampRoleBase, IWampRole
        where TMessageCodes : WampRoleMessageCodes<TMessageCodes>
    {
        protected readonly TMessageCodes MessageCodes;

        internal protected WampRoleBase(WebSocket webSocket, TMessageCodes messageCodes) : base(webSocket) => 
            MessageCodes = messageCodes;
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleBase<TMessageCodes, TMessageCodeEnum> : WampRoleBase<TMessageCodes>, IWampRole<TMessageCodeEnum>, IWampRole
        where TMessageCodes : WampRoleMessageCodes<TMessageCodes>
        where TMessageCodeEnum : struct, Enum
    {
        internal protected WampRoleBase(WebSocket webSocket, TMessageCodes messageCodes) : base(webSocket, messageCodes)
        {
        }

        public virtual async Task SendAsync(WampRequestMessage<TMessageCodeEnum> message, CancellationToken cancellationToken = default) => 
            await base.SendAsync(message, cancellationToken);

        public new virtual async Task<WampResponseMessage<TMessageCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await base.ReceiveJsonArrayAsync();
            return new WampResponseMessage<TMessageCodeEnum>(array[0].GetUInt16(), array.Skip(1).ToArray());
        }

        async Task<WampResponseMessage> IWampRole.ReceiveAsync(CancellationToken cancellationToken) => await base.ReceiveAsync(cancellationToken);
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleClientBase : WampRoleBase, IWampRoleClient, IWampRole
    {
        protected new ClientWebSocket WebSocket;

        internal protected WampRoleClientBase() : base(new ClientWebSocket())
        {
            WebSocket = (ClientWebSocket)base.WebSocket;
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            await WebSocket.ConnectAsync(uri, cancellationToken);
        }

        public async Task ConnectAsync(string uri, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));

            var uriUri = new Uri(uri);

            if (!uriUri.IsAbsoluteUri)
                throw new ArgumentException("The URI must be absolute.", nameof(uriUri));

            await WebSocket.ConnectAsync(uriUri, cancellationToken);
        }
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleClientBase<TMessageCodes> : WampRoleBase<TMessageCodes>, IWampRoleClient, IWampRole
        where TMessageCodes : WampRoleMessageCodes<TMessageCodes>
    {
        protected new ClientWebSocket WebSocket;

        internal protected WampRoleClientBase(TMessageCodes messageCodes) : base(new ClientWebSocket(), messageCodes) => 
            WebSocket = (ClientWebSocket)base.WebSocket;

        public ClientWebSocketOptions Options => ((ClientWebSocket)WebSocket).Options;

        public virtual async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default) => 
            await WebSocket.ConnectAsync(uri, cancellationToken);

        public virtual async Task ConnectAsync(string uri, CancellationToken cancellationToken = default) => 
            await WebSocket.ConnectAsync(uri, cancellationToken);
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleClientBase<TMessageCodes, TMessageCodeEnum> : WampRoleClientBase<TMessageCodes>, IWampRoleClient<TMessageCodeEnum>, IWampRoleClient, IWampRole<TMessageCodeEnum>, IWampRole
        where TMessageCodes : WampRoleMessageCodes<TMessageCodes>
        where TMessageCodeEnum : struct, Enum
    {
        protected new ClientWebSocket WebSocket;

        internal protected WampRoleClientBase(TMessageCodes messageCodes) : base(messageCodes) => 
            WebSocket = (ClientWebSocket)base.WebSocket;

        public virtual async Task SendAsync(WampRequestMessage<TMessageCodeEnum> message, CancellationToken cancellationToken = default) =>
            await base.SendAsync(message, cancellationToken);

        public new virtual async Task<WampResponseMessage<TMessageCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await base.ReceiveJsonArrayAsync();
            return new WampResponseMessage<TMessageCodeEnum>(array[0].GetUInt16(), array.Skip(1).ToArray());
        }

        async Task<WampResponseMessage> IWampRole.ReceiveAsync(CancellationToken cancellationToken) => await base.ReceiveAsync(cancellationToken);
    }

    #region Other Roles
#if false

    public interface IWampPublisher : IWampRole
    {
        Task PublishAsync(JsonElement element, CancellationToken cancellationToken = default);
    }

    public class WampPublisher : WampRoleBase, IWampPublisher
    {
        public async Task PublishAsync(JsonElement element, CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Publish, element), cancellationToken);
        }
    }

    public interface IWampBroker : IWampRole
    {
        Task SubscribedAsync(CancellationToken cancellationToken = default);
        Task UnsubscribedAsync(CancellationToken cancellationToken = default);
        Task EventAsync(CancellationToken cancellationToken = default);
    }

    public class WampBroker : WampRoleBase, IWampBroker
    {
        public async Task EventAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Event, element), cancellationToken);
        }

        public async Task SubscribedAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Subscribed, element), cancellationToken);
        }

        public async Task UnsubscribedAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Unsubscribed, element), cancellationToken);
        }
    }

    public interface IWampCaller : IWampRole
    {
        Task CallAsync(CancellationToken cancellationToken = default);
    }

    public class WampCaller : WampRoleBase, IWampCaller
    {
        public async Task CallAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Call, element), cancellationToken);
        }
    }

    public interface IWampDealer : IWampRole
    {
        Task ResultAsync(CancellationToken cancellationToken = default);
        Task RegisteredAsync(CancellationToken cancellationToken = default);
        Task UnregisteredAsync(CancellationToken cancellationToken = default);
        Task InvocationAsync(CancellationToken cancellationToken = default);
    }

    public class WampDealer : WampRoleBase, IWampDealer
    {
        public Task InvocationAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Invocation, element), cancellationToken);
        }

        public Task RegisteredAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Registered, element), cancellationToken);
        }

        public Task ResultAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Result, element), cancellationToken);
        }

        public Task UnregisteredAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Unregistered, element), cancellationToken);
        }
    }

    public interface IWampCallee : IWampRole
    {
        Task RegisterAsync(CancellationToken cancellationToken = default);
        Task UnregisterAsync(CancellationToken cancellationToken = default);
        Task YieldAsync(CancellationToken cancellationToken = default);
    }

    public class WampCallee : WampRoleBase, IWampCallee
    {
        public async Task RegisterAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Register, element), cancellationToken);
        }

        public async Task UnregisterAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Unregister, element), cancellationToken);
        }

        public async Task YieldAsync(CancellationToken cancellationToken = default)
        {
            await SendAsync(new(MessageCodes.Yield, element), cancellationToken);
        }
    }
#endif

    #endregion Other Roles
}