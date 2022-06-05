using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumWebDriverTest
{
    static class Program
    {
        static IWebDriver driver;

        static string department = "Research & Development";
        static int expectedResult = 15;
        static uint timeout = 10;

        static void Main(string[] args)
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://careers.veeam.com/vacancies");

            //timeout = Convert.ToUInt32(Console.ReadLine());

            IWebElement dropdownBox = driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/header/div/div/div[2]/div"), timeout);
            dropdownBox.Click();

            IWebElement dropdownItem = dropdownBox.FindChildElementByName(By.ClassName("dropdown-item"), department, timeout);
            dropdownItem.Click();

            driver.FindElement(By.CssSelector("button[class='btn btn-outline-success']"), timeout).Click();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);

            ReadOnlyCollection<IWebElement> vacancies = driver.FindElement(By.CssSelector("div[class='d-none d-lg-block']")).FindElements(By.XPath("*"));

            driver.FindElement(By.CssSelector("input[class='dropdown-toggle form-control']")).SendKeys($"found {vacancies.Count} vacancies");

            Console.WriteLine(vacancies.Count);

            if (vacancies.Count > expectedResult)
            {
                Console.WriteLine("succes!!!");
            }
        }

        static IWebElement FindElement(this IWebDriver driver, By by, uint timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        static IWebElement FindChildElementByName(this IWebElement parent, By by, string name, uint timeoutInSeconds = 10)
        {
            if (timeoutInSeconds > 0)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv =>
                {
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
    }
}
