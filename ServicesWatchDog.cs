using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RestrictedMode
{
    /// <summary>
    /// Config for one process to keep running (restarted if killed).
    /// </summary>
    public class WatchDogEntry
    {
        /// <summary>
        /// Full path to .exe
        /// </summary>
        public string ExePath { get; set; }

        /// <summary>
        /// Command-line arguments (optional)
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Working directory (null = exe folder)
        /// </summary>
        public string WorkingDirectory { get; set; }

        public WatchDogEntry(string exePath, string arguments = null, string workingDirectory = null)
        {
            ExePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
        }

        /// <summary>
        /// Process name (no .exe) for run check.
        /// </summary>
        public string ProcessName => Path.GetFileNameWithoutExtension(ExePath);
    }

    /// <summary>
    /// Watchdog: periodically checks processes and restarts them if killed.
    /// Active only when <see cref="RestrictedState.IsRestrictedMode"/> is true.
    /// </summary>
    public class ServicesWatchDog
    {
        private readonly List<WatchDogEntry> _entries = new List<WatchDogEntry>();
        private System.Timers.Timer _timer;
        private readonly object _lock = new object();
        private bool _running;

        /// <summary>
        /// Check interval in ms (default 5000).
        /// </summary>
        public int CheckIntervalMs { get; set; } = 5000;

        /// <summary>
        /// Raised when a process is restarted (for log/debug).
        /// </summary>
        public event Action<WatchDogEntry, Exception> ProcessRestarted;

        /// <summary>
        /// Adds an exe to watch; restarts if killed.
        /// </summary>
        /// <param name="exePath">Full path to .exe</param>
        /// <param name="arguments">Command-line args (optional)</param>
        /// <param name="workingDirectory">Working dir (null = exe folder)</param>
        public void AddProcess(string exePath, string arguments = null, string workingDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(exePath)) return;
            lock (_lock)
            {
                _entries.Add(new WatchDogEntry(exePath, arguments, workingDirectory));
            }
        }

        /// <summary>
        /// Adds a pre-configured entry.
        /// </summary>
        public void AddProcess(WatchDogEntry entry)
        {
            if (entry == null) return;
            lock (_lock)
            {
                _entries.Add(entry);
            }
        }

        /// <summary>
        /// Clears all watched processes.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _entries.Clear();
            }
        }

        /// <summary>
        /// Starts watchdog (checks only in restricted mode).
        /// </summary>
        public void Start()
        {
            if (_timer != null) return;
            _running = true;
            _timer = new System.Timers.Timer(CheckIntervalMs);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        /// <summary>
        /// Stops watchdog.
        /// </summary>
        public void Stop()
        {
            _running = false;
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_running || !RestrictedState.IsRestrictedMode) return;

            List<WatchDogEntry> snapshot;
            lock (_lock)
            {
                if (_entries.Count == 0) return;
                snapshot = new List<WatchDogEntry>(_entries);
            }

            foreach (var entry in snapshot)
            {
                try
                {
                    if (!IsProcessRunning(entry))
                        RestartProcess(entry);
                }
                catch (Exception ex)
                {
                    ProcessRestarted?.Invoke(entry, ex);
                }
            }
        }

        private static bool IsProcessRunning(WatchDogEntry entry)
        {
            string name = entry.ProcessName;
            if (string.IsNullOrEmpty(name)) return false;
            var processes = Process.GetProcessesByName(name);
            try
            {
                return processes != null && processes.Length > 0;
            }
            finally
            {
                foreach (var p in processes ?? Enumerable.Empty<Process>())
                    p.Dispose();
            }
        }

        private void RestartProcess(WatchDogEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.ExePath) || !File.Exists(entry.ExePath))
            {
                ProcessRestarted?.Invoke(entry, new FileNotFoundException("Exe file not found.", entry.ExePath));
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = entry.ExePath,
                UseShellExecute = true,
                WorkingDirectory = string.IsNullOrWhiteSpace(entry.WorkingDirectory)
                    ? Path.GetDirectoryName(entry.ExePath)
                    : entry.WorkingDirectory
            };
            if (!string.IsNullOrWhiteSpace(entry.Arguments))
                startInfo.Arguments = entry.Arguments;

            Process.Start(startInfo);
            ProcessRestarted?.Invoke(entry, null);
        }
    }
}
