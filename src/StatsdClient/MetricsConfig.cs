﻿using StatsdClient.Senders;
using System.Collections.Generic;

namespace StatsdClient
{
    public class MetricsConfig
    {
        /// <summary>
        /// The full host name of your statsd server.
        /// </summary>
        public string StatsdServerName { get; set; }

        /// <summary>
        /// Uses the statsd default port if not specified (8125).
        /// </summary>
        public int StatsdServerPort { get; set; }

        /// <summary>
        /// Allows you to override the maximum UDP packet size (in bytes) if your setup requires that. Defaults to 512.
        /// </summary>
        public int StatsdMaxUDPPacketSize { get; set; }

        /// <summary>
        /// Allows you to optionally specify a stat name prefix for all your stats.
        /// Eg setting it to "Production.MyApp", then sending a counter with the name "Value" will result in a final stat name of "Production.MyApp.Value".
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Allows you to configure the Sender that is used to transmit data to StatsD.
        /// </summary>
        public ISender Sender { get; set; }

        /// <summary>
        /// Allows you to configure global tags which are appended to each metric.
        /// </summary>
        public IDictionary<string, object> GlobalTags { get; set; }

        public const int DefaultStatsdServerPort = 8125;
        public const int DefaultStatsdMaxUDPPacketSize = 512;

        public MetricsConfig()
        {
            StatsdServerPort = DefaultStatsdServerPort;
            StatsdMaxUDPPacketSize = DefaultStatsdMaxUDPPacketSize;
            Sender = null;
            GlobalTags = new Dictionary<string, object>();
        }
    }
}
