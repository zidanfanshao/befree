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
                var regex = new Regex(@"^([A-Za-z0-9]+)@([A-Za-z0-9\.\-]+):(\d+)\?(.+?)#(.+)$");
                var match = regex.Match(url);
                var password = match.Groups[1].Value;
                var host = match.Groups[2].Value;
                var port = int.Parse(match.Groups[3].Value);
                var query = match.Groups[4].Value;
                var remark = HttpUtility.UrlDecode(match.Groups[5].Value);
                var parameters = HttpUtility.ParseQueryString(query);
                var peer = parameters["peer"];
                return new TrojanNode
                {
                    Name = remark,
                    Password = password,
                    Server = host,
                    Port = port,
                    Sni = peer
                    
                };

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error parsing ShadowsocksR node: {ex.Message}");
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