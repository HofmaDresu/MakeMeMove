using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.View;
using Xamarin.Forms;

namespace MakeMeMove
{
    public class RootPage : Xamarin.Forms.MasterDetailPage
    {
        private readonly IUserNotification _userNotification;
        private readonly ISchedulePersistence _schedulePersistence;
        private readonly IProgressPersistence _progressPersistence;

        public RootPage()
        {
            Title = "Menu";
            _userNotification = DependencyService.Get<IUserNotification>();
            _schedulePersistence = DependencyService.Get<ISchedulePersistence>();
            _progressPersistence = DependencyService.Get<IProgressPersistence>();

            var menuPage = new MenuPage();

            menuPage.HomeClicked += (sender, args) =>
            {
                Detail = new NavigationPage(new Main());
                IsPresented = false;
            };

            menuPage.StatsClicked += (sender, args) =>
            {
                Detail = new NavigationPage(new ViewProgress());
                IsPresented = false;
            };

            menuPage.ResetClicked += (sender, args) =>
            {
                _userNotification.ShowAreYouSureDialog("Resetting your data will remove all custom exercises, schedules, and your history data. It will also return you to the main screen;",
                    () =>
                    {
                        _schedulePersistence.RemoveAllData();
                        _progressPersistence.RemoveAllData();
                        Detail = new NavigationPage(new Main());
                        IsPresented = false;
                    });
            };

            Master = menuPage;
            Detail = new NavigationPage(new Main());
        }
            
    }
}
