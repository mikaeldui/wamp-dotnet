using System.Diagnostics;

namespace System.Net.WebSockets.Wamp
{
    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampRequestMessage : IWampMessage
    {
        // TODO: Maybe object[] isn't the best.
        internal WampRequestMessage(object[] elements)
        {
            Elements = elements.ToList();
        }

        public WampRequestMessage(ushort messageType, params object[] elements) : this(elements)
        {
            MessageCode = messageType;
        }

        public ushort MessageCode { get; set; }
        public List<object> Elements { get; }
        IEnumerable<object> IWampMessage.Elements => Elements;
    }

    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampRequestMessage<TWampMessageTypeEnum> : WampRequestMessage, IWampMessage
    where TWampMessageTypeEnum : struct, Enum, IConvertible
    {
        internal WampRequestMessage(ushort messageCode, params object[] elements) : this((TWampMessageTypeEnum)Enum.ToObject(typeof(TWampMessageTypeEnum), messageCode), elements)
        {
        }

        public WampRequestMessage(TWampMessageTypeEnum messageCode, params object[] elements) : base(elements)
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
