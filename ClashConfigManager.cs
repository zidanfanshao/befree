using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Befree
{
    public static class ClashConfigManager
    {
        public static void GenerateConfig(List<Node> nodes, string outputFileName, int listenPort)
        {
            try
            {
                // 去重节点名称
                ResolveDuplicateNames(nodes);

                var proxies = new List<string>();
                var proxyNames = new List<string>();
                var nameCount = new Dictionary<string, int>();

                // 遍历节点并生成代理配置
                foreach (var xxx in nodes)
                {
                    var proxyConfig = xxx.ToClashProxy();
                    if (proxyConfig is Dictionary<string, object> dict)
                    {
                        if (dict.ContainsKey("name"))
                        {
                            string name = dict["name"].ToString();
                            if (nameCount.ContainsKey(name))
                            {
                                nameCount[name]++;
                                name = $"{name}_{nameCount[name]}";
                            }
                            else
                            {
                                nameCount[name] = 1;
                            }
                            proxyNames.Add(name);
                            proxies.Add(FormatProxyConfig(dict));
                        }
                    }
                }

                // 创建 Clash 配置的 YAML 字符串
                var yaml = new StringBuilder();
                yaml.AppendLine("allowLan: true");
                yaml.AppendLine($"mixed-port: {listenPort}");
                yaml.AppendLine("rules:");
                yaml.AppendLine("  - MATCH, proxy_pool");
                yaml.AppendLine("proxy-groups:");
                yaml.AppendLine("  - name: proxy_pool");
                yaml.AppendLine("    type: load-balance");
                yaml.AppendLine("    proxies:");
                foreach (var name in proxyNames)
                {
                    yaml.AppendLine($"      - {name}");
                }
                yaml.AppendLine("    url: http://www.google.com");
                yaml.AppendLine("    interval: 300");
                yaml.AppendLine("    strategy: round-robin");
                yaml.AppendLine("proxies:");
                foreach (var proxy in proxies)
                {
                    yaml.AppendLine(proxy);
                }

                // 获取当前程序运行的目录，并构建完整的输出路径
                string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFileName);

                // 确保目录存在
                var directory = Path.GetDirectoryName(outputPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 将 YAML 写入文件，使用 UTF-8 编码
                File.WriteAllText(outputPath, yaml.ToString(), Encoding.UTF8);
                Console.WriteLine($" [+] http & socks 监听端口：{listenPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [-] 生成配置时发生错误：{ex.Message}");
            }
        }

        private static void ResolveDuplicateNames(List<Node> nodes)
        {
            var nameCount = new Dictionary<string, int>();

            foreach (var node in nodes)
            {
                dynamic dynamicNode = node;
                if (dynamicNode.Name != null)
                {
                    var originalName = dynamicNode.Name;
                    if (nameCount.ContainsKey(originalName))
                    {
                        nameCount[originalName]++;
                        dynamicNode.Name = $"{originalName}__{nameCount[originalName]}";
                    }
                    else
                    {
                        nameCount[originalName] = 1;
                    }
                }
            }
        }

        private static string FormatProxyConfig(Dictionary<string, object> proxyConfig)
        {
            var yaml = new StringBuilder();
            yaml.AppendLine("  - name: " + proxyConfig["name"]);
            yaml.AppendLine("    type: " + proxyConfig["type"]);
            yaml.AppendLine("    server: " + proxyConfig["server"]);
            yaml.AppendLine("    port: " + proxyConfig["port"]);
            if (proxyConfig.ContainsKey("password"))
            {
                yaml.AppendLine("    password: " + proxyConfig["password"]);
            }
            if (proxyConfig.ContainsKey("cipher"))
            {
                yaml.AppendLine("    cipher: " + proxyConfig["cipher"]);
            }
            if (proxyConfig.ContainsKey("uuid"))
            {
                yaml.AppendLine("    uuid: " + proxyConfig["uuid"]);
            }
            if (proxyConfig.ContainsKey("alterId"))
            {
                yaml.AppendLine("    alterId: " + proxyConfig["alterId"]);
            }
            if (proxyConfig.ContainsKey("skip-cert-verify"))
            {
                yaml.AppendLine("    skip-cert-verify: " + proxyConfig["skip-cert-verify"]);
            }
            return yaml.ToString();
        }
    }
}
