﻿using StatsdClient.MetricTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace StatsdClient
{
    public class NullStatsd : IStatsd
    {

        public NullStatsd()
        {
        }

        public void Send<TCommandType>(string name, int value, object tags = null) where TCommandType : Metric, IAllowsInteger, new()
        {
        }

        public void Send<TCommandType>(string name, double value, object tags = null) where TCommandType : Metric, IAllowsDouble, new()
        {
        }

        public void Send<TCommandType>(string name, string value, object tags = null) where TCommandType : Metric, IAllowsString, new()
        {
        }

        public void Send<TCommandType>(string name, int value, double sampleRate, object tags = null) where TCommandType : Metric, IAllowsInteger, IAllowsSampleRate, new()
        {
        }

        public void Send(Action actionToTime, string statName, double sampleRate = 1, object tags = null)
        {
            actionToTime();
        }
    }
}
