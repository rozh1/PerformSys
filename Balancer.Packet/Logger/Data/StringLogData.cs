﻿using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger.Data
{
    public class StringLogData : ILogData
    {
        public StringLogData(string text)
        {
            DataParams = new[] {text};
        }

        public string[] DataParams { get; set; }
    }
}