using System.Diagnostics;
using System.Web; // 引入 HttpUtility.UrlDecode 支持

namespace Befree
{
    public class ShadowsocksNode : Node
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string Cipher { get; set; }

        public static ShadowsocksNode Parse(string ssLink)
        {
            try
            {
                string[] parts = ssLink.Split('#');
                var name = HttpUtility.UrlDecode(parts[1]).Trim();
                if (string.IsNullOrEmpty(name)){name = "xxxx";}
                string cipher = string.Empty, password = string.Empty, server = string.Empty, port = string.Empty;

                if (parts[0].Contains("@")){
                    string[] parts2 = parts[0].Split('@');
                    string[] cipher_password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Program.cleanBase64String(parts2[0]))).Split(':');
                    cipher = cipher_password[0];
                    password = cipher_password[1];
                    string[] server_port = parts2[1].Split(':');
                    server = server_port[0].Trim();
                    port = server_port[1].Trim();
                }
                else{
                    var parts2 = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Program.cleanBase64String(parts[0])));
                    string[] parts3 = parts2.Split('@');

                    string[] cipher_password = Program.cleanBase64String(parts3[0]).Split(':');
                    cipher = cipher_password[0];
                    password = cipher_password[1];
                    
                    string[] server_port = parts3[1].Split(':');
                    server = server_port[0].Trim();
                    port = server_port[1].Trim();
                }

                return new ShadowsocksNode
                {
                    Name = name,
                    Server = server,
                    Port = int.Parse(port),
                    Password = password,
                    Cipher = cipher
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] 发现一处Shadowsocks节点 {HttpUtility.UrlDecode(ssLink)} 转换错误，非正常命名节点。");
                return null;
            }
        }
        public override object ToClashProxy()
        {
            // 创建字典并返回
            var proxyDict = new Dictionary<string, object>
            {
                { "name", Name },
                { "type", "ss" },
                { "server", Server },
                { "port", Port },
                { "cipher", Cipher },
                { "password", Password }
            };
            return proxyDict;
        }
    }
}
