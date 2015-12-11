using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class MenuPage : ContentPage
    {
        public EventHandler HomeClicked;
        public EventHandler StatsClicked;
        public EventHandler ResetClicked;

        public MenuPage()
        {
            InitializeComponent();
            Title = "Make Me Move";
            Icon = "settings.png";

            Home.Clicked += (sender, args) => { HomeClicked?.Invoke(sender, args); };
            Stats.Clicked += (sender, args) => { StatsClicked?.Invoke(sender, args); };
            Reset.Clicked += (sender, args) => { ResetClicked?.Invoke(sender, args); };
        }
    }
}
