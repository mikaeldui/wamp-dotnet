// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets.Wamp;

public static class WampBasicProfile
{
    public enum WampBrokerMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Publish = 16,
        Published = 17,

        Subscribe = 33,
        Subscribed = 33,
        Unsubscribe = 34,
        Unsubscribed = 35,
        Event = 36,

        Extension = 0
    }

    public enum WampCalleeMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Register = 64,
        Registered = 65,
        Unregister = 66,
        Unregistered = 67,
        Invocation = 68,
        Yield = 70,

        Extension = 0
    }

    public enum WampCallerMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Subscribe = 33,
        Subscribed = 33,
        Unsubscribe = 34,
        Unsubscribed = 35,
        Event = 36,

        Extension = 0
    }

    public enum WampDealerMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Call = 48,
        Result = 58,

        Register = 64,
        Registered = 65,
        Unregister = 66,
        Unregistered = 67,
        Invocation = 68,
        Yield = 70,

        Extension = 0
    }

    public enum WampMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Publish = 16,
        Published = 17,

        Subscribe = 33,
        Subscribed = 33,
        Unsubscribe = 34,
        Unsubscribed = 35,
        Event = 36,

        Call = 48,
        Result = 58,

        Register = 64,
        Registered = 65,
        Unregister = 66,
        Unregistered = 67,
        Invocation = 68,
        Yield = 70,

        Extension = 0
    }

    public enum WampPublisherMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Publish = 16,
        Published = 17,

        Extension = 0
    }

    public enum WampRoleMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8
    }

    public enum WampSubscriberMessageTypeCode : ushort
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,

        Error = 8,

        Subscribe = 33,
        Subscribed = 33,
        Unsubscribe = 34,
        Unsubscribed = 35,
        Event = 36,

        Extension = 0
    }
}