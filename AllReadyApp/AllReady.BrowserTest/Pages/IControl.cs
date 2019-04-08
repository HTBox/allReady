using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace AllReady.BrowserTest.Pages
{
    abstract class IControl
    {
        public abstract void Set(string s);
    }
}
