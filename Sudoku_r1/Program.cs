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

namespace Sudoku_r1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async static void R1(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string result = string.Empty;
            HtmlNodeCollection tables = doc.DocumentNode.SelectNodes("//table[@id='sudoku_mt']");

            for(int i=0; i<9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var el_i_j = doc.DocumentNode.SelectSingleNode("//td[@id='sc" +
                    "" +
                    i +
                    "-" +
                    j +
                    "']").InnerText;

                    Console.WriteLine(el_i_j);
                }
            }

            

            /*
            string result111 = "Start";
            foreach (HtmlNode t in tables)
            {
                result111 += t.InnerText + Environment.NewLine;
            }
            File.WriteAllText("ttt.html", result111);
            */

            return;

            //remove unnecessary-------------------
            tables.RemoveAt(0);
            tables.RemoveAt(0);
            for (int i = 0; i < 82; i++)
            {
                tables.RemoveAt(tables.Count - 1);
            }
            //------------------------------------

            foreach (HtmlNode table in tables)
            {
                //Console.WriteLine("Found: " + table.Id);
                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    result += "row" + Environment.NewLine;
                    foreach (HtmlNode cell in row.SelectNodes("th|td"))
                    {
                        result += "cell: " + cell.InnerText + Environment.NewLine;
                    }

                }
            }

            result = string.Join("", result.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            string regularExpressionPattern1 = @"1234(.*?)56789";
            Regex regex = new Regex(regularExpressionPattern1, RegexOptions.Singleline);
            MatchCollection collection = regex.Matches(result);

            List<int?> list = new List<int?>();//оставить список на случай если понадобится исходная задача
            foreach (Match c in collection)
            {
                if (string.IsNullOrEmpty(c.Groups[1].Value.ToString()))
                {
                    list.Add(null);
                }
                else
                {
                    list.Add(int.Parse(c.Groups[1].Value));
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

            Grid grid = new Grid(a);
            SudokoSolver ss = new SudokoSolver(grid);
            ss.SolvePuzzle();

            result = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result += grid.Data[j, i] + " ";//Console.Write(grid.Data[j,i]+" ");
                }
                result += Environment.NewLine;//Console.WriteLine(); Console.WriteLine();
            }

            string nameID = "0001";
            File.WriteAllText(nameID + ".txt", result);
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

        async static Task MainAsync(string[] args)
        {
            /*int readAccountResult = ReadAccount();
            if (readAccountResult == 0)
            {
                ChromeBrowser b1 = new ChromeBrowser();
                b1.driver.Navigate().GoToUrl("http://www.grandgames.net/");
                Thread.Sleep(1000);
                IWebElement query = b1.driver.FindElement(By.Id("lfshow"));
                query.Click();
                Thread.Sleep(2000);

                b1.driver.FindElement(By.Name("login")).SendKeys(account.login);
                b1.driver.FindElement(By.Name("pass")).SendKeys(account.pass);
                Thread.Sleep(500);
                b1.driver.FindElement(By.Name("pass")).SendKeys(Keys.Enter);
                Thread.Sleep(1000);
                b1.driver.Navigate().GoToUrl("http://www.grandgames.net/sudoku");

                //var tt = b1.driver.FindElement(By.ClassName("pages")).Text;
                b1.driver.Navigate().GoToUrl("https://grandgames.net/sudoku/id145221");
                //Thread.Sleep(7500);
                while (b1.driver.FindElements(By.ClassName("dsbut")).Count>0)
                {
                    Thread.Sleep(200);
                }
                b1.driver.FindElement(By.ClassName("prbut")).Click();
                Thread.Sleep(3000);

                R1(b1.driver.PageSource);//TUT
            }*/

            var tt=File.ReadAllText("tt1.html");
            R1(tt);

            Console.ReadKey();
        }





//----------THE END
    }
}
