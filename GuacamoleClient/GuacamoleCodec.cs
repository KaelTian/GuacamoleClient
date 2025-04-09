namespace GuacamoleClient
{
    using System;
    using System.Text;

    /// <summary>
    /// Guacamole 连接字符串的编码和解码工具
    /// </summary>
    public static class GuacamoleCodec
    {
        // 编码分隔符（ASCII null 字符）
        private static readonly char Separator = '\0';

        /// <summary>
        /// 编码 Guacamole 连接字符串（标准 Base64）
        /// </summary>
        /// <param name="guacId">连接 ID（如 "2"）</param>
        /// <param name="guacType">连接类型（通常为 "c"）</param>
        /// <param name="dataSource">数据源（如 "postgresql"）</param>
        public static string Encode(string guacId, string guacType, string dataSource)
            => Encode(guacId, guacType, dataSource, urlSafe: false);

        /// <summary>
        /// 编码 Guacamole 连接字符串（可选 URL 安全 Base64）
        /// </summary>
        /// <param name="urlSafe">是否生成 URL 安全的 Base64（替换 +/ 为 -_，移除 =）</param>
        public static string Encode(string guacId, string guacType, string dataSource, bool urlSafe)
        {
            // 构造连接字符串："{GUAC_ID}\0{GUAC_TYPE}\0{DATA_SOURCE}"
            string connectionInfo = $"{guacId}{Separator}{guacType}{Separator}{dataSource}";

            // Base64 编码
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(connectionInfo));

            // 可选：URL 安全处理
            return urlSafe ? ToUrlSafeBase64(base64) : base64;
        }

        /// <summary>
        /// 解码 Guacamole 连接字符串（自动识别标准/URL 安全 Base64）
        /// </summary>
        /// <returns>(GUAC_ID, GUAC_TYPE, DATA_SOURCE)</returns>
        public static (string guacId, string guacType, string dataSource) Decode(string base64Encoded)
        {
            // 修复 URL 安全 Base64
            string standardBase64 = FromUrlSafeBase64(base64Encoded);

            // Base64 解码
            byte[] data = Convert.FromBase64String(standardBase64);
            string decoded = Encoding.UTF8.GetString(data);

            // 按 \0 分割字符串
            string[] parts = decoded.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                throw new FormatException("无效的 Guacamole 连接字符串格式！");

            return (parts[0], parts[1], parts[2]);
        }

        // 将标准 Base64 转为 URL 安全格式
        private static string ToUrlSafeBase64(string standardBase64)
            => standardBase64
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

        // 将 URL 安全 Base64 转为标准格式
        private static string FromUrlSafeBase64(string urlSafeBase64)
        {
            // 替换字符
            string standard = urlSafeBase64
                .Replace('-', '+')
                .Replace('_', '/');

            // 补足填充符
            switch (standard.Length % 4)
            {
                case 2: standard += "=="; break;
                case 3: standard += "="; break;
            }

            return standard;
        }
    }
}
