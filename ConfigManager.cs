using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace RestrictedMode
{
    /// <summary>
    /// Cấu hình tổ hợp phím thoát restricted (lưu dạng tên phím string).
    /// </summary>
    public class ExitHotkeyConfig
    {
        public string Key { get; set; } = "F12";
        public bool Ctrl { get; set; } = true;
        public bool Shift { get; set; } = true;
        public bool Alt { get; set; } = false;
    }

    /// <summary>
    /// Một tiến trình trong watchdog (cho serialization).
    /// </summary>
    public class WatchDogProcessConfig
    {
        public string ExePath { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
    }

    /// <summary>
    /// Cấu hình watchdog.
    /// </summary>
    public class WatchDogConfig
    {
        public int CheckIntervalMs { get; set; } = 5000;
        public WatchDogProcessConfig[] Processes { get; set; } = new WatchDogProcessConfig[0];
    }

    /// <summary>
    /// Cấu hình toàn bộ ứng dụng restricted.
    /// </summary>
    public class AppConfig
    {
        public ExitHotkeyConfig ExitHotkey { get; set; } = new ExitHotkeyConfig();
        public WatchDogConfig WatchDog { get; set; } = new WatchDogConfig();
        /// <summary>Mật khẩu để vào/thoát Restricted Mode (để trống là không yêu cầu).</summary>
        public string RestrictedPassword { get; set; }
    }

    /// <summary>
    /// Đọc/ghi config từ config.json cùng thư mục exe; nội dung file được mã hóa bằng key hardcode.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly byte[] Key;
        private static readonly byte[] Iv;
        private static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        static ConfigManager()
        {
            // Key hardcode: 32 bytes cho AES-256, 16 bytes cho IV
            byte[] keyBytes = Encoding.UTF8.GetBytes("RestrictedModeConfig32BytesKey!!123456");
            byte[] ivBytes = Encoding.UTF8.GetBytes("RestrictedModeIV16!!");
            Key = new byte[32];
            Iv = new byte[16];
            Array.Copy(keyBytes, Key, Math.Min(32, keyBytes.Length));
            Array.Copy(ivBytes, Iv, Math.Min(16, ivBytes.Length));
        }

        /// <summary>Đường dẫn file config (cùng thư mục với exe).</summary>
        public static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        /// <summary>Đọc config từ file (giải mã nếu file tồn tại), nếu lỗi hoặc không có file thì trả về config mặc định.</summary>
        public static AppConfig Load()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return GetDefault();
                string raw = File.ReadAllText(ConfigPath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(raw)) return GetDefault();
                string json = Decrypt(raw);
                if (string.IsNullOrEmpty(json)) return GetDefault();
                var config = Json.Deserialize<AppConfig>(json);
                return config ?? GetDefault();
            }
            catch
            {
                return GetDefault();
            }
        }

        /// <summary>Ghi config ra file (mã hóa).</summary>
        public static void Save(AppConfig config)
        {
            if (config == null) return;
            try
            {
                string json = Json.Serialize(config);
                string encrypted = Encrypt(json);
                File.WriteAllText(ConfigPath, encrypted, Encoding.UTF8);
            }
            catch { /* ignore */ }
        }

        /// <summary>Config mặc định.</summary>
        public static AppConfig GetDefault()
        {
            return new AppConfig
            {
                ExitHotkey = new ExitHotkeyConfig { Key = "F12", Ctrl = true, Shift = true, Alt = false },
                WatchDog = new WatchDogConfig { CheckIntervalMs = 5000, Processes = new WatchDogProcessConfig[0] },
                RestrictedPassword = null
            };
        }

        private static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = Iv;
                using (var enc = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                        sw.Write(plainText);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private static string Decrypt(string cipherBase64)
        {
            if (string.IsNullOrWhiteSpace(cipherBase64)) return string.Empty;
            try
            {
                byte[] data = Convert.FromBase64String(cipherBase64);
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = Iv;
                    using (var dec = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(data))
                    using (var cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Encoding.UTF8))
                        return sr.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
