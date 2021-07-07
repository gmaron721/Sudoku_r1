using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Sudoku_r1
{
    enum ChromeBrowserResult
    {
        OK,
        BrowserPathError,
        ProxyError,
        UnknowError
    }
    class ChromeBrowser
    {
        private static string browserPath;
        private static string driverPath;
        private static ChromeBrowserResult GetChromeExecutableName()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    browserPath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\chrome-win\chrome.exe";
                    driverPath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\chrome-win";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    browserPath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\chrome-linux\chrome";
                    driverPath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\chrome-linux";
                }


                if (System.IO.File.Exists(browserPath) && System.IO.Directory.Exists(driverPath))
                {
                    return ChromeBrowserResult.OK;
                }
                else
                {
                    Console.WriteLine("Paths not found");
                    return ChromeBrowserResult.BrowserPathError;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ChromeBrowserResult.BrowserPathError;
            }
        }

        public IWebDriver driver { get; private set; }
        public ChromeBrowserResult CloseBrowser()
        {
            try
            {
                driver.Close();
                driver.Quit();
                return ChromeBrowserResult.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ChromeBrowserResult.UnknowError;
            }
        }
        public ChromeBrowserResult Init(string browserPath, string driverPath)
        {
            try
            {
                //Консоль
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);
                chromeDriverService.HideCommandPromptWindow = false;

                //Браузер
                ChromeOptions options = new ChromeOptions();
                options.BinaryLocation = browserPath;

                //options.AddArgument("headless");

                //----------------
                driver = new ChromeDriver(chromeDriverService, options);
                //----------------

                return ChromeBrowserResult.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ChromeBrowserResult.UnknowError;
            }
        }

        public ChromeBrowser()
        {
            if (GetChromeExecutableName().Equals(ChromeBrowserResult.OK))
            {
                Init(browserPath, driverPath);
            }
        }

        //The End
    }
}
