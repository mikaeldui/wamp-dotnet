using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Net.WebSockets.Wamp
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleMessageCodes<T>
        where T : WampRoleMessageCodes<T>
    {
        internal WampRoleMessageCodes()
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

        public static readonly WampRoleMessageCodes<T> BasicProfile = FromEnum<WampBasicProfile.WampRoleMessageCode>();

        internal bool IsReadOnly = false;

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public static T FromEnum<TEnum>() where TEnum : struct, Enum => FromEnum(typeof(TEnum));

        /// <summary>
        /// [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageCode"/>.
        /// </summary>
        public static T FromEnum(Type @enum)
        {
            var type = typeof(T);
            var messageCode = Activator.CreateInstance(type, true);

            foreach (var code in Enum.GetNames(@enum))
            {
                var propertyInfo = type.GetProperty(code);
                if (propertyInfo != null)
                    propertyInfo.SetValue(messageCode, ((IConvertible)Enum.Parse(@enum, code)).ToUInt16(provider: null), null);
            }

            return (T)messageCode;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WampRoleMessageCodes : WampRoleMessageCodes<WampRoleMessageCodes>
    {
        internal WampRoleMessageCodes()
        {
        }
    }

    public class WampSubscriberMessageCodes : WampRoleMessageCodes<WampSubscriberMessageCodes>
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

        public static new readonly WampSubscriberMessageCodes BasicProfile = FromEnum<WampBasicProfile.WampSubscriberMessageCode>();
    }
}
