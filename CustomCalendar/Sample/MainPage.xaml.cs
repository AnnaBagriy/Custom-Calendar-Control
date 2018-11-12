using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CustomCalendar.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            CalendarControl.SelectedDateCommand = new Command<object>((obj) =>
            {
                var s = obj;
            });
        }
    }
}