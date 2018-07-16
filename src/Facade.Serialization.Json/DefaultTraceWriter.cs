using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Facade.Serialization.Json
{
    internal class DefaultTraceWriter : ITraceWriter
    {
        private ILogger logger;

        public DefaultTraceWriter(ILoggerFactory loggerFactory, TraceLevel levelFilter = TraceLevel.Verbose)
        {
            this.logger = loggerFactory.CreateLogger<DefaultTraceWriter>();
            this.LevelFilter = levelFilter;
        }

        public TraceLevel LevelFilter { get; }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    this.logger.LogError(message, ex);
                    break;
                case TraceLevel.Info:
                    this.logger.LogInformation(message, ex);
                    break;
                case TraceLevel.Off:
                    this.logger.LogTrace(message);
                    break;
                case TraceLevel.Verbose:
                    this.logger.LogTrace(message);
                    break;
                case TraceLevel.Warning:
                    this.logger.LogWarning(message);
                    break;
                default:
                    break;
            }
        }
    }
}
