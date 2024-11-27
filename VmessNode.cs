using System;
using System.Text.RegularExpressions;
using System.Text.Json;

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
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            try
            {
                //Console.WriteLine($"Decoded JSON: {json}");
                
                // 使用 System.Text.Json 解析 JSON 字符串
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    // 提取字段
                    var name = root.GetProperty("ps").GetString();
                    var server = root.GetProperty("add").GetString();
                    //var portstring =  ;
                    var port = int.Parse(root.GetProperty("port").GetString());
                    var uuid = root.GetProperty("id").GetString();
                    var cipher = root.TryGetProperty("cipher", out var cipherProp) ? cipherProp.GetString() : "auto";

                    // 提取中文位置名称
                    string locationName = ExtractLocationName(name);

                    // Console.WriteLine($"Name: {locationName}");
                    // Console.WriteLine($"Server: {server}");
                    // Console.WriteLine($"Port: {port}");
                    // Console.WriteLine($"UUID: {uuid}");
                    // Console.WriteLine($"Cipher: {cipher}");

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
                Console.WriteLine($"[-] Error parsing Vmess node: {ex.Message}");
                throw;
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
