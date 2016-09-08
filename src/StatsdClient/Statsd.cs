using StatsdClient.MetricTypes;
using StatsdClient.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace StatsdClient
{
    public class Statsd : IStatsd
    {
        private readonly Configuration _config = null;

        public Statsd(Configuration config)
        {
            if (config.Udp == null)
                throw new ArgumentNullException("Configuration.Udp");

            if (config.Sender == null)
                config.Sender = new ThreadSafeConsumerProducerSender();
            config.Sender.StatsdUDP = config.Udp;

            if(config.RandomGenerator == null)
                config.RandomGenerator = new RandomGenerator();
            if(config.StopwatchFactory == null)
                config.StopwatchFactory = new StopWatchFactory();
            if (config.Prefix == null)
                config.Prefix = string.Empty;
            else
                config.Prefix = config.Prefix.TrimEnd('.');
            _config = config;
        }
        
        public void Send<TCommandType>(string name, int value, object tags = null) where TCommandType : Metric, IAllowsInteger, new()
        {
            _config.Sender.Send(new TCommandType() { Name = BuildNamespacedStatName(name, tags), ValueAsInt = value });
        }

        public void Send<TCommandType>(string name, double value, object tags = null) where TCommandType : Metric, IAllowsDouble, new()
        {
            _config.Sender.Send(new TCommandType() { Name = BuildNamespacedStatName(name, tags), ValueAsDouble = value });
        }

        public void Send<TCommandType>(string name, string value, object tags = null) where TCommandType : Metric, IAllowsString, new()
        {
            _config.Sender.Send(new TCommandType() { Name = BuildNamespacedStatName(name, tags), Value = value });
        }

        public void Send<TCommandType>(string name, int value, double sampleRate, object tags = null) where TCommandType : Metric, IAllowsInteger, IAllowsSampleRate, new()
        {
            if (_config.RandomGenerator.ShouldSend(sampleRate))
            {
                _config.Sender.Send(new TCommandType() { Name = BuildNamespacedStatName(name, tags), ValueAsInt = value, SampleRate = sampleRate });
            }
        }

        public void Send(Action actionToTime, string statName, double sampleRate = 1, object tags = null)
        {
            var stopwatch = _config.StopwatchFactory.Get();

            try
            {
                stopwatch.Start();
                actionToTime();
            }
            finally
            {
                stopwatch.Stop();
                if (_config.RandomGenerator.ShouldSend(sampleRate))
                {
                    Send<Timing>(statName, stopwatch.ElapsedMilliseconds(), tags);
                }
            }
        }

        private string BuildNamespacedStatName(string statName, object tags)
        {
            if (!String.IsNullOrWhiteSpace(_config.Prefix))
                statName = String.Format("{0}.{1}", _config.Prefix, statName);

            if (tags != null)
            {
                statName = AnonymousToDictionary(tags)
                    .Aggregate(statName, (stat, tag) => String.Format("{0},{1}={2}", stat, tag.Key, tag.Value));
            }

            return statName;
        }

        private IDictionary<string, object> AnonymousToDictionary(object obj)
        {
            if (obj == null)
                return new Dictionary<string, object>();

            return obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .ToDictionary(property => property.Name, property => property.GetValue(obj, null));
        }

        public class Configuration
        {
            public IStopWatchFactory StopwatchFactory { get; set; }
            public IStatsdUDP Udp { get; set; }
            public IRandomGenerator RandomGenerator { get; set; }
            public ISender Sender { get; set; }
            public string Prefix { get; set; }
        }

        #region Backward Compatibility
        public class Counting : MetricTypes.Counting { }
        public class Gauge : MetricTypes.Gauge { }
        public class Histogram : MetricTypes.Histogram { }
        public class Meter : MetricTypes.Meter { }
        public class Set : MetricTypes.Set { }
        public class Timing : MetricTypes.Timing { }
        #endregion

    }
}
