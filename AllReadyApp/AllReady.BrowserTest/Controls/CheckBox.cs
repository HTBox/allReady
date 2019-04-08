using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Controls
{
    class CheckBox
    {
        IWebDriver _driver;
        IWebElement _element;

        public CheckBox(IWebDriver driver, string id)
        {
            _driver = driver;
            _element = _driver.FindElement(By.Id(id));
        }

        public void Checked(bool isChecked)
        {
            if ((_element.Selected && !isChecked) || (!_element.Selected && isChecked))
            {
                _element.Click();
            }
        }
    }
}
