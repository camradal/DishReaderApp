using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DishReaderApp.Resources;
using Microsoft.Phone.Controls;

namespace DishReaderApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // application title need to be upper case
            ApplicationTitle.Text = Strings.AppTitle.ToUpper(CultureInfo.CurrentCulture);
        }
    }
}