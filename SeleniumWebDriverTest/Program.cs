using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using Colored;

namespace SeleniumWebDriverTest
{
    /// <summary>
    /// SeleniumWebDriverTest main class
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Chrome driver
        /// </summary>
        static IWebDriver driver;

        /// <summary>
        /// Link to default site
        /// </summary>
        static Uri uri = new Uri("https://cz.careers.veeam.com/vacancies");
        /// <summary>
        /// Dropdown department
        /// </summary>
        static string department = "Research & Development";
        /// <summary>
        /// Expected number of vacancies
        /// </summary>
        static uint expectedResult = 15;
        /// <summary>
        /// Timeout for the page to load
        /// </summary>
        static uint timeout = 10;

        /// <summary>
        /// Main function
        /// </summary>
        static void Main()
        {
            InitVariables();
            InitDriver();

            ChooseLanguage("Global");

            GoToJobs();

            ChooseDepartment();
            FindVacancies();
        }

        #region Init

        /// <summary>
        /// Initialization of all necessary variables entered by the user
        /// </summary>
        static void InitVariables()
        {
            if (!ColoredLine.YesOrNo("Change input parameters?", ConsoleColor.DarkMagenta))
                return;

            ColoredLine.Write("\nIf the field is left blank, the values will not change.\n", ConsoleColor.Red);

            department = ColoredLine.WriteAndRead(department, $"Enter {nameof(department)} in {department.GetType()} type", ConsoleColor.DarkMagenta);
            expectedResult = ColoredLine.WriteAndRead(expectedResult, $"\nEnter {nameof(expectedResult)} in {expectedResult.GetType()} type", ConsoleColor.DarkMagenta);
            timeout = ColoredLine.WriteAndRead(timeout, $"\nEnter {nameof(timeout)} in {timeout.GetType()} type", ConsoleColor.DarkMagenta);
            Console.WriteLine();
        }

        /// <summary>
        /// Driver initialization and configuration
        /// </summary>
        static void InitDriver()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(uri);
        }

        #endregion

        /// <summary>
        /// Page language selection
        /// </summary>
        static void ChooseLanguage(string language)
        {
            IWebElement dropBox = driver.FindElement(By.XPath("//*[@id='root']/div/header/div[1]/div/div/div/nav/nav/div/div"), timeout);
            dropBox.Click();

            IWebElement dropItem = dropBox.FindChildElementByName(By.ClassName("dropdown-item"), language, timeout);
            dropItem.Click();
        }

        /// <summary>
        /// Transition to the department of vacancies
        /// </summary>
        static void GoToJobs()
        {
            // It's is used to set time to wait for loading of the page.
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            driver.FindElement(By.CssSelector("button[class='btn btn-apply btn-success']"), timeout).Click();
        }

        /// <summary>
        /// Selecting a job filter
        /// </summary>
        static void ChooseDepartment()
        {
            IWebElement dropdownBox = driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/header/div/div/div[2]/div"), timeout);
            dropdownBox.Click();

            IWebElement dropdownItem = dropdownBox.FindChildElementByName(By.ClassName("dropdown-item"), department, timeout);
            dropdownItem.Click();

            driver.FindElement(By.CssSelector("button[class='btn btn-outline-success']"), timeout).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
        }

        /// <summary>
        /// Finds the number of vacancies and compares them with the expected result
        /// </summary>
        static void FindVacancies()
        {
            ReadOnlyCollection<IWebElement> vacancies = driver.FindElement(By.CssSelector("div[class='d-none d-lg-block']"), timeout).FindElements(By.XPath("*"), timeout);

            driver.FindElement(By.CssSelector("input[class='dropdown-toggle form-control']"), timeout).SendKeys("Look in the log");
            ColoredLine.Write($"\nFound {vacancies.Count} vacancies", ConsoleColor.DarkMagenta);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

            if (vacancies.Count > expectedResult)
                ColoredLine.Write($"Succes!!! This is more than expected vacancies by {vacancies.Count - expectedResult}!\n", ConsoleColor.Green);
            else
                ColoredLine.Write($"Failure((( This is less than expected vacancies by {expectedResult - vacancies.Count}(\n", ConsoleColor.Red);
        }

        #region FindElementWithTimeout

        static IWebElement FindElement(this IWebDriver dr, By by, uint timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                // Webdriver waits until it times out then returns a variable.
                WebDriverWait wait = new WebDriverWait(dr, TimeSpan.FromSeconds(timeoutInSeconds));

                // sometimes gives an error. The solution is to increase the waiting time.
                return wait.Until(drv => drv.FindElement(by));
            }
            return dr.FindElement(by);
        }

        static ReadOnlyCollection<IWebElement> FindElements(this IWebElement parent, By by, uint timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => parent.FindElements(by));
            }
            return driver.FindElements(by);
        }

        static IWebElement FindChildElementByName(this IWebElement parent, By by, string name, uint timeoutInSeconds = 10)
        {
            if (timeoutInSeconds > 0)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv =>
                {
                    // Find "first-line" parent element with the given name.
                    ReadOnlyCollection<IWebElement> webElements = parent.FindElements(by);
                    foreach (IWebElement element in webElements)
                    {
                        if (element.Text == name)
                        {
                            return element;
                        }
                    }

                    return null;
                }
                );
            }
            return parent.FindElement(by);
        }

        #endregion
    }
}
