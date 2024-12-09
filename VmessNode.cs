using System;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Web; // 引入 HttpUtility.UrlDecode 支持

namespace Befree
{
    public class VmessNode : Node
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string UUID { get; set; }
        public string Cipher { get; set; }



        private static string ExtractLocationName(string proxyName)
        {
            // 匹配连续的中文字符
            string pattern = @"[\u4e00-\u9fa5]{2,}";
            var match = Regex.Match(proxyName, pattern);
            return match.Success ? match.Value : string.Empty;
        }

        public static VmessNode FromBase64(string base64)
        {
            try
            {
                string sssx =  Program.cleanBase64String(base64);
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(sssx));
                //Console.WriteLine($"json:{json}");
                // 使用 System.Text.Json 解析 JSON 字符串
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    // 提取字段
                    var name = root.GetProperty("ps").GetString();
                    //Console.WriteLine($"name:{name}");
                    var server = root.GetProperty("add").GetString();
                    //Console.WriteLine($"server:{server}");
                    //var port = root.GetProperty("port").GetInt32();
                    //var port = int.Parse(root.GetProperty("port").GetString());
                    JsonElement portElement = root.GetProperty("port");
                    int port = 0;
                    if(portElement.ValueKind == JsonValueKind.String)
                    {
                        port = int.Parse(portElement.GetString());
                    }
                    else if (portElement.ValueKind == JsonValueKind.Number)
                    {
                        port = portElement.GetInt32();
                    }
                    //Console.WriteLine($"port:{port}");
                    var uuid = root.GetProperty("id").GetString();
                    //Console.WriteLine($"uuid:{uuid}");
                    var cipher = root.TryGetProperty("cipher", out var cipherProp) ? cipherProp.GetString() : "auto";
                    //Console.WriteLine($"cipher:{cipher}");

                    // 提取中文位置名称
                    string locationName = ExtractLocationName(name);
                    if (string.IsNullOrEmpty(locationName)){locationName = "xxxx";}

                    return new VmessNode
                    {
                        Name = locationName,
                        Server = server,
                        Port = port,
                        UUID = uuid,
                        Cipher = cipher
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] 发现一处Vmess节点 {HttpUtility.UrlDecode(base64)} 转换错误，非正常命名节点。");
                return null;
            }
        }

        public override object ToClashProxy()
        {
            // 创建字典并返回
            var proxyDict = new Dictionary<string, object>
            {
                { "name", Name },
                { "type", "vmess" },
                { "server", Server },
                { "port", Port },
                { "uuid", UUID },
                { "cipher", Cipher },
                { "alterId", 0 }
            };
            return proxyDict;
        }
    }
}