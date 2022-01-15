using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace System.Net.WebSockets.Wamp
{
    public interface IWampMessage
    {
        public ushort MessageCode { get; set; }
        public IEnumerable<object> Elements { get; }
    }

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
