
using System.Text;
using Befree;
class Program
{

    static void Main(string[] args)
    {
        string inputFile = "./input.txt";
        int listenPort = 1081;
        var parsedArgs = ArgsParser.ParseArgs(args);
        if (parsedArgs.Count == 0)
        {
            return;
        }
        if (parsedArgs.ContainsKey("-f"))
        {
            inputFile = parsedArgs["-f"];
        }

        if (parsedArgs.ContainsKey("-p"))
        {
            if (int.TryParse(parsedArgs["-p"], out int parsedPort))
            {
                listenPort = parsedPort;
            }
            else
            {
                Console.WriteLine("[-] 无效的端口号。使用默认端口 1081。");
            }
        }

        Console.WriteLine("我的天空！ Befree");
        
        RunMain(inputFile,listenPort);
        
    }

    static void RunMain(string inputFile,int listenPort)
    {
        string outputFile = "sectest.yaml";

        // 全局计数器,统计各类型节点总共获取数量
        int totalVmessCount = 0;
        int totalSsCount = 0;
        int totalSsrCount = 0;
        int totalTrojanCount = 0;

        //1.加载订阅
        var subscriptionUrls = LoadSubscriptionUrls(inputFile);
        Console.WriteLine($" [+] {inputFile}文件中，发现{subscriptionUrls.Count} 个订阅地址");

        //2.请求订阅并解析
        var allNodes = new List<Node>();
        foreach(var url in subscriptionUrls)
        {
            //Console.WriteLine($" [+] 正在处理订阅地址：{url}");
            var nodes = FetchAndParseSubscription(url, ref totalVmessCount, ref totalSsCount, ref totalSsrCount, ref totalTrojanCount);
            allNodes.AddRange(nodes);
        }
        Console.WriteLine($" [+] 总共解析到{allNodes.Count} 个节点");
        // 输出所有协议类型节点的总数
        Console.WriteLine($" [+] 其中包含vmess节点数量为: {totalVmessCount}");
        Console.WriteLine($" [+] 其中包含ss节点数量为: {totalSsCount}");
        Console.WriteLine($" [+] 其中包含ssr节点数量为: {totalSsrCount}**");
        Console.WriteLine($" [+] 其中包含trojan节点数量为: {totalTrojanCount}");

        //3.生成clash配置文件
        ClashConfigManager.GenerateConfig(allNodes,outputFile,listenPort);
        //Console.WriteLine($" [+] Clash配置已生成: {outputFile}");

        //4.运行clash
        ClashRunner.RunClash(outputFile);
    }


    //统计订阅文件中有多少订阅地址  
    static List<string> LoadSubscriptionUrls(string filePath)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"订阅文件未找到：{filePath}");
        }
        return new List<string>(File.ReadAllLines(filePath));
    }
    static List<Node> ParseNodes(string rawData, ref int totalVmessCount, ref int totalSsCount, ref int totalSsrCount, ref int totalTrojanCount)
    {
        var nodes = new List<Node>();
        foreach (var line in rawData.Split('\n'))
        {
            Node node = null;
            if (line.StartsWith("vmess://"))
            {
                //Console.WriteLine($"vmess_line: {line}");
                node = VmessNode.FromBase64(line.Substring(8));
                totalVmessCount++;
            }
            else if(line.StartsWith("ss://"))
            {
                //Console.WriteLine($"ss_line: {line}");
                node = ShadowsocksNode.Parse(line.Substring(5));
                totalSsCount++;
            }
            else if (line.StartsWith("ssr://"))
            {
                totalSsrCount++;
            }
            else if (line.StartsWith("trojan://"))
            {
                node = TrojanNode.Parse(line.Substring(9));
                totalTrojanCount++;
            }
            if (node != null)
            {
                nodes.Add(node);
            }
        }
        return nodes;
    }


    //请求解析订阅
    static List<Node> FetchAndParseSubscription(string url, ref int totalVmessCount, ref int totalSsCount, ref int totalSsrCount, ref int totalTrojanCount)
    {
        try
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            var response = client.GetStringAsync(url).Result;
            Console.WriteLine($" [+] 订阅获取成功： {url}");

            string decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(response));
            return ParseNodes(decodedData, ref totalVmessCount, ref totalSsCount, ref totalSsrCount, ref totalTrojanCount);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" [-] 处理订阅地址 {url} 时出错: {ex.Message}");
            return new List<Node>();
        }
    }
    static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        return new HttpClient(handler);
    }
}