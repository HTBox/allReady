using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class Spinner
    {
        IWebDriver _driver;
        IWebElement _element;

        public Spinner(IWebDriver driver, string id)
        {
            _driver = driver;
            _element = _driver.FindElement(By.Id(id));
        }

        public void SendKeys(string s)
        {
            _element.SendKeys(Keys.Control + "a");
            _element.SendKeys(s);
        }
    }
}
