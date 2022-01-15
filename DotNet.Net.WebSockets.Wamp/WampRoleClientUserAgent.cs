using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Net.WebSockets.Wamp
{
    internal static class WampRoleClientUserAgent
    {
        internal static readonly string USER_AGENT;

        static WampRoleClientUserAgent()
        {
            var wampUserAgent = UserAgent.From(typeof(WampRoleClientUserAgent).GetTypeInfo().Assembly);

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                wampUserAgent.DependentProduct = UserAgent.From(entryAssembly);

            USER_AGENT = wampUserAgent.ToString();
        }
    }
}
