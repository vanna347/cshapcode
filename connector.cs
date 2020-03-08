using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Selenium.Helper
{
    public enum Browser
    {
        Chrome,
        Firefox,
        PhantomJS
    }

    public class Connector
    {
        public static IWebDriver WebDriver { get; set; }

        private const Uri EmptyUri = null;

        public static IWebDriver Initialize(Browser browser)
        {
            return Initialize(browser, false, "", EmptyUri);
        }

        public static IWebDriver Initialize(Browser browser, string driverPath)
        {
            return Initialize(browser, false, driverPath, EmptyUri);
        }

        public static IWebDriver Initialize(Browser browser, Uri seleniumHubURL, string operatingSystem = "", bool maximize = false)
        {
            return Initialize(browser, true, "", seleniumHubURL, operatingSystem, maximize);
        }

        public static IWebDriver Initialize(Browser browser, bool remote, string driverPath, Uri seleniumHubURL, string operatingSystem = "", bool maximize = true)
        {
            if (remote)
            {
                WebDriver = InitializeRemote(browser, seleniumHubURL);
            }
            else
            {
                switch (browser)
                {
                    case Browser.Firefox:
                        var firefoxOptions = new FirefoxOptions();
                        WebDriver = new FirefoxDriver(firefoxOptions);
                        break;
                    default:  // default is Chrome
                        // prevent annoying popup from Chrome regarding Developer Extensions
                        var chromeOptions = new ChromeOptions();
                        //chromeOptions.BinaryLocation =Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        chromeOptions.EnableMobileEmulation("iPhone X");
                        chromeOptions.AddArgument("--disable-extensions");
                        chromeOptions.AddArguments("--window-size=800,600");
                        chromeOptions.AddArgument("--test-type");
                        //chromeOptions.AddArguments("--user-agent='Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1'");
                        if (maximize)
                        {
                            chromeOptions.AddArgument("--start-maximized");
                        }
                        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        WebDriver = new ChromeDriver(@"C:\Users\vanna\source\repos\AutoTest\SeleniumHelper\bin\Debug\", chromeOptions);
                        break;
                }
            }

            if (maximize)
            {
                WebDriver.Manage().Window.Maximize();
            }
            return WebDriver;
        }

        [Obsolete]
        public static IWebDriver InitializeRemote(Browser browser, Uri seleniumHubURL, string operatingSystem = "")
        {
            var capabilities = new DesiredCapabilities();
            var url = seleniumHubURL;
            if (url == null || url.ToString() == "")
            {
                try
                {
                    url = new Uri(ConfigurationManager.AppSettings["SeleniumHubURL"].ToLower(System.Globalization.CultureInfo.InvariantCulture));
                }
                catch
                {
                    url = null;
                }
            }
            else
            {
                throw new Exception("URL for SeleniumHub must be provided");
            }
            return WebDriver;

        }

        [Obsolete]
        public static IWebDriver GetRemoteChromeDriver()
        {
            var options = new ChromeOptions
            {
                BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
            };
            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability(ChromeOptions.Capability, options);
            WebDriver = new RemoteWebDriver(new Uri("http://192.168.2.4:4444/wd/hub"), capabilities);
            return WebDriver;
        }
    }
}
