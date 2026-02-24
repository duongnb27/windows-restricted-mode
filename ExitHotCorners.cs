using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// When cursor enters the configured corner (or user taps/clicks the corner), triggers exit restricted flow.
    /// Uses an always-on-top overlay so the hot corner is clickable even when other apps are on top (e.g. touch screens).
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
        private HotCornerOverlayForm _overlay;

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
                EnsureOverlay();
                if (_overlay != null)
                    _overlay.UpdatePosition(_cornerIndex, _sizePx);
                if (!_timer.Enabled)
                    _timer.Start();
            }
            else
            {
                _timer.Stop();
                DestroyOverlay();
            }
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
            DestroyOverlay();
        }

        private void EnsureOverlay()
        {
            if (_overlay != null && !_overlay.IsDisposed)
                return;
            _overlay = new HotCornerOverlayForm(_invokeTarget, () => TriggerExitRequested());
            _overlay.UpdatePosition(_cornerIndex, _sizePx);
            _overlay.Show();
        }

        private void DestroyOverlay()
        {
            if (_overlay == null) return;
            try
            {
                if (!_overlay.IsDisposed)
                    _overlay.Dispose();
            }
            finally
            {
                _overlay = null;
            }
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
            DestroyOverlay();
            _disposed = true;
        }

        /// <summary>
        /// Small always-on-top form in the hot corner so taps/clicks always hit this window (works with touch when other apps are on top).
        /// </summary>
        private sealed class HotCornerOverlayForm : Form
        {
            private readonly Control _invokeTarget;
            private readonly Action _onActivated;
            private int _cornerIndex;
            private int _sizePx;

            public HotCornerOverlayForm(Control invokeTarget, Action onActivated)
            {
                _invokeTarget = invokeTarget ?? throw new ArgumentNullException(nameof(invokeTarget));
                _onActivated = onActivated ?? throw new ArgumentNullException(nameof(onActivated));
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                TopMost = true;
                StartPosition = FormStartPosition.Manual;
                Opacity = 0.01;
                BackColor = Color.Black;
                Size = new Size(1, 1);
                _cornerIndex = 0;
                _sizePx = 50;

                MouseEnter += (s, e) => FireExit();
                MouseDown += (s, e) => FireExit();
                Click += (s, e) => FireExit();
            }

            public void UpdatePosition(int cornerIndex, int sizePx)
            {
                _cornerIndex = cornerIndex;
                _sizePx = sizePx;
                var screen = Screen.PrimaryScreen;
                if (screen == null) return;
                Rectangle b = screen.Bounds;
                Size = new Size(_sizePx, _sizePx);
                switch (_cornerIndex)
                {
                    case 0: Location = new Point(b.Left, b.Top); break;
                    case 1: Location = new Point(b.Right - _sizePx, b.Top); break;
                    case 2: Location = new Point(b.Left, b.Bottom - _sizePx); break;
                    case 3: Location = new Point(b.Right - _sizePx, b.Bottom - _sizePx); break;
                    default: Location = new Point(b.Left, b.Top); break;
                }
            }

            private void FireExit()
            {
                if (!RestrictedState.IsRestrictedMode) return;
                if (_invokeTarget.IsDisposed) return;
                if (_invokeTarget.InvokeRequired)
                    _invokeTarget.BeginInvoke(new Action(_onActivated));
                else
                    _onActivated();
            }
        }
    }
}
