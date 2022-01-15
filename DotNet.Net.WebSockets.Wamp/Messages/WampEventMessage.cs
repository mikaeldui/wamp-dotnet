using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace System.Net.WebSockets.Wamp
{
    //public class WampTopicEventMessage : WampResponseMessage
    //{
    //    internal WampTopicEventMessage(ushort messageType, params JsonElement[] elements) : base(messageType, elements)
    //    {
    //        Topic = elements.First(e => e.ValueKind == JsonValueKind.String).GetString();
    //        ArgumentKw = elements.Last().Deserialize<Dictionary<string, object>>();
    //    }

    //    public string? Topic { get; }

    //    public Dictionary<string, JsonElement> ArgumentKw { get; }
    //}
}
