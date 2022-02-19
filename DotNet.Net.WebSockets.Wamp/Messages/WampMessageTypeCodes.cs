using System.ComponentModel;

// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets.Wamp;

internal static class WampMessageTypeCodesLoader
{
    /// <summary>
    ///     [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode" />.
    /// </summary>
    internal static T LoadFromEnum<T>(Type @enum)
        where T : WampMessageTypeCodes
    {
        var type = typeof(T);
        var messageCodes = (T) Activator.CreateInstance(type, true);
        foreach (var code in Enum.GetNames(@enum))
            type.GetProperty(code)
                ?.SetValue(messageCodes, ((IConvertible) Enum.Parse(@enum, code)).ToUInt16(null), null);

        return messageCodes;
    }
}

// TODO: Consider making these classes internal.

[EditorBrowsable(EditorBrowsableState.Never)]
public class WampMessageTypeCodes
{
    private ushort? _abort;
    private ushort? _error;
    private ushort? _goodbye;

    private ushort? _hello;
    private ushort? _welcome;

    internal bool IsReadOnly = false;

    protected internal WampMessageTypeCodes()
    {
    }

    public ushort? Hello
    {
        get => _hello;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _hello = value;
        }
    }

    public ushort? Welcome
    {
        get => _welcome;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _welcome = value;
        }
    }

    public ushort? Abort
    {
        get => _abort;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _abort = value;
        }
    }

    public ushort? Goodbye
    {
        get => _goodbye;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _goodbye = value;
        }
    }

    public ushort? Error
    {
        get => _error;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _error = value;
        }
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public class WampSubscriberMessageTypeCodes : WampMessageTypeCodes
{
    public static readonly WampSubscriberMessageTypeCodes BasicProfile =
        FromEnum<WampBasicProfile.WampSubscriberMessageTypeCode>();

    private ushort _event;

    private ushort _subscribe;
    private ushort? _subscribed;
    private ushort _unsubscribe;
    private ushort? _unsubscribed;

    internal WampSubscriberMessageTypeCodes()
    {
    }

    public WampSubscriberMessageTypeCodes(ushort subscribe, ushort unsubscribe, ushort @event)
    {
        Subscribe = subscribe;
        Unsubscribe = unsubscribe;
        Event = @event;
    }

    public ushort Subscribe
    {
        get => _subscribe;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _subscribe = value;
        }
    }

    public ushort? Subscribed
    {
        get => _subscribed;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _subscribed = value;
        }
    }

    public ushort Unsubscribe
    {
        get => _unsubscribe;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _unsubscribe = value;
        }
    }

    public ushort? Unsubscribed
    {
        get => _unsubscribed;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _unsubscribed = value;
        }
    }

    public ushort Event
    {
        get => _event;
        set
        {
            if (IsReadOnly) throw new InvalidOperationException("This WampMessageCodes is read-only.");
            _event = value;
        }
    }

    /// <summary>
    ///     [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode" />.
    /// </summary>
    public static WampSubscriberMessageTypeCodes FromEnum<TEnum>() where TEnum : struct, Enum
    {
        return FromEnum(typeof(TEnum));
    }

    /// <summary>
    ///     [ADVANCED] Use an enum like <see cref="WampBasicProfile.WampSubscriberMessageTypeCode" />.
    /// </summary>
    public static WampSubscriberMessageTypeCodes FromEnum(Type @enum)
    {
        return WampMessageTypeCodesLoader.LoadFromEnum<WampSubscriberMessageTypeCodes>(@enum);
    }
}