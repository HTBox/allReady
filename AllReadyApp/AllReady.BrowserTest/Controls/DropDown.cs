using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class DropDown : IControl
    {
        IWebDriver _driver;
        SelectElement _selectElement;

        public DropDown(IWebDriver driver, string id)
        {
            _driver = driver;
            _selectElement = new SelectElement(_driver.FindElement(By.Id(id)));
        }

        public void SelectByText(string text, bool partialMatch = false)
        {
            _selectElement.SelectByText(text, partialMatch);
        }

        public void SelectLast()
        {
            _selectElement.SelectByIndex(_selectElement.Options.Count - 1);
        }

        public override void Set(string s)
        {
            SelectByText(s);
        }
    }
}
