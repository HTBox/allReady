using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Controls
{
    class TinyMCE : IControl
    {
        IWebElement _element;
        IWebDriver _driver;

        public TinyMCE(IWebDriver driver, string iFrameID)
        {
            _driver = driver;
            _element = FindTinyMCEByFrameId(iFrameID);
        }

        IWebElement FindTinyMCEByFrameId(string iFrameID)
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
            var element = wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.Id(iFrameID));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(element).Perform();
            // 
            _driver.SwitchTo().Frame(element);

            return _driver.FindElement(By.Id("tinymce"));
        }

        public void SendKeys(string text)
        {
            _element.SendKeys(text);
            _driver.SwitchTo().DefaultContent();
        }

        public override void Set(string s)
        {
            SendKeys(s);
        }
    }
}
