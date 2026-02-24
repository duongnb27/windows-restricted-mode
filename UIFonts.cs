namespace RestrictedMode
{
    /// <summary>
    /// Setup font chữ / cỡ chữ toàn ứng dụng — chỉnh tại đây để áp dụng cho tất cả form.
    /// </summary>
    public static class UIFonts
    {
        /// <summary>Tên font (ví dụ: Segoe UI, Tahoma).</summary>
        public const string Family = "Segoe UI";

        /// <summary>Cỡ chữ bình thường (label, textbox, nút nhỏ...).</summary>
        public const float SizeNormal = 10F;

        /// <summary>Cỡ chữ nút lớn (Lưu cấu hình, Bắt đầu Restricted).</summary>
        public const float SizeButtonLarge = 12F;
    }
}
