using System.ComponentModel;
using System.Text.Json;
// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets.Wamp
{
    #region Interfaces
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWampRole : IDisposable
    {
        Task SendAsync(WampMessage message, CancellationToken cancellationToken = default);
        Task SendAsync(ushort messageCode, object[] elements, CancellationToken cancellationToken = default);
        Task<WampMessage> ReceiveAsync(CancellationToken cancellationToken = default);
        Task CloseAsync(CancellationToken cancellationToken = default);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWampRole<TMessageTypeCodes> : IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
    {
        TMessageTypeCodes MessageCodes { get; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWampRole<TMessageTypeCodes, TMessageTypeCodeEnum> : IWampRole<TMessageTypeCodes>, IWampRole, IDisposable
        where TMessageTypeCodes : WampMessageTypeCodes
        where TMessageTypeCodeEnum : struct, Enum
    {
        Task SendAsync(WampMessage<TMessageTypeCodeEnum> message, CancellationToken cancellationToken = default);
        new Task<WampMessage<TMessageTypeCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default);
    }
    #endregion Interfaces

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleBase<TMessageTypeCodes> : IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
    {
        protected readonly WebSocket WebSocket;
        public TMessageTypeCodes MessageCodes { get; }

        protected internal WampRoleBase(WebSocket webSocket, TMessageTypeCodes messageCodes)
        {
            WebSocket = webSocket ?? throw new ArgumentNullException("WebSockets can't be null!", nameof(webSocket));
            MessageCodes = messageCodes ?? throw new ArgumentNullException("Message Codes can't be null!", nameof(messageCodes));
        }

        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            //await SendAsync(MessageCodes.Goodbye ?? (ushort) WampBasicProfile.WampRoleMessageTypeCode.Goodbye, new object[0], cancellationToken);
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
        }

        protected internal async Task<JsonElement[]> ReceiveJsonArrayAsync(CancellationToken cancellationToken = default)
        {
            string? json = "";

            // The League client sometimes sends empty strings. Unsure why.
            for (int i = 0; i < 100 && string.IsNullOrEmpty(json); i++)
                json = await WebSocket.ReceiveStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new WampResponseException("The returned message isn't a JSON array!", json);

            if (doc.RootElement.GetArrayLength() == 0)
                throw new WampResponseException("The returned JSON array didn't contain any values!", json);

            // TODO: Check if one enumerator is enough
            var enumerator = doc.RootElement.EnumerateArray();
            enumerator.MoveNext();
            if (enumerator.Current.ValueKind != JsonValueKind.Number)
                throw new WampResponseException("The first item in the returned array wasn't a message code!", json);

            return doc.RootElement.Clone().EnumerateArray().ToArray();
        }

        /// <summary>
        /// Override this method to do custom logic on messages received.
        /// </summary>
        protected virtual WampMessage OnMessageReceived(ushort messageCode, JsonElement[] elements) => new(messageCode, elements);

        public async Task<WampMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await ReceiveJsonArrayAsync(cancellationToken);
            return OnMessageReceived(array[0].GetUInt16(), array.Skip(1).ToArray());
        }

        public async Task SendAsync(WampMessage request, CancellationToken cancellationToken = default)
        {
            var array = new object[request.Elements.Length + 1];
            array[0] = request.MessageCode;
            request.Elements.CopyTo(array, 1);

            var json = JsonSerializer.Serialize(array);

            await WebSocket.SendStringAsync(json, cancellationToken);
        }

        public async Task SendAsync(ushort messageCode, object[] elements, CancellationToken cancellationToken = default) =>
            await SendAsync(new WampMessage(messageCode, elements), cancellationToken);

        public WebSocketState State => WebSocket.State;

        public void Dispose() => WebSocket.Dispose();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleBase<TMessageTypeCodes, TMessageTypeCodeEnum> : 
        WampRoleBase<TMessageTypeCodes>, IWampRole<TMessageTypeCodes, TMessageTypeCodeEnum>, IWampRole<TMessageTypeCodes>, IWampRole
        where TMessageTypeCodes : WampMessageTypeCodes
        where TMessageTypeCodeEnum : struct, Enum
    {
        protected internal WampRoleBase(WebSocket webSocket, TMessageTypeCodes messageCodes) : base(webSocket, messageCodes)
        {
        }

        public async Task SendAsync(WampMessage<TMessageTypeCodeEnum> message, CancellationToken cancellationToken = default) => 
            await base.SendAsync(message, cancellationToken);

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the other OnMessageReceived. This method is NOT called in this class.", true)]
        protected override WampMessage OnMessageReceived(ushort messageCode, JsonElement[] elements) => throw new NotImplementedException();
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        /// <summary>
        /// Override this method to do custom logic on messages received.
        /// </summary>
        protected virtual WampMessage<TMessageTypeCodeEnum> OnMessageReceived(TMessageTypeCodeEnum messageCode, JsonElement[] elements) => new(messageCode, elements);

        public new async Task<WampMessage<TMessageTypeCodeEnum>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var array = await ReceiveJsonArrayAsync(cancellationToken);
            return OnMessageReceived((TMessageTypeCodeEnum)Enum.ToObject(typeof(TMessageTypeCodeEnum), array[0].GetUInt16()), array.Skip(1).ToArray());
        }

        async Task<WampMessage> IWampRole.ReceiveAsync(CancellationToken cancellationToken) => await base.ReceiveAsync(cancellationToken);
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
