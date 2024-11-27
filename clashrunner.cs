using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Befree
{
    public class ClashRunner
    {
        // 提取并启动 Clash
        public static void RunClash(string configFilePath)
        {
            try
            {
                // 获取当前程序的执行路径
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;

                // 指定临时文件路径
                string clashExecutablePath = Path.Combine(currentDir, "clash-windows-amd64.exe");

                // 检查该文件是否已经存在
                if (!File.Exists(clashExecutablePath))
                {
                    // 如果文件不存在，从嵌入资源中提取
                    ExtractClashExecutable(clashExecutablePath);
                }

                // 创建一个新的进程来运行 Clash
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = clashExecutablePath,      // 指定 Clash 可执行文件路径
                    Arguments = $"-f {configFilePath}",  // 指定配置文件
                    UseShellExecute = false,             // 不使用 shell 执行（直接启动进程）
                    CreateNoWindow = true               // 不显示命令行窗口
                };

                // 启动进程
                using (Process process = Process.Start(startInfo))
                {
                    Console.WriteLine(" [+] running...");
                    process.WaitForExit(); // 等待 Clash 进程结束
                    Console.WriteLine(" [-] stop...");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"An error occurred while starting Clash: {ex.Message}");
            }
        }

        // 提取嵌入的 Clash 可执行文件到磁盘
        private static void ExtractClashExecutable(string outputPath)
        {
            try
            {
                // 获取当前程序集
                var assembly = Assembly.GetExecutingAssembly();

                // 获取嵌入资源的名称
                string resourceName = "Befree.Resources.clash-windows-amd64.exe"; // 这里的名称要与资源名称一致

                // 打开嵌入资源流
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new InvalidOperationException("Resource not found.");
                    }

                    // 创建文件并将资源内容写入到文件中
                    using (var fileStream = new FileStream(outputPath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    Console.WriteLine(" [+] start !!!");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($" [-] {ex.Message}");
            }
        }
    }
}
