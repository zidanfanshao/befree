using System;
using System.Collections.Generic;
using Figgle;

namespace Befree
{
    public class ArgsParser
    {
        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            string banner = FiggleFonts.Standard.Render("B e f r e e !!!");
            Console.WriteLine(banner);

            var parsedArgs = new Dictionary<string, string>();

            if (args.Length == 0)
            {
                ShowHelp();
                return parsedArgs;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                        ShowHelp();
                        Environment.Exit(0); // 退出程序，不再处理后续参数
                        break;
                    case "-f":
                        if (i + 1 < args.Length)
                        {
                            parsedArgs["-f"] = args[i + 1];
                            i++; // 跳过值
                        }
                        else
                        {
                            Console.WriteLine("Error: -f option requires a file path.");
                        }
                        break;
                    case "-p":
                        if (i + 1 < args.Length)
                        {
                            parsedArgs["-p"] = args[i + 1];
                            i++; // 跳过值
                        }
                        else
                        {
                            Console.WriteLine("Error: -p option requires a port number.");
                        }
                        break;
                    case "-t":
                        if (i + 1 < args.Length)
                        {
                            parsedArgs["-t"] = args[i + 1];
                            i++;
                        }else{Console.WriteLine("Error: -t option requires a speed url");};
                        break;
                    case "-y":
                        if (i + 1 < args.Length)
                        {
                            parsedArgs["-y"] = args[i + 1];
                            i++;
                        }else{Console.WriteLine("Error: -y option requires a yourself clash yaml file");};
                        break;
                    default:
                        Console.WriteLine($"Unrecognized argument: {args[i]}");
                        Environment.Exit(1);
                        break;
                }
            }

            return parsedArgs;
        }

        // 显示帮助信息
        private static void ShowHelp()
        {
            Console.WriteLine("我的天空！ Befree v0.3");
            Console.WriteLine("by: https://github.com/zidanfanshao/befree");
            Console.WriteLine("Usage:");
            Console.WriteLine("  -h      Show help information");
            Console.WriteLine("  -f      Specify a contain subscribe file path");
            Console.WriteLine("  -p      Specify a port number(http&socks5)");
            Console.WriteLine("  -t      Specify a link for speed testing(default:https://www.google.com)");
            Console.WriteLine("  -y      Specify a yourself clash yaml file");
        }
    }
}
