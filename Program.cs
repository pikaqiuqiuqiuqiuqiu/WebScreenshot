using CefSharp;
using CefSharp.WinForms;
using System.Runtime.InteropServices;

namespace WebScreenshot
{
    internal static class Program
    {
        
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用DPI感知
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            string appBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty;
            string runtimeArch = GetRuntimeArchitecture();
            
            // 输出基本信息
            Console.WriteLine($"检测到运行时架构: {runtimeArch}");
            Console.WriteLine($"应用程序基础路径: {appBasePath}");
            
            // 优先检查是否存在runtimes目录
            string runtimesPath = Path.Combine(appBasePath, "runtimes");
            string browserSubProcessPath = null;

            if (Directory.Exists(runtimesPath))
            {
                // 检查对应架构的runtime目录
                string runtimePath = Path.Combine(runtimesPath, runtimeArch);
                string nativeSubProcessPath = Path.Combine(runtimePath, "native", "CefSharp.BrowserSubprocess.exe");
                
                if (File.Exists(nativeSubProcessPath))
                {
                    browserSubProcessPath = nativeSubProcessPath;
                    Console.WriteLine($"找到runtimes目录CEF浏览器子进程: {browserSubProcessPath}");
                }
            }
            
            // 如果在runtimes目录中没有找到，检查架构特定目录
            if (browserSubProcessPath == null)
            {
                string archPath = Path.Combine(appBasePath, runtimeArch.Split('-')[1], "CefSharp.BrowserSubprocess.exe");
                if (File.Exists(archPath))
                {
                    browserSubProcessPath = archPath;
                    Console.WriteLine($"找到架构目录CEF浏览器子进程: {browserSubProcessPath}");
                }
            }
            
            // 如果都没有找到，直接使用当前目录
            if (browserSubProcessPath == null)
            {
                browserSubProcessPath = Path.Combine(appBasePath, "CefSharp.BrowserSubprocess.exe");
                Console.WriteLine($"未找到特定路径CEF组件，使用当前目录: {browserSubProcessPath}");
            }

            // 初始化CefSharp
            CefSettings settings = new CefSettings();
            settings.BrowserSubprocessPath = browserSubProcessPath;
            settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebScreenshot", "Cache");
            settings.LogSeverity = LogSeverity.Disable;
            
            // 设置资源和本地化文件目录为当前应用程序目录
            settings.ResourcesDirPath = appBasePath;
            settings.LocalesDirPath = Path.Combine(appBasePath, "locales");
            
            // 添加命令行参数以启用高DPI支持
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            // 添加高DPI相关命令行参数
            settings.CefCommandLineArgs.Add("high-dpi-support", "1");
            settings.CefCommandLineArgs.Add("force-device-scale-factor", "1");
            
            // 添加弹出窗口行为控制参数
            settings.CefCommandLineArgs.Add("disable-popup-blocking", "1");  // 禁用弹出窗口阻止
            settings.CefCommandLineArgs.Add("disable-new-tab-first-run", "1"); // 禁用新标签页首次运行体验
            
            // 设置窗口渲染选项
            settings.WindowlessRenderingEnabled = false;
            
            // 初始化Cef
            Console.WriteLine("正在初始化CEF...");
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            Console.WriteLine("CEF初始化完成");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // 启用高DPI支持
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            
            Application.Run(new MainForm());
            
            // 关闭Cef
            Cef.Shutdown();
        }

        /// <summary>
        /// 获取当前运行时架构标识符
        /// </summary>
        /// <returns>运行时架构标识符，例如"win-x64"</returns>
        private static string GetRuntimeArchitecture()
        {
            string platform;
            string architecture;

            // 检测当前操作系统
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = "osx";
            }
            else
            {
                platform = "win";
            }

            // 检测当前系统架构
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                architecture = "x64";
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                architecture = "x86";
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                architecture = "arm64";
            }
            else
            {
                architecture = "x86";
            }

            return $"{platform}-{architecture}";
        }
    }
}