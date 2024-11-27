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
                // 使用 Split 方法提取 @ 前的部分
                string[] parts = ssLink.Split('@');
                var decodedString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
                string[] parts123 = parts[1].Split('#');
                string[] parts1234 = parts123[0].Split(':');
                string[] passandcipher = decodedString.Split(':');

                return new ShadowsocksNode
                {
                    Name = HttpUtility.UrlDecode(parts123[1]).Trim(),
                    Server = parts1234[0],
                    Port = int.Parse(parts1234[1]),
                    Password = passandcipher[1],
                    Cipher = passandcipher[0]
                };
            }
            catch (Exception ex)
            {
                //Console.WriteLine($" [-] Error parsing Shadowsocks node: {ex.Message}");
                throw;
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
