using NUnit.Framework;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProcessMonitor.Tests
{
    [TestFixture]
    public class ProcessMonitorTests
    {
        [Test]
        public async Task TestMonitorProcessesAsync_KillsLongRunningProcess()
        {
            // Arrange
            string processName = "notepad";
            int maxLifetime = 1; // 1 minute
            int monitoringFrequency = 1; // 1 minute

            // Start a process
            var process = Process.Start("notepad.exe");

            try
            {
                // Wait for process to exceed max lifetime
                await Task.Delay(2 * 60 * 1000); // 2 minutes

                // Act
                await ProcessMonitor.Program.MonitorProcessesAsync(processName, maxLifetime, monitoringFrequency, new System.Threading.CancellationToken());

                // Assert
                Assert.IsTrue(process.HasExited, "Process should have been killed.");
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
        }
    }
}
