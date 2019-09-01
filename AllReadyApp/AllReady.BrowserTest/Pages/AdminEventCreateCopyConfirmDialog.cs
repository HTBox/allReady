using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    class AdminEventCreateCopyConfirmDialog
    {
        IWebDriver _driver;
        WebDriverWait _wait;
        IWebElement _confirmDialog;

        public AdminEventCreateCopyConfirmDialog(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));

            _confirmDialog = _wait.Until<IWebElement>((_driver) =>
            {
                var e = _driver.FindElement(By.Id("confirmLocationModal"));
                return e.Displayed ? e : null;
            });
            Actions action = new Actions(_driver);
            action.MoveToElement(_confirmDialog).Perform();
        }

        public void ClickOK()
        {
            _driver.FindElement(By.Id("confirmOverwriteLocation")).Click();
            _wait.Until<IWebElement>((_driver) =>
            {
                return _confirmDialog.Displayed ? null : _confirmDialog;
            });
        }
    }
}
