using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Sudoku_r1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static int GetLastPageFromBase()
        {
            using (var db = new ApplicationContext())
            {
                if (db.Sudoku_Pages.Count() == 0)
                {
                    return 1;//numbering on the site from one
                }
                else
                {
                    return int.Parse(db.Sudoku_Pages.OrderBy(p => p.Page).LastOrDefault().Page);
                }
            }
        }
        public static bool IsLastPage(IWebDriver driver, int page)
        {
            driver.Navigate().GoToUrl("https://grandgames.net/sudoku/"+page);
            //Overcome AntiDDos
            if (driver.FindElements(By.XPath("//div[@class='pages']")).Count==0)
            {
                Thread.Sleep(60000);
                driver.Navigate().GoToUrl("https://grandgames.net/sudoku/" + page);
            }
            IWebElement pages_div = driver.FindElement(By.XPath("//div[@class='pages']"));
            var pages = pages_div.FindElements(By.XPath(".//*"));
            if (pages[pages.Count - 2].Text == "►")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static int GetLastPage(IWebDriver driver)
        {
            int last_page = GetLastPageFromBase();
            
            //No records in the database
            if (last_page == 0)
            {
                int i = 1;
                while (!IsLastPage(driver, i))
                {
                    i++;
                }

                driver.Navigate().GoToUrl("https://grandgames.net/sudoku/" + i);
                IWebElement pages_div = driver.FindElement(By.XPath("//div[@class='pages']"));
                var pages = pages_div.FindElements(By.XPath(".//*"));
                string lastNum = pages[pages.Count - 2].Text;

                return int.Parse(lastNum);
            }
            //Records in the database
            else
            {
                //covers cases if records in the database, but the page is not the last
                int i = last_page;
                while (!IsLastPage(driver, i))
                {
                    i++;
                }

                driver.Navigate().GoToUrl("https://grandgames.net/sudoku/" + i);
                IWebElement pages_div = driver.FindElement(By.XPath("//div[@class='pages']"));
                var pages = pages_div.FindElements(By.XPath(".//*"));
                string lastNum = pages[pages.Count - 2].Text;

                return int.Parse(lastNum);
            }


        }
        public static void AddLastPage(string last_page)
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    db.Add(new Sudoku_Page { Page = last_page });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    //Skip duplicate entries
                }
            }
        }
        public static Account account = new Account();
        public class Account
        {
            public string login;
            public string pass;
        }
        public static int ReadAccount()
        {
            try
            {
                using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Account.json"))
                {
                    string json = r.ReadToEnd();
                    account = JsonConvert.DeserializeObject<Account>(json);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        public static void Login(IWebDriver driver)
        {
            //Login
            driver.Navigate().GoToUrl("http://www.grandgames.net/");
            Thread.Sleep(1000);
            IWebElement query = driver.FindElement(By.Id("lfshow"));
            query.Click();
            Thread.Sleep(2000);

            driver.FindElement(By.Name("login")).SendKeys(account.login);
            driver.FindElement(By.Name("pass")).SendKeys(account.pass);
            Thread.Sleep(500);
            driver.FindElement(By.Name("pass")).SendKeys(Keys.Enter);
            Thread.Sleep(1000);
            driver.Navigate().GoToUrl("http://www.grandgames.net/sudoku");
            //---------------------------------------------------------------
        }

        public static void R1(IWebDriver driver, string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string result = string.Empty;

            List<int?> list = new List<int?>();//оставить список на случай если понадобится исходная задача
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string el_i_j = doc.DocumentNode.SelectSingleNode("//td[@id='sc" +
                    "" +
                    i +
                    "-" +
                    j +
                    "']").InnerText;

                    //Console.WriteLine(el_i_j);
                    if (string.IsNullOrEmpty(el_i_j))
                    {
                        list.Add(null);
                    }
                    else
                    {
                        list.Add(Int32.Parse(el_i_j));
                    }
                }
            }

            int?[,] a = new int?[9, 9];//int? Типы, допускающие значение NULL
            //List-to-array[,]----------------------------
            int iCount = 0;
            foreach (var number in list)
            {
                a[iCount % 9, (int)(iCount / 9)] = number;
                iCount++;
            }
            //--------------------------------------------

            Grid source_grid = new Grid(a);
            Grid grid = new Grid(a);
            SudokoSolver ss = new SudokoSolver(grid);
            ss.SolvePuzzle();

            result = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result += grid.Data[j, i] + " ";
                    HtmlNode element = doc.DocumentNode.SelectSingleNode("//td[@id='sc" + i + "-" + j + "']");
                    if (string.IsNullOrEmpty(element.InnerText))
                    {
                        //element.InnerHtml = grid.Data[j, i].ToString();
                        ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('sc" + i + "-" + j + "').innerHTML = '" + grid.Data[j, i].ToString() + "';");
                    }
                }
                result += Environment.NewLine;
            }

            //string nameID = "0001";
            //File.WriteAllText(nameID + ".txt", result);
        }

        public static void Solve(IWebDriver driver, string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                //Overcome AntiDDos
                if (driver.FindElements(By.ClassName("prbut")).Count == 0)
                {
                    Thread.Sleep(60000);
                    driver.Navigate().GoToUrl(url);
                }
                while (driver.FindElements(By.ClassName("dsbut")).Count > 0)
                {
                    Thread.Sleep(200);
                }
                driver.FindElement(By.ClassName("prbut")).Click();
                Thread.Sleep(2000);

                R1(driver, driver.PageSource);
                Thread.Sleep(200);
                driver.FindElement(By.XPath("//button[@onclick='Check();']")).Click();

                UpdateSudokuUrl(url);
            }
            catch { }
        }

        //Add to database
        public static void AddSudokuUrl(string url)
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    db.Add(new Sudoku_URL { Url = url });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    //Skip duplicate entries
                }
            }
        }
        
        //Update SudokuUrl_time()
        public static void UpdateSudokuUrl(string url)
        {
            try
            {
                //convert string to relative form
                if (url.Split('/').Length > 3)
                {
                    var items = url.Split('/');
                    url = @"/" + items[3] + @"/" + items[4];
                }
                //------------------------------------------
                using (var db = new ApplicationContext())
                {
                    var my_url = db.Sudoku_Urls.Single(u => u.Url == url);
                    my_url.Time = DateTime.Now.ToString();
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("UpdateSudokuUrlException: "+ex.Message);
            }
        }

        public static void AddNewSudokuUrls(IWebDriver driver, int page_i)
        {
            driver.Navigate().GoToUrl("https://grandgames.net/sudoku/" + page_i);
            //Overcome AntiDDos
            if (driver.FindElements(By.XPath("//td[@class='l_cell']")).Count == 0)
            {
                Thread.Sleep(60000);
                driver.Navigate().GoToUrl("https://grandgames.net/sudoku/" + page_i);
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.PageSource);
            var urls = doc.DocumentNode.SelectNodes("//td[@class='l_cell']//a[@href]");
            foreach(var url in urls)
            {
                string hrefValue = url.GetAttributeValue("href", string.Empty);
                AddSudokuUrl(hrefValue);
            }
        }

        public static List<Sudoku_URL> GetUrlsFromBase()
        {
            using (var db = new ApplicationContext())
            {
                var urls = db.Sudoku_Urls.ToList();
                return urls;
            }
        }

        async static Task MainAsync(string[] args)
        {
            int readAccountResult = ReadAccount();
            if (readAccountResult == 0)
            {
                ChromeBrowser b1 = new ChromeBrowser();

                Login(b1.driver);

                while(true)
                {
                    //Pages
                    int last_page0 = GetLastPageFromBase();
                    int last_page1 = GetLastPage(b1.driver);
                    //-------------------------------------
                    for (int i = last_page0; i < last_page1; i++)
                    {
                        AddNewSudokuUrls(b1.driver, i);
                    }

                    //--check last page
                    if (last_page0 == last_page1)
                    {
                        AddNewSudokuUrls(b1.driver, last_page1);
                    }
                    //----

                    AddLastPage(last_page1.ToString());

                    List<Sudoku_URL> urls = GetUrlsFromBase();
                    for (int i = 0; i < urls.Count; i++)
                    {
                        if (urls[i].Time == null)
                        {
                            Solve(b1.driver, @"https://grandgames.net" + urls[i].Url);
                        }
                    }

                    Thread.Sleep(24*60*60*1000);
                }
            }



            Console.ReadKey();
        }


//----------THE END
    }
}
