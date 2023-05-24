// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

using System;
using System.IO;
using System.Json;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp;
using SharpedUtilsCollection.PuppeteerUtils.Models;

namespace SharpedUtilsCollection.PuppeteerUtils;

public static class Wrapper
{
    private static IBrowser? _browser;
    private static IPage? _page;

    public static string? ProfilePath = null;

    public static readonly WaitForSelectorOptions ShortSelectorAwaiter = new() { Timeout = 5310 };
    public static readonly WaitForSelectorOptions LongSelectorAwaiter = new() { Timeout = 55555 };
    public static readonly WaitForSelectorOptions SuperLongSelectorAwaiter = new() { Timeout = 999999 };

    public static WaitForSelectorOptions GetSelectorAwaiter(int timeout)
    {
        return new WaitForSelectorOptions()
        {
            Timeout = timeout
        };
    }

    public static string[] Args = new[]
    {
        "--start-maximized",
        "--disable-web-security",
        "--disable-features=IsolateOrigins,site-per-process"
    };

    public static Browser Browser
    {
        get
        {
            _browser ??= LaunchBrowser().Result;
            return (_browser as Browser)!;
        }
    }

    public static Page Page
    {
        get
        {
            _page ??= Browser.NewPageAsync().Result;
            return (_page as Page)!;
        }
    }

    private static async Task<IBrowser> LaunchBrowser()
    {
        var launchOptions = new LaunchOptions
        {
            DefaultViewport = null,
            Headless = true,
            //Headless = false,
            IgnoreHTTPSErrors = true,

            Args = Args
        };

        #if (DEBUG)
            // Environment.SetEnvironmentVariable("DISPLAY", "192.168.0.139:0.0");
            launchOptions.Headless = false;
        #endif

        #if (!DEBUG)
            // Environment.SetEnvironmentVariable("DISPLAY", ":0.0");
        #endif

        if (ProfilePath != null)
        {
            launchOptions.UserDataDir = ProfilePath;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            File.Exists("/usr/bin/google-chrome"))
        {
            launchOptions.ExecutablePath = "/usr/bin/google-chrome";

            var args = launchOptions.Args.ToList();
            args.Add("--canvas-msaa-sample-count=0");
            args.Add("--disable-gpu-driver-bug-workarounds");
            args.Add("--disable-gpu-vsync");
            args.Add("--disable-features=InfiniteSessionRestore");
            args.Add("--disable-smooth-scrolling");
            args.Add("--enable-accelerated-2d-canvas");
            args.Add("--enable-direct-composition-layers");
            args.Add("--enable-gpu-rasterization");
            args.Add("--enable-hardware-overlays");
            args.Add("--enable-low-res-tiling");
            args.Add("--enable-native-gpu-memory-buffers");
            args.Add("--enable-tile-compression");
            args.Add("--enable-zero-copy");
            args.Add("--force-gpu-rasterization");
            args.Add("--gpu-rasterization-msaa-sample-count=0");
            args.Add("--ignore-gpu-blocklist");
            launchOptions.Args = args.ToArray();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (File.Exists("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"))
            {
                launchOptions.ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
            }

            if (File.Exists("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"))
            {
                launchOptions.ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            }
        }
        else
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        }

        return await Puppeteer.LaunchAsync(launchOptions);
    }

    public static async Task CloseAsync()
    {
        await Browser.CloseAsync();
        _page = null;
        _browser = null;
    }

    public static void LoadExtension(string extensionId)
    {
        try
        {
            var extensionFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                ".config", 
                "google-chrome", 
                "Default", 
                "Extensions", 
                extensionId
            );
                
            if (!Directory.Exists(extensionFolder)) return;

            var versionFolder = Directory.GetDirectories(extensionFolder).First();

            Args = Args.Append($"--disable-extensions-except={versionFolder}").ToArray();
            Args = Args.Append($"--load-extension={versionFolder}").ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static async Task ClickIfExists(string selector, int? timeout = null)
    {
        try
        {
            var awaiter = timeout is null ? ShortSelectorAwaiter : GetSelectorAwaiter((int)timeout);
            await Page.WaitForSelectorAsync(selector, awaiter);
            await Page.ClickAsync(selector);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Click on html-element via JS
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static async Task ClickViaJs(string selector)
    {
        try
        {
            await Page.WaitForSelectorAsync(selector, ShortSelectorAwaiter);
            IElementHandle elementHandle = await Page.QuerySelectorAsync(selector);
            await Page.EvaluateFunctionAsync<bool>("element => element.click()", elementHandle);
        }
        catch (Exception)
        {
            Console.WriteLine($"Selector '{selector}' not found");
        }
    }

    /// <summary>
    /// Click on html-element via JS using getBoundingClientRect
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static async Task ClickByCalculatedCoordinates(string selector)
    {
        string jsCmd = $"JSON.stringify(document.querySelector(\"{selector}\").getBoundingClientRect())";
        var boundingClientRectStr = await Page.EvaluateExpressionAsync<string>(jsCmd);
            
        DomRect? boundingClientRect = JsonConvert.DeserializeObject<DomRect>(boundingClientRectStr);
        System.Diagnostics.Debug.Assert(boundingClientRect != null, nameof(boundingClientRect) + " != null");
            
        var xCoordinate = boundingClientRect.X+boundingClientRect.Width/2;
        var yCoordinate = boundingClientRect.Y+boundingClientRect.Height/2;
        await Page.Mouse.ClickAsync((int)xCoordinate, (int)yCoordinate);
    }

    /// <summary>
    /// Click on html-element via JS using getBoundingClientRect
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static async Task ClickByCalculatedCoordinates(IElementHandle element)
    {
        string script = await File.ReadAllTextAsync("PuppeteerUtils/js/getBoundingClientRect.js");
        var boundingBox = await Page.EvaluateFunctionAsync<BoundingBox>(script, element);
        
        var xCoordinate = boundingBox.X+boundingBox.Width/2;
        var yCoordinate = boundingBox.Y+boundingBox.Height/2;
        var scrollTo =    boundingBox.Y-boundingBox.Height*2;

        await Page.EvaluateExpressionAsync<bool>($"window.scrollTo(0,{scrollTo})");
        await Page.Mouse.ClickAsync((int)xCoordinate, (int)yCoordinate);
    }

    public static async Task<JsonValue> GetJson(string url)
    {
        System.Diagnostics.Debug.Assert(Page != null, nameof(Page) + " != null");
        var response = await Page.GoToAsync(url, WaitUntilNavigation.Load);
        string pageContent = await response.TextAsync();
        return JsonValue.Parse(pageContent);
    }

    public static async Task WaitNetworkIdle2(Page? page = null)
    {
        page ??= Page;
        System.Diagnostics.Debug.Assert(page != null, nameof(page) + " != null");
            
        await page.WaitForNavigationAsync(new NavigationOptions
        {
            WaitUntil = new [] { WaitUntilNavigation.Networkidle2 }
        });
    }
}
