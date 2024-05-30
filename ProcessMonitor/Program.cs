using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: monitor.exe <process_name> <max_lifetime_in_minutes> <monitoring_frequency_in_minutes>");
                return;
            }

            string processName = args[0];
            if (!int.TryParse(args[1], out int maxLifetime) || !int.TryParse(args[2], out int monitoringFrequency))
            {
                Console.WriteLine("Invalid arguments. Please enter numeric values for max lifetime and monitoring frequency.");
                return;
            }

            Console.WriteLine($"Monitoring process: {processName}, Max Lifetime: {maxLifetime} minutes, Monitoring Frequency: {monitoringFrequency} minutes");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task monitorTask = MonitorProcessesAsync(processName, maxLifetime, monitoringFrequency, cts.Token);

            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey().KeyChar != 'q') { }
            
            cts.Cancel();
            await monitorTask;
        }

        static async Task MonitorProcessesAsync(string processName, int maxLifetime, int monitoringFrequency, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    TimeSpan runtime = DateTime.Now - process.StartTime;
                    if (runtime.TotalMinutes > maxLifetime)
                    {
                        try
                        {
                            process.Kill();
                            Console.WriteLine($"Killed process {processName} (ID: {process.Id}) after running for {runtime.TotalMinutes} minutes.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error killing process {processName} (ID: {process.Id}): {ex.Message}");
                        }
                    }
                }
                await Task.Delay(monitoringFrequency * 60 * 1000, token);
            }
        }
    }
}
