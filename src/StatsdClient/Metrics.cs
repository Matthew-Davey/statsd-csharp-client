using StatsdClient.MetricTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StatsdClient
{
    public static class Metrics
    {
        private static IStatsd _statsD = new NullStatsd();
        private static StatsdUDP _statsdUdp;
        private static string _prefix;

        public static void Configure(MetricsConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _prefix = config.Prefix ?? "";
            _prefix = _prefix.TrimEnd('.');
            CreateStatsD(config);
        }

        private static void CreateStatsD(MetricsConfig config)
        {
            if (_statsdUdp != null)
            {
                _statsdUdp.Dispose();
            }

            _statsdUdp = null;

            if (!string.IsNullOrEmpty(config.StatsdServerName))
            {
                _statsdUdp = new StatsdUDP(config.StatsdServerName, config.StatsdServerPort, config.StatsdMaxUDPPacketSize);
                _statsD = new Statsd(new Statsd.Configuration() { Udp = _statsdUdp, Sender = config.Sender, Prefix = _prefix, GlobalTags = config.GlobalTags });
            }
        }

        public static void Counter(string statName, int value = 1, object tags = null, double sampleRate = 1)
        {
            _statsD.Send<Counting>(statName, value, sampleRate, tags);
        }

        public static void Gauge(string statName, double value, object tags = null)
        {
            _statsD.Send<Gauge>(statName, value, tags);
        }

        public static void Timer(string statName, int value, object tags = null, double sampleRate = 1)
        {
            _statsD.Send<Timing>(statName, value, sampleRate, tags);
        }

        public static IDisposable StartTimer(string name, object tags = null)
        {
            return new MetricsTimer(name, tags);
        }

        public static void Time(Action action, string statName, object tags = null, double sampleRate = 1) 
        {
            _statsD.Send(action, statName, sampleRate, tags);
        }

        public static T Time<T>(Func<T> func, string statName,  object tags = null)
        {
            using (StartTimer(statName, tags))
            {
                return func();
            }
        }

        public static void Set(string statName, string value, object tags = null)
        {
            _statsD.Send<Set>(statName, value, tags);
        }
    }
}
