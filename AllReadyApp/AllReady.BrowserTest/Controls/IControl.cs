using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace AllReady.BrowserTest.Controls
{
    abstract class IControl
    {
        public abstract void Set(string s);
    }
}
