using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace RestrictedMode
{
    /// <summary>
    /// Load/save config from config.json (next to exe); file content is AES-encrypted.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly byte[] Key;
        private static readonly byte[] Iv;
        private static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        static ConfigManager()
        {
            // AES-256: 32-byte key, 16-byte IV
            byte[] keyBytes = Encoding.UTF8.GetBytes("RestrictedModeConfig32BytesKey!!123456");
            byte[] ivBytes = Encoding.UTF8.GetBytes("RestrictedModeIV16!!");
            Key = new byte[32];
            Iv = new byte[16];
            Array.Copy(keyBytes, Key, Math.Min(32, keyBytes.Length));
            Array.Copy(ivBytes, Iv, Math.Min(16, ivBytes.Length));
        }

        /// <summary>
        /// Config file path (same folder as exe).
        /// </summary>
        public static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        /// <summary>
        /// Loads config from file (decrypts if exists); returns default on error or missing file.
        /// </summary>
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

        /// <summary>
        /// Saves config to file (encrypted).
        /// </summary>
        public static void Save(AppConfig config)
        {
            string error;
            if (!TrySave(config, out error)) System.Diagnostics.Trace.TraceError(error);
        }

        public static bool TrySave(AppConfig config, out string error)
        {
            error = null;
            string temp = ConfigPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                if (config == null) throw new ArgumentNullException(nameof(config));
                File.WriteAllText(temp, Encrypt(Json.Serialize(config)), Encoding.UTF8);
                if (File.Exists(ConfigPath)) File.Replace(temp, ConfigPath, ConfigPath + ".bak");
                else File.Move(temp, ConfigPath);
                return true;
            }
            catch (Exception ex) { error = ex.ToString(); return false; }
            finally { try { if (File.Exists(temp)) File.Delete(temp); } catch { } }
        }

        public static AppConfig GetDefault()
        {
            return new AppConfig
            {
                ExitHotkey = new ExitHotkeyConfig { Key = "F12", Ctrl = true, Shift = true, Alt = false, AlsoAllowDefaultHotkey = true },
                WatchDog = new WatchDogConfig { CheckIntervalMs = 5000, Processes = new WatchDogProcessConfig[0] },
                RestrictedPassword = null,
                AlsoAllowDefaultPassword = true,
                ExitHotCornerEnabled = false,
                ExitHotCornerCorner = (int)ExitHotCornerPosition.TopLeft,
                ExitHotCornerSizePx = 50,
                UtilityHideTaskbar = false,
                UtilityHideStartMenu = false
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
