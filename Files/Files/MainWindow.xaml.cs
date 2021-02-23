using Files.ViewModels;
using Files.Views;
using Microsoft.System;
using Microsoft.Toolkit.Uwp.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Files
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Window management
            if (!(Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.CacheSize = 1;
                Content = rootFrame;
            }
            this.Activated += MainWindow_Activated;
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            this.Activated -= MainWindow_Activated;

            await EnsureSettingsAndConfigurationAreBootstrapped();
            await this.DispatcherQueue.EnqueueAsync(() =>
            {
                (this.Content as Frame).Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());

            });

        }

        internal static async Task EnsureSettingsAndConfigurationAreBootstrapped()
        {

            if (App.CloudDrivesManager == null)
            {
                //Enumerate cloud drives on in the background. It will update the UI itself when finished
                _ = Files.Filesystem.CloudDrivesManager.Instance.ContinueWith(o =>
                {
                    App.CloudDrivesManager = o.Result;
                });
            }

            //Start off a list of tasks we need to run before we can continue startup
            var tasksToRun = new List<Task>();

            if (App.AppSettings == null)
            {
                await DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
                {
                    //We can't create AppSettings at the same time as everything else as other dependencies depend on AppSettings
                    App.AppSettings = await SettingsViewModel.CreateInstance();
                    if (App.AppSettings?.AcrylicTheme == null)
                    {
                        Helpers.ThemeHelper.Initialize();
                    }
                });
            }

            if (App.SidebarPinnedController == null)
            {
                await Files.Controllers.SidebarPinnedController.CreateInstance().ContinueWith(o => App.SidebarPinnedController = o.Result);

            }

            if (App.DrivesManager == null)
            {
                tasksToRun.Add(Files.Filesystem.DrivesManager.Instance.ContinueWith(o => App.DrivesManager = o.Result));
            }

            if (App.InteractionViewModel == null)
            {
                App.InteractionViewModel = new InteractionViewModel();
            }

            if (tasksToRun.Any())
            {
                //Only proceed when all tasks are completed
                await Task.WhenAll(tasksToRun);
            }
        }
    }
}
