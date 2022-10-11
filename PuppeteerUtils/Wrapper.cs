// ReSharper disable MemberCanBePrivate.Global

using System;
using System.IO;
using System.Json;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace SharpedUtilsCollection.PuppeteerUtils
{
    public static class Utils
    {
        private static Browser? _browser;
        private static Page? _page;

        public static string? ProfilePath = null;

        public static readonly WaitForSelectorOptions ShortSelectorAwaiter = new() { Timeout = 5310 };
        public static readonly WaitForSelectorOptions LongSelectorAwaiter = new() { Timeout = 55555 };
        public static readonly WaitForSelectorOptions SuperLongSelectorAwaiter = new() { Timeout = 999999 };

        public static string[] Args = new[]
        {
            "--start-maximized",
            "--disable-web-security",
            "--disable-features=IsolateOrigins,site-per-process"
        };

        public static Browser? Browser
        {
            get
            {
                if (_browser == null)
                {
                    LaunchBrowser().GetAwaiter().GetResult();
                }

                return _browser;
            }
        }

        public static Page? Page
        {
            get
            {
                if (_page == null)
                {
                    _page = Browser.NewPageAsync().Result;

                    /*await _page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = 1600,
                        Height = 900,
                        DeviceScaleFactor = 2
                    });*/
                }

                return _page;
            }
        }

        private static async Task LaunchBrowser()
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

            _browser = await Puppeteer.LaunchAsync(launchOptions);
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
                    Environment.GetEnvironmentVariable("HOME"),
                    ".config/google-chrome/Default/Extensions",
                    extensionId
                );

                var versionFolder = Directory.GetDirectories(extensionFolder).First();

                Args = Args.Append($"--disable-extensions-except={versionFolder}").ToArray();
                Args = Args.Append($"--load-extension={versionFolder}").ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Click on html-element
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task WaitAndClick(string selector, WaitForSelectorOptions? options = null)
        {
            await _page.WaitForSelectorAsync(selector, options ?? ShortSelectorAwaiter);
            await _page.ClickAsync(selector);
        }

        public static async Task ClickIfExists(string selector)
        {
            /*
            try
            {
                await _page.WaitForSelectorAsync(selector, ShortSelectorAwaiter);
                await _page.ClickAsync(selector);
            }
            catch (Exception)
            {
                // ignored
            }
            */
            var tag = await _page.QuerySelectorAsync(selector);
            if (tag != null) await _page.ClickAsync(selector);
        }

        /// <summary>
        /// Click on html-element via JS
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static async Task JsClickAsync(string selector)
        {
            await _page.WaitForSelectorAsync(selector, ShortSelectorAwaiter);
            var elementHandle = await _page.QuerySelectorAsync(selector);
            await _page.EvaluateFunctionAsync<bool>("element => element.click()", elementHandle);
        }

        /// <summary>
        /// Click on html-element via JS using getBoundingClientRect
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static async Task JsCoordsClickAsync(string selector)
        {
            string jsCmd = $"JSON.stringify(document.querySelector(\"{selector}\").getBoundingClientRect())";
            var boundingClientRectStr = await _page.EvaluateExpressionAsync<string>(jsCmd);
            DomRect boundingClientRect = JsonConvert.DeserializeObject<DomRect>(boundingClientRectStr);
            var xCoord = boundingClientRect.X+boundingClientRect.Width/2;
            var yCoord = boundingClientRect.Y+boundingClientRect.Height/2;
            await _page.Mouse.ClickAsync((int)xCoord, (int)yCoord);
        }

        /// <summary>
        /// Click on html-element via JS using getBoundingClientRect
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static async Task JsCoordsClickAsync(ElementHandle element)
        {
            var boundingClientRectStr = await element.EvaluateFunctionAsync<string>("element => element.getBoundingClientRect()");
            DomRect boundingClientRect = JsonConvert.DeserializeObject<DomRect>(boundingClientRectStr);
            var xCoord = boundingClientRect.X+boundingClientRect.Width/2;
            var yCoord = boundingClientRect.Y+boundingClientRect.Height/2;
            await _page.Mouse.ClickAsync((int)xCoord, (int)yCoord);
        }

        public static async Task<JsonValue> GetJson(string url)
        {
            var response = await Page.GoToAsync(url, WaitUntilNavigation.Load);
            string pageContent = await response.TextAsync();
            return JsonValue.Parse(pageContent);
        }

        public static async Task WaitNetworkIdle2(Page? page = null)
        {
            page ??= _page;

            await page.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new [] { WaitUntilNavigation.Networkidle2 }
            });
        }
    }
}
