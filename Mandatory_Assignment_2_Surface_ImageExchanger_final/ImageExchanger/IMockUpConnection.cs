using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ImageExchanger
{
    interface IMockUpConnection
    {
        List<Image> SendImages1();
        List<Image> SendImages2();
        List<Image> SendImages3();
    }
}
