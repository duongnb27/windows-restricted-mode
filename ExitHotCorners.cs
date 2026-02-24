using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// When cursor enters the configured corner, triggers exit restricted flow (password dialog). Touch that moves the cursor is also detected.
    /// </summary>
    public class ExitHotCorners : IDisposable
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        #endregion

        private readonly Timer _timer;
        private readonly Control _invokeTarget;
        private bool _lastInCorner;
        private bool _enabled;
        private int _cornerIndex;
        private int _sizePx;
        private bool _disposed;

        public ExitHotCorners(Control invokeTarget)
        {
            _invokeTarget = invokeTarget ?? throw new ArgumentNullException(nameof(invokeTarget));
            _timer = new Timer { Interval = 150 };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// cornerIndex: 0=TopLeft, 1=TopRight, 2=BottomLeft, 3=BottomRight.
        /// </summary>
        public void Update(bool enabled, int cornerIndex, int sizePx)
        {
            _enabled = enabled && RestrictedState.IsRestrictedMode;
            _cornerIndex = Math.Max(0, Math.Min(3, cornerIndex));
            _sizePx = Math.Max(20, Math.Min(200, sizePx));
            _lastInCorner = false;

            if (_enabled)
            {
                if (!_timer.Enabled)
                    _timer.Start();
            }
            else
                _timer.Stop();
        }

        public void Start(bool enabled, int cornerIndex, int sizePx)
        {
            Update(enabled, cornerIndex, sizePx);
        }

        public void Stop()
        {
            _timer.Stop();
            _enabled = false;
            _lastInCorner = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_disposed || !_enabled || !RestrictedState.IsRestrictedMode)
            {
                if (!RestrictedState.IsRestrictedMode)
                    Stop();
                return;
            }

            if (!GetCursorPos(out POINT pt))
                return;

            var screen = Screen.PrimaryScreen;
            if (screen == null)
                return;

            Rectangle bounds = screen.Bounds;
            bool inCorner = IsInCorner(pt.X, pt.Y, bounds, _cornerIndex, _sizePx);

            if (inCorner && !_lastInCorner)
            {
                _lastInCorner = true;
                TriggerExitRequested();
            }
            else if (!inCorner)
            {
                _lastInCorner = false;
            }
        }

        private static bool IsInCorner(int x, int y, Rectangle bounds, int cornerIndex, int sizePx)
        {
            switch (cornerIndex)
            {
                case 0:
                    return x >= bounds.Left && x < bounds.Left + sizePx && y >= bounds.Top && y < bounds.Top + sizePx;
                case 1:
                    return x >= bounds.Right - sizePx && x <= bounds.Right && y >= bounds.Top && y < bounds.Top + sizePx;
                case 2:
                    return x >= bounds.Left && x < bounds.Left + sizePx && y >= bounds.Bottom - sizePx && y <= bounds.Bottom;
                case 3:
                    return x >= bounds.Right - sizePx && x <= bounds.Right && y >= bounds.Bottom - sizePx && y <= bounds.Bottom;
                default:
                    return false;
            }
        }

        private void TriggerExitRequested()
        {
            if (_invokeTarget.IsDisposed)
                return;
            if (_invokeTarget.InvokeRequired)
                _invokeTarget.BeginInvoke(new Action(RestrictedState.RequestExitRestricted));
            else
                RestrictedState.RequestExitRestricted();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _timer.Stop();
            _timer.Dispose();
            _disposed = true;
        }
    }
}
