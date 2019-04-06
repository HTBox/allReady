using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class DateTimePicker
    {
        IWebDriver _driver;
        IWebElement _element;
        WebDriverWait wait;

        public DateTimePicker(IWebDriver driver, string id)
        {
            _driver = driver;
            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            _element = _driver.FindElement(By.Id(id));
        }

        IWebElement FindElement(string id)
        {
            var element = _driver.FindElement(By.Id(id));
            element.Click();
            //wait.Until<IWebElement>(_driver =>
            //{
            //    var e = _driver.FindElement(By.ClassName("bootstrap-datetimepicker-widget"));
            //    return e.Displayed ? e : null;
            //});
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count > 0);

            //return _driver.FindElement(By.Id(id));
            return element;
        }

        public void SendKeys(string text)
        {
            _element.SendKeys(Keys.Control + "a");
            _element.SendKeys(text);
            _element.SendKeys(Keys.Enter);
            wait.Until(_driver => _driver.FindElements(By.ClassName("bootstrap-datetimepicker-widget")).Count == 0);
        }
    }
}
