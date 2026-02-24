using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RestrictedMode
{
    /// <summary>
    /// Cấu hình một tiến trình exe cần chạy liên tục (nếu bị tắt sẽ được khởi động lại).
    /// </summary>
    public class WatchDogEntry
    {
        /// <summary>Đường dẫn đầy đủ tới file .exe</summary>
        public string ExePath { get; set; }

        /// <summary>Tham số dòng lệnh (tùy chọn)</summary>
        public string Arguments { get; set; }

        /// <summary>Thư mục làm việc khi chạy (null = dùng thư mục chứa exe)</summary>
        public string WorkingDirectory { get; set; }

        public WatchDogEntry(string exePath, string arguments = null, string workingDirectory = null)
        {
            ExePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
        }

        /// <summary>Tên tiến trình (không có .exe) dùng để kiểm tra còn chạy không.</summary>
        public string ProcessName => Path.GetFileNameWithoutExtension(ExePath);
    }

    /// <summary>
    /// Watchdog: kiểm tra định kỳ các tiến trình exe cần chạy liên tục.
    /// Nếu phát hiện tiến trình bị tắt thì tự động khởi động lại.
    /// Chỉ hoạt động khi <see cref="RestrictedState.IsRestrictedMode"/> = true.
    /// </summary>
    public class ServicesWatchDog
    {
        private readonly List<WatchDogEntry> _entries = new List<WatchDogEntry>();
        private System.Timers.Timer _timer;
        private readonly object _lock = new object();
        private bool _running;

        /// <summary>Chu kỳ kiểm tra (ms). Mặc định 5000 (5 giây).</summary>
        public int CheckIntervalMs { get; set; } = 5000;

        /// <summary>Sự kiện khi khởi động lại một tiến trình (để log/debug).</summary>
        public event Action<WatchDogEntry, Exception> ProcessRestarted;

        /// <summary>Thêm một exe cần theo dõi và tự khởi động lại nếu bị tắt.</summary>
        /// <param name="exePath">Đường dẫn đầy đủ tới file .exe</param>
        /// <param name="arguments">Tham số dòng lệnh (tùy chọn)</param>
        /// <param name="workingDirectory">Thư mục làm việc (null = thư mục chứa exe)</param>
        public void AddProcess(string exePath, string arguments = null, string workingDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(exePath)) return;
            lock (_lock)
            {
                _entries.Add(new WatchDogEntry(exePath, arguments, workingDirectory));
            }
        }

        /// <summary>Thêm một entry đã cấu hình.</summary>
        public void AddProcess(WatchDogEntry entry)
        {
            if (entry == null) return;
            lock (_lock)
            {
                _entries.Add(entry);
            }
        }

        /// <summary>Xóa tất cả danh sách tiến trình theo dõi.</summary>
        public void Clear()
        {
            lock (_lock)
            {
                _entries.Clear();
            }
        }

        /// <summary>Bắt đầu watchdog (chỉ kiểm tra khi đang ở restricted mode).</summary>
        public void Start()
        {
            if (_timer != null) return;
            _running = true;
            _timer = new System.Timers.Timer(CheckIntervalMs);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        /// <summary>Dừng watchdog.</summary>
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
                // Có thể có nhiều process cùng tên; nếu cần chặt chẽ hơn thì so sánh đường dẫn exe
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
                ProcessRestarted?.Invoke(entry, new FileNotFoundException("Không tìm thấy file exe.", entry.ExePath));
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
