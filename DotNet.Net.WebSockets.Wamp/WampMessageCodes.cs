using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Net.WebSockets.Wamp
{
    internal static class WampMessageCodesLoader
    {
        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        internal static T LoadFromEnum<T>(Type @enum)
            where T : WampMessageCodes
        {
            var type = typeof(T);
            var messageCodes = (T) Activator.CreateInstance(type, true);
            foreach (var code in Enum.GetNames(@enum))
            {
                var propertyInfo = type.GetProperty(code);
                if (propertyInfo != null)
                    propertyInfo.SetValue(messageCodes, ((IConvertible)Enum.Parse(@enum, code)).ToUInt16(provider: null), null);
            }
            return messageCodes;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WampMessageCodes
    {
        internal protected WampMessageCodes()
        {
        }

        private ushort? _hello;
        private ushort? _welcome;
        private ushort? _abort;
        private ushort? _goodbye;
        private ushort? _error;

        public ushort? Hello { get => _hello; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _hello = value; } }
        public ushort? Welcome { get => _welcome; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _welcome = value; } }
        public ushort? Abort { get => _abort; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _abort = value; } }
        public ushort? Goodbye { get => _goodbye; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _goodbye = value; } }
        public ushort? Error { get => _error; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _error = value; } }

        internal bool IsReadOnly = false;
    }

    public class WampSubscriberMessageCodes : WampMessageCodes
    {
        internal WampSubscriberMessageCodes()
        {
        }
        
        public WampSubscriberMessageCodes(ushort subscribe, ushort unsubscribe, ushort @event)
        {
            Subscribe = subscribe;
            Unsubscribe = unsubscribe;
            Event = @event;
        }

        private ushort _subscribe;
        private ushort? _subscribed;
        private ushort _unsubscribe;
        private ushort? _unsubscribed;
        private ushort _event;

        public ushort Subscribe { get => _subscribe; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _subscribe = value; } }
        public ushort? Subscribed { get => _subscribed; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _subscribed = value; } }
        public ushort Unsubscribe { get => _unsubscribe; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _unsubscribe = value; } }
        public ushort? Unsubscribed { get => _unsubscribed; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _unsubscribed = value; } }
        public ushort Event { get => _event; set { if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only."); _event = value; } }

        public static readonly WampSubscriberMessageCodes BasicProfile = FromEnum<WampBasicProfile.WampSubscriberMessageCode>();

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public static WampSubscriberMessageCodes FromEnum<TEnum>() where TEnum : struct, Enum => FromEnum(typeof(TEnum));

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public static WampSubscriberMessageCodes FromEnum(Type @enum) => WampMessageCodesLoader.LoadFromEnum<WampSubscriberMessageCodes>(@enum);
    }
}
