using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.WebSockets.Wamp
{
    public class WampResponseException : Exception
    {
        public WampResponseException(string message) : base(message)
        {
        }

        public WampResponseException(string message, string webSocketMessage) : this(message)
        {
            WebSocketMessage = webSocketMessage;
        }

        public string? WebSocketMessage { get; }
    }
}
