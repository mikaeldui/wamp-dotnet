using System.ComponentModel;
using System.Diagnostics;

namespace System.Net.WebSockets.Wamp
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWampMessage
    {
        ushort MessageCode { get; set; }
        object[] Elements { get; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWampMessage<TMessageTypeCodeEnum> : IWampMessage
        where TMessageTypeCodeEnum : struct, Enum
    {
        new TMessageTypeCodeEnum MessageCode { get; set; }
    }

    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampMessage : IWampMessage
    {
        // TODO: Maybe object[] isn't the best.
        internal WampMessage(object[] elements)
        {
            Elements = elements;
        }

        public WampMessage(ushort messageType, params object[] elements) : this(elements)
        {
            MessageCode = messageType;
        }

        public ushort MessageCode { get; set; }
        public object[] Elements { get; }
    }

    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampMessage<TWampMessageTypeEnum> : WampMessage, IWampMessage<TWampMessageTypeEnum>, IWampMessage
    where TWampMessageTypeEnum : struct, Enum, IConvertible
    {
        public WampMessage(TWampMessageTypeEnum messageCode, params object[] elements) : base(elements)
        {
            MessageCode = messageCode;
        }

        private TWampMessageTypeEnum _messageCode;
        public new virtual TWampMessageTypeEnum MessageCode
        {
            get => _messageCode;
            set
            {
                _messageCode = value;
                base.MessageCode = MessageCode.ToUInt16(provider: null);
            }
        }

        ushort IWampMessage.MessageCode
        {
            get => base.MessageCode;
            set => MessageCode = (TWampMessageTypeEnum)Enum.ToObject(typeof(TWampMessageTypeEnum), value);
        }
    }
}
