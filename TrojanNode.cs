using System.Text.RegularExpressions;
using System.Web;

namespace Befree
{
    public class TrojanNode : Node
    {
        public string Password { get; set; }
        public int Port { get; set; }
        public string Sni { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
  
        public static TrojanNode Parse(string url)
        {
            try
            {
                string[] a = url.Split('@');
                var password = a[0];
                var host = a[1].Split(':')[0];
                var port = a[1].Split(':')[1].Split('?')[0];
                var peer = a[1].Split(':')[1].Split('?')[1].Split('#')[0];
                var remark = a[1].Split(':')[1].Split('?')[1].Split('#')[1];
                return new TrojanNode
                {
                    Name = remark,
                    Password = password,
                    Server = host,
                    Port = int.Parse(port),
                    Sni = peer
                    
                };

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error parsing TrojanNode node: {ex.Message}");
                throw;
            }
        }
        public override object ToClashProxy()
        {
            var proxyConfig = new Dictionary<string, object>
            {
                { "name", Name },
                { "type", "trojan" },
                { "server", Server },
                { "port", Port },
                { "sni", Sni },
                { "password", Password },
                { "skip-cert-verify", true } // Use "skip-cert-verify" as the key in the dictionary
            };

            return proxyConfig;
        }
    }
}