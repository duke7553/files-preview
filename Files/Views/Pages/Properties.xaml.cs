using Files.DataModels.NavigationControlItems;
using Files.Filesystem;
using Files.ViewModels;
using Files.ViewModels.Properties;
using CommunityToolkit.WinUI;
using System;
using System.Threading;
using Windows.ApplicationModel.Resources.Core;
using Windows.System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;

namespace Files.Views
{
    public partial class Properties : Window
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private object navParameterItem;
        private IShellPage AppInstance;

        private ListedItem listedItem;

        public SettingsViewModel AppSettings => App.AppSettings;
        public PropertiesPageNavigationArguments displayArgs { get; set; }

        public Properties()
        {
            InitializeComponent();
            this.Closed += Properties_Closed;
            AppInstance = displayArgs.AppInstanceArgument;
            navParameterItem = displayArgs.Item;
            listedItem = displayArgs.Item as ListedItem;
            TabShorcut.Visibility = listedItem != null && listedItem.IsShortcutItem ? Visibility.Visible : Visibility.Collapsed;
            TabLibrary.Visibility = listedItem != null && listedItem.IsLibraryItem ? Visibility.Visible : Visibility.Collapsed;
            TabDetails.Visibility = listedItem != null && listedItem.FileExtension != null && !listedItem.IsShortcutItem && !listedItem.IsLibraryItem ? Visibility.Visible : Visibility.Collapsed;
            TabSecurity.Visibility = displayArgs.Item is DriveItem ||
                (listedItem != null && !listedItem.IsLibraryItem && !listedItem.IsRecycleBinItem) ? Visibility.Visible : Visibility.Collapsed;
            //var flowDirectionSetting = ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];

            //if (flowDirectionSetting == "RTL")
            //{
            //    FlowDirection = FlowDirection.RightToLeft;
            //}
        }

        private void Properties_Closed(object sender, WindowEventArgs displayArgs)
        {
            (contentFrame.Content as PropertiesTab).Dispose();
            if (tokenSource != null && !tokenSource.IsCancellationRequested)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
        }

        private async void Properties_Loaded(object sender, RoutedEventArgs e)
        {
            //AppSettings.ThemeModeChanged += AppSettings_ThemeModeChanged;
            await DispatcherQueue.EnqueueAsync(() => AppSettings.UpdateThemeElements.Execute(null));
        }

        private void Properties_Unloaded(object sender, RoutedEventArgs e)
        {
            // Why is this not called? Are we cleaning up properly?
        }

        //private async void AppSettings_ThemeModeChanged(object sender, EventArgs e)
        //{
        //    var selectedTheme = ThemeHelper.RootTheme;
        //    await DispatcherQueue.EnqueueAsync(() =>
        //    {
        //        RequestedTheme = selectedTheme;
        //        //if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        //        //{
        //        //    switch (RequestedTheme)
        //        //    {
        //        //        case ElementTheme.Default:
        //        //            TitleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemBaseLowColor"];
        //        //            TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
        //        //            break;

        //        //        case ElementTheme.Light:
        //        //            TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 0, 0, 0);
        //        //            TitleBar.ButtonForegroundColor = Colors.Black;
        //        //            break;

        //        //        case ElementTheme.Dark:
        //        //            TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 255, 255, 255);
        //        //            TitleBar.ButtonForegroundColor = Colors.White;
        //        //            break;
        //        //    }
        //        //}
        //    });
        //}

        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (contentFrame.Content is PropertiesGeneral propertiesGeneral)
            {
                await propertiesGeneral.SaveChangesAsync(listedItem);
            }
            else
            {
                if (!await (contentFrame.Content as PropertiesTab).SaveChangesAsync(listedItem))
                {
                    return;
                }
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.Equals(VirtualKey.Escape))
            {
                this.Close();
            }
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs displayArgs)
        {
            var navParam = new PropertyNavParam()
            {
                tokenSource = tokenSource,
                navParameter = navParameterItem,
                AppInstanceArgument = AppInstance
            };

            switch (displayArgs.SelectedItemContainer.Tag)
            {
                case "General":
                    contentFrame.Navigate(typeof(PropertiesGeneral), navParam, displayArgs.RecommendedNavigationTransitionInfo);
                    break;

                case "Shortcut":
                    contentFrame.Navigate(typeof(PropertiesShortcut), navParam, displayArgs.RecommendedNavigationTransitionInfo);
                    break;

                case "Library":
                    contentFrame.Navigate(typeof(PropertiesLibrary), navParam, displayArgs.RecommendedNavigationTransitionInfo);
                    break;

                case "Details":
                    contentFrame.Navigate(typeof(PropertiesDetails), navParam, displayArgs.RecommendedNavigationTransitionInfo);
                    break;

                case "Security":
                    contentFrame.Navigate(typeof(PropertiesSecurity), navParam, displayArgs.RecommendedNavigationTransitionInfo);
                    break;
            }
        }

        public class PropertiesPageNavigationArguments
        {
            public object Item { get; set; }
            public IShellPage AppInstanceArgument { get; set; }
        }

        public class PropertyNavParam
        {
            public CancellationTokenSource tokenSource;
            public object navParameter;
            public IShellPage AppInstanceArgument { get; set; }
        }

        private void Page_Loading(FrameworkElement sender, object displayArgs)
        {
            // This manually adds the user's theme resources to the page
            // I was unable to get this to work any other way
            try
            {
                var xaml = XamlReader.Load(App.ExternalResourcesHelper.CurrentThemeResources) as ResourceDictionary;
                App.Current.Resources.MergedDictionaries.Add(xaml);
            }
            catch (Exception)
            {
            }
        }
    }
}