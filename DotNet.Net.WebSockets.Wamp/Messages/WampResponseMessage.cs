using System.Diagnostics;
using System.Text.Json;

namespace System.Net.WebSockets.Wamp
{
    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampResponseMessage : IWampMessage
    {
        internal WampResponseMessage(JsonElement[] elements)
        {
            Elements = elements;
        }

        internal WampResponseMessage(ushort messageType, params JsonElement[] elements) : this(elements)
        {
            MessageCode = messageType;
        }

        public virtual ushort MessageCode { get; set; }
        public JsonElement[] Elements { get; }
        IEnumerable<object> IWampMessage.Elements => (IEnumerable<object>)Elements.ToList();
    }

    [DebuggerDisplay("MessageCode = {MessageCode} Element = {Elements}")]
    public class WampResponseMessage<TWampMessageTypeEnum> : WampResponseMessage, IWampMessage
    where TWampMessageTypeEnum : struct, Enum, IConvertible
    {
        internal WampResponseMessage(ushort messageCode, params JsonElement[] elements) : this((TWampMessageTypeEnum)Enum.ToObject(typeof(TWampMessageTypeEnum), messageCode), elements)
        {
        }

        internal WampResponseMessage(TWampMessageTypeEnum messageCode, params JsonElement[] elements) : base(elements)
        {
            MessageCode = messageCode;
        }

        internal WampResponseMessage(TWampMessageTypeEnum messageCode, IEnumerable<JsonElement> elements) : base(elements.ToArray())
        {
            MessageCode = messageCode;
        }

        public WampResponseMessage(TWampMessageTypeEnum messageCode) : base(new JsonElement[0])
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
