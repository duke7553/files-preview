using Files.SettingsPages;
using Files.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;

namespace Files.Views
{
    public sealed partial class Settings : Page
    {
        public SettingsViewModel AppSettings => App.AppSettings;

        public Settings()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            // TODO: Extend into titlebar with Win32 api
            //var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //CoreTitleBar.ExtendViewIntoTitleBar = true;
            //Window.Current.SetTitleBar(DragArea);

            //var currentView = SystemNavigationManager.GetForCurrentView();
            //currentView.BackRequested += OnBackRequested;

            // TODO: Fix RTL layout direction
            //ResourceManager resourceManager = new ResourceManager();

            //var flowDirectionSetting = resourceManager.CreateResourceContext().QualifierValues["LayoutDirection"];

            //if (flowDirectionSetting == "RTL")
            //{
            //    FlowDirection = FlowDirection.RightToLeft;
            //}

            SettingsPane.SelectedItem = SettingsPane.MenuItems[0];
        }

        protected override void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            //var currentView = SystemNavigationManager.GetForCurrentView();
            //currentView.BackRequested -= OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = App.mainWindow.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                GoBack();
                e.Handled = true;
            }
        }

        private void SettingsPane_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            GoBack();
        }

        private void GoBack()
        {
            Frame rootFrame = App.mainWindow.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void SettingsPane_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            _ = SettingsPane.MenuItems.IndexOf(SettingsPane.SelectedItem) switch
            {
                0 => SettingsContentFrame.Navigate(typeof(Appearance)),
                1 => SettingsContentFrame.Navigate(typeof(OnStartup)),
                2 => SettingsContentFrame.Navigate(typeof(Preferences)),
                3 => SettingsContentFrame.Navigate(typeof(Widgets)),
                4 => SettingsContentFrame.Navigate(typeof(Multitasking)),
                5 => SettingsContentFrame.Navigate(typeof(FilesAndFolders)),
                6 => SettingsContentFrame.Navigate(typeof(Experimental)),
                7 => SettingsContentFrame.Navigate(typeof(About)),
                _ => SettingsContentFrame.Navigate(typeof(Appearance))
            };
        }
    }
}