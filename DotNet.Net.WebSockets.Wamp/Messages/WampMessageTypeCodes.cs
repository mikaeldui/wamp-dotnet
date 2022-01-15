using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Net.WebSockets.Wamp
{
    internal static class WampMessageTypeCodesLoader
    {
        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode"/>.
        /// </summary>
        internal static T LoadFromEnum<T>(Type @enum)
            where T : WampMessageTypeCodes
        {
            var type = typeof(T);
            T messageCodes = (T) Activator.CreateInstance(type, true);
            foreach (var code in Enum.GetNames(@enum))
            {
                var propertyInfo = type.GetProperty(code);
                if (propertyInfo != null)
                    propertyInfo.SetValue(messageCodes, ((IConvertible)Enum.Parse(@enum, code)).ToUInt16(provider: null), null);
            }
            return messageCodes;
        }
    }

    // TODO: Consider making these classes internal.

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WampMessageTypeCodes
    {
        internal protected WampMessageTypeCodes()
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WampSubscriberMessageTypeCodes : WampMessageTypeCodes
    {
        internal WampSubscriberMessageTypeCodes()
        {
        }
        
        public WampSubscriberMessageTypeCodes(ushort subscribe, ushort unsubscribe, ushort @event)
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

        public static readonly WampSubscriberMessageTypeCodes BasicProfile = FromEnum<WampBasicProfile.WampSubscriberMessageTypeCode>();

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode"/>.
        /// </summary>
        public static WampSubscriberMessageTypeCodes FromEnum<TEnum>() where TEnum : struct, Enum => FromEnum(typeof(TEnum));

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode"/>.
        /// </summary>
        public static WampSubscriberMessageTypeCodes FromEnum(Type @enum) => WampMessageTypeCodesLoader.LoadFromEnum<WampSubscriberMessageTypeCodes>(@enum);
    }
}
