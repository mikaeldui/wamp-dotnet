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
}
