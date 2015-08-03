High Performance StatsD Client
==============================

A high performance StatsD client.  Increases performance over other clients with these key features:

**Metric Buffering**
Multiple metrics are bundled together into a single UDP packet.  This means fewer packets are sent to the server.

**Metric Aggregation**
When possible, metrics are aggregated in the client before sending to the server.  For example, if a counter is increased by 1 three times in quick succession, the client will only report the metric to the server once, with a value of 3.

**Thread-Safe**
The client is completely thread safe.  An unlimited number of worker threads in your application can all report metrics through the same instance of the client.

**Non-Blocking**
The client is non-blocking, so reporting a metric always returns immediately, without slowing down your application.

**Consumer/Producer Pattern**
The client uses a configurable thread pool of consumer worker threads to bundle up metrics and send them to the server.  In most cases a single worker thread is plenty, but if your application sends a high volume of metrics, simply change the configuration to use a higher number of workers.

Installation
------------

Can be found on Nuget with the ID of [StatsdClient.HighPerformance](https://www.nuget.org/packages/StatsdClient.HighPerformance)


Usage
-----

At start of your app, configure the `Metrics` class like this:

``` C#
var metricsConfig = new MetricsConfig
{
  StatsdServerName = "host.name",
  Prefix = "myApp",
  StatsdMaxUDPPacketSize = 512 // Optional, defaults to 512
};

StatsdClient.Metrics.Configure(metricsConfig);
```

Where *host.name* is the name of the statsd server and *myApp* is an optional prefix that is prepended onto the names of all metrics sent to the server.

Use it like this:

``` C#
Metrics.Counter("stat-name");
Metrics.Timer("stat-name", (int)stopwatch.ElapsedMilliseconds);
Metrics.Gauge("gauge-name", gaugeValue);
```

 And timing around blocks of code:

``` C#
using (Metrics.StartTimer("stat-name"))
{
  DoMagic();
}
```

And timing an action

``` C#
Metrics.Time(() => DoMagic(), "stat-name");
```

or replace a method that returns a value

``` C#
var result = GetResult();
```

with a timed `Func<T>` that returns the same value

``` C#
var result = Metrics.Time(() => GetResult(), "stat-name");
```

Advanced Configuration
----------------------
| Configuration Option | Default Value | Description                                                                                                                                                                                                                                                                                                                                      |
|----------------------|---------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MaxSendDelayMS       | 5000          | Maximum amount of time (in milliseconds), to wait for additional metrics to be sent before bundling up the metrics and sending them to the server.  It is important that this value is always smaller than the flush interval.                                                                                                                   |
| MaxThreads           | 1             | Number of worker threads that will be used to send metrics to StatsD.  In very high volume use cases, a single worker thread may not be able to keep up with all of the metrics to be sent.  In that case, the number of threads can be increased with this option.  In most cases, the default value of one worker thread should be sufficient. |


To configure these options, create an instance of the *ThreadSafeConsumerProducerSender* class using the appropriate configuration options, and use it as the value for the *Sender* property in your MetricsConfig object.

    var metricsConfig = new MetricsConfig
    {
      StatsdServerName = "host.name",
      Prefix = "myApp",
      Sender = new ThreadSafeConsumerProducerSender(
        new ThreadSafeConsumerProducerSender.Configuration() { 
          MaxSendDelayMS = 5000,
          MaxThreads = 3
    };
    Metrics.Configure(metricsConfig);

Credits
--------
This project is forked from Goncalo Pereira's [original Statsd client](https://github.com/goncalopereira/statsd-csharp-client).

Copyright (c) 2012 Goncalo Pereira and all contributors. See MIT-LICENCE.md for further details.

Thanks to Goncalo Pereira, Anthony Steele, Darrell Mozingo, Antony Denyer, and Tim Skauge for their contributions to the original client.

Ideas for client-side metric aggregation are based on [Hulu's Bank project](http://tech.hulu.com/blog/2013/07/09/bank-an-open-source-statsdmetricsd-aggregation-frontend/)