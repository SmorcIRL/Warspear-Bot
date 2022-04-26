using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WarspearBot
{
    public class PerformanceLogger : BackgroundService
    {
        private readonly ILogger<PerformanceLogger> _logger;

        public PerformanceLogger(ILogger<PerformanceLogger> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var (cpuCounter, ramCounter) = CreateCounters();
            var delay = TimeSpan.FromSeconds(20);

            while (!stoppingToken.IsCancellationRequested)
            {
                var cpuUsage = Math.Round(cpuCounter.NextValue() / Environment.ProcessorCount, 2);
                var ramUsage = Math.Round(ramCounter.NextValue() / 1024 / 1024, 2);

                _logger.LogDebug($"CPU: {cpuUsage}%, RAM: {ramUsage}MB");

                await Task.Delay(delay, stoppingToken);
            }
        }

        private (PerformanceCounter cpuCounter, PerformanceCounter ramCounter) CreateCounters()
        {
            var process = Process.GetCurrentProcess();
            var instanceName = string.Empty;

            var counterCategory = new PerformanceCounterCategory("Process");

            foreach (var name in counterCategory.GetInstanceNames().Where(x => x.StartsWith(process.ProcessName)))
            {
                using var processId = new PerformanceCounter("Process", "ID Process", name, true);

                if (process.Id == processId.RawValue)
                {
                    instanceName = name;
                    break;
                }
            }

            if (instanceName == string.Empty)
            {
                throw new InvalidOperationException("Unable to create performance counters");
            }

            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", instanceName, true);
            var ramCounter = new PerformanceCounter("Process", "Private Bytes", instanceName, true);

            return (cpuCounter, ramCounter);
        }
    }
}