using System.Text.Json;
using System.Text;
using GuacamoleClient;

try
{
    // 标准 Base64
    string standardBase64 = GuacamoleCodec.Encode("2", "c", "postgresql");
    Console.WriteLine(standardBase64); // 输出: MgBjAHBvc3RncmVzcWw=

    // URL 安全 Base64（用于浏览器 URL）
    string urlSafeBase64 = GuacamoleCodec.Encode("2", "c", "postgresql", urlSafe: true);
    Console.WriteLine(urlSafeBase64); // 输出: MgBjAHBvc3RncmVzcWw

    // 解码标准 Base64
    var (id1, type1, source1) = GuacamoleCodec.Decode("MgBjAHBvc3RncmVzcWw=");
    Console.WriteLine($"ID: {id1}, Type: {type1}, Source: {source1}");
    // 输出: ID: 2, Type: c, Source: postgresql

    // 解码 URL 安全 Base64
    var (id2, type2, source2) = GuacamoleCodec.Decode("MgBjAHBvc3RncmVzcWw");
    Console.WriteLine($"ID: {id2}, Type: {type2}, Source: {source2}");
    // 输出: ID: 2, Type: c, Source: postgresql
}
catch (FormatException ex)
{
    Console.WriteLine($"Base64 解码失败: {ex.Message}");
}


Console.ReadLine();

string guacamoleUrl = "http://192.168.0.209:8080/guacamole"; // Guacamole 服务器地址
string username = "guacadmin";
string password = "guacadmin";
string authToken = string.Empty;
DateTime tokenExpiry = DateTime.MinValue;

try
{
    // 登录并获取 Token
    await RefreshAuthToken();

    // 创建新的 RDP 连接
    string connectionId = await CreateConnection("TestRDP", "192.168.0.209", "3389", "田赛", "ts6yhn7ujm^&*I") ?? throw new ArgumentNullException("ConnectionId获取失败");
    var base64ConnectionId = ConvertToGuacamoleId(connectionId);
    Console.WriteLine($"✅ 连接创建成功，Connection ID: {connectionId}");

    // 生成远程访问 URL
    string accessUrl = $"{guacamoleUrl}/#/client/{base64ConnectionId}?token={authToken}";
    Console.WriteLine($"🔗 访问 URL: {accessUrl}");

    Console.ReadLine();

    // 定时刷新 Token，防止过期
    await StartTokenRefreshLoop();

}
catch (Exception ex)
{
    Console.WriteLine("❌ 出现异常: " + ex.Message);
}

string ConvertToGuacamoleId(string id)
{
    byte[] bytes = Encoding.UTF8.GetBytes(id);
    return Convert.ToBase64String(bytes)
        .Replace("=", "")
        .Replace("/", "_")
        .Replace("+", "-");
}

async Task RefreshAuthToken()
{
    authToken = await GetAuthToken() ?? throw new ArgumentNullException("Token获取失败");
    tokenExpiry = DateTime.UtcNow.AddHours(1); // Token 1 小时过期
}

async Task<string?> GetAuthToken()
{
    using (HttpClient client = new HttpClient())
    {
        // 构建表单数据
        var formData = new FormUrlEncodedContent(new[]
        {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
        });
        HttpResponseMessage response = await client.PostAsync(guacamoleUrl + "/api/tokens", formData);
        // 检查响应状态
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            JsonDocument json = JsonDocument.Parse(responseBody);
            return json.RootElement.GetProperty("authToken").GetString();
        }
        else
        {
            Console.WriteLine("❌ 登录失败: " + responseBody);
            return null;
        }
    }
}

async Task<string?> CreateConnection(string name, string hostname, string port, string rdpUser, string rdpPass)
{
    using (HttpClient client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("Guacamole-Token", authToken);
        var createContent = GetConnection(name, hostname, port, rdpUser, rdpPass, ignoreCert: true);
        //var content = new StringContent(
        //    "{\"name\":\"" + name + "\",\"protocol\":\"rdp\",\"parentIdentifier\":\"ROOT\",\"parameters\":{\"hostname\":\"" + hostname + "\",\"port\":\"" + port + "\",\"username\":\"" + rdpUser + "\",\"password\":\"" + rdpPass + "\"}}",
        //    Encoding.UTF8, "application/json");
        var content = new StringContent(createContent, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(guacamoleUrl + "/api/session/data/postgresql/connections", content);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            JsonDocument json = JsonDocument.Parse(responseBody);
            return json.RootElement.GetProperty("identifier").GetString();
        }
        else
        {
            Console.WriteLine("❌ 创建连接失败: " + responseBody);
            return null;
        }
    }
}

string GetConnection(string name, string hostname, string port, string username, string password, bool ignoreCert = false)
{
    var connection = new
    {
        parentIdentifier = "ROOT",
        name = name,
        protocol = "rdp",
        parameters = new
        {
            port = port,
            readOnly = "",
            swapRedBlue = "",
            cursor = "",
            colorDepth = "",
            forceLossless = "",
            clipboardEncoding = "",
            disableCopy = "",
            disablePaste = "",
            destPort = "",
            recordingExcludeOutput = "",
            recordingExcludeMouse = "",
            recordingIncludeKeys = "",
            createRecordingPath = "",
            enableSftp = "",
            sftpPort = "",
            sftpServerAliveInterval = "",
            sftpDisableDownload = "",
            sftpDisableUpload = "",
            enableAudio = "",
            wolSendPacket = "",
            wolUdpPort = "",
            wolWaitTime = "",
            security = "",
            disableAuth = "",
            ignoreCert = ignoreCert ? "true" : "",
            gatewayPort = "",
            serverLayout = "",
            timezone = (object)null,
            enableTouch = "",
            console = "",
            width = "",
            height = "",
            dpi = "",
            resizeMethod = "",
            normalizeClipboard = "",
            consoleAudio = "",
            disableAudio = "",
            enableAudioInput = "",
            enablePrinting = "",
            enableDrive = "",
            disableDownload = "",
            disableUpload = "",
            createDrivePath = "",
            enableWallpaper = "",
            enableTheming = "",
            enableFontSmoothing = "",
            enableFullWindowDrag = "",
            enableDesktopComposition = "",
            enableMenuAnimations = "",
            disableBitmapCaching = "",
            disableOffscreenCaching = "",
            disableGlyphCaching = "",
            preconnectionId = "",
            recordingExcludeTouch = "",
            hostname = hostname,
            username = username,
            password = password
        },
        attributes = new
        {
            maxConnections = "",
            maxConnectionsPerUser = "",
            weight = "",
            failoverOnly = "",
            guacdPort = "",
            guacdEncryption = ""
        }
    };

    return JsonSerializer.Serialize(connection, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}

async Task StartTokenRefreshLoop()
{
    while (true)
    {
        if (DateTime.UtcNow >= tokenExpiry)
        {
            Console.WriteLine("🔄 Token 过期，重新登录...");
            await RefreshAuthToken();
            Console.WriteLine("✅ Token 刷新成功");
        }
        await Task.Delay(TimeSpan.FromMinutes(55)); // 每 55 分钟刷新 Token
    }
}
