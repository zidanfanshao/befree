using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace Befree
{
    public class ShadowsocksRNode : Node
    {
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public string Method { get; set; }
        public string Obfs { get; set; }
        public string ObfsParam { get; set; }
        public string Remarks { get; set; }
        public string Group { get; set; }

        // SSR URL格式: ssr://<base64 encoded>


        private static bool IsValidServer(string server)
        {
            if(string.IsNullOrWhiteSpace(server)) return false;
            if (Regex.IsMatch(server, @"^((25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$"))
                return true;
            // 检查是否为域名
            if (Regex.IsMatch(server, @"^(?=.{1,255}$)[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z]{2,})+$"))
                return true;

            return false;
        }
        public static ShadowsocksRNode Parse(string url)
        {
            try
            {
                // URL 解码并处理 Base64 字符串的 padding 问题
                string urlDecoded = HttpUtility.UrlDecode(url);
                int paddingNeeded = 4 - (urlDecoded.Length % 4);
                if (paddingNeeded < 4)
                {
                    urlDecoded = urlDecoded.PadRight(urlDecoded.Length + paddingNeeded, '=');
                }

                string decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(urlDecoded));

                // 解析节点数据
                string[] parts = decodedString.Split(':');

                // 提取基础信息
                var server = parts[0];
                if (!IsValidServer(server))
                {
                    //Console.WriteLine($"Invalid server: {server}");
                    return null; // 如果 server 无效，直接返回 null
                }
                //Console.WriteLine($"server: {server}");
                var port = int.Parse(parts[1]);
                //Console.WriteLine($"port: {port}");

                var protocol = parts[2];
                //Console.WriteLine($"protocol: {protocol}");

                var method = parts[3];
                //Console.WriteLine($"method: {method}");

                var obfs = parts[4];
                //Console.WriteLine($"obfs: {obfs}");
                
                string password = Encoding.UTF8.GetString(Convert.FromBase64String(parts[5].Split('/')[0]));
                //Console.WriteLine($"password: {password}");

                // 获取额外参数（remarks 和 group）
                var remarkGroupParams = HttpUtility.ParseQueryString(parts[5].Split('?')[1]);

                string remarks = Encoding.UTF8.GetString(Convert.FromBase64String(remarkGroupParams["remarks"].Replace('-','+')));
                //Console.WriteLine($"remarks: {remarks}");

                return new ShadowsocksRNode
                {
                    Server = server,
                    Port = port,
                    Protocol = protocol,
                    Method = method,
                    Password = password,
                    Obfs = obfs,
                    Name = remarks
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing SSR node: {ex.Message}");
                return null;  // 或抛出异常，取决于需求
            }
        }

        // 将 SSR 节点转换为 Clash 配置格式
        public override object ToClashProxy()
        {
            try
            {
                var proxyConfig = new Dictionary<string, object>
                {
                    { "name", Name },
                    { "type", "ssr" },
                    { "server", Server },
                    { "port", Port },
                    { "password", Password },
                    { "protocol", Protocol },
                    { "method", Method ?? "aes-256-cfb" },  // 默认加密方式
                    { "obfs", Obfs ?? "http" },  // 默认 obfs
                    { "obfs-param", ObfsParam ?? "" }, // 如果没有 obfs-param，传入空字符串
                    { "skip-cert-verify", true }
                };
                //Console.WriteLine($"[+] 生成 Clash 配置: {string.Join(", ", proxyConfig.Select(kv => $"{kv.Key}={kv.Value}"))}");
                return proxyConfig;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[-] ToClashProxy 出现异常: {ex.Message}");
                return null;
            }
        }
    }
}
