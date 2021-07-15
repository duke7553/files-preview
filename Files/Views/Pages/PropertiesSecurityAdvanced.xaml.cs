using Files.DataModels.NavigationControlItems;
using Files.Filesystem;
using Files.Filesystem.Permissions;
using Files.ViewModels.Properties;
using CommunityToolkit.WinUI;
using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;


namespace Files.Views
{
    public partial class PropertiesSecurityAdvanced : Window
    {
        private object navParameterItem;
        private IShellPage AppInstance;

        public string DialogTitle => string.Format("SecurityAdvancedPermissionsTitle".GetLocalized(), ViewModel.Item.ItemName);

        public SecurityProperties ViewModel { get; set; }
        public PropertiesPageNavigationArguments displayArgs { get; set; }

        public PropertiesSecurityAdvanced()
        {
            this.InitializeComponent();

            AppInstance = displayArgs.AppInstanceArgument;
            navParameterItem = displayArgs.Item;

            if (displayArgs.Item is ListedItem listedItem)
            {
                ViewModel = new SecurityProperties(listedItem, AppInstance);
            }
            else if (displayArgs.Item is DriveItem driveitem)
            {
                ViewModel = new SecurityProperties(driveitem, AppInstance);
            }
        }

        private void Properties_Loaded(object sender, RoutedEventArgs e)
        {
            //App.AppSettings.ThemeModeChanged += AppSettings_ThemeModeChanged;

            ViewModel.GetFilePermissions();
        }

        private void Properties_Unloaded(object sender, RoutedEventArgs e)
        {
            // Why is this not called? Are we cleaning up properly?
        }

        //private async void AppSettings_ThemeModeChanged(object sender, EventArgs e)
        //{
        //    var selectedTheme = ThemeHelper.RootTheme;
        //    await Dispatcher.RunAsync(() =>
        //    {
        //        RequestedTheme = selectedTheme;
        //        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        //        {
        //            switch (RequestedTheme)
        //            {
        //                case ElementTheme.Default:
        //                    TitleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemBaseLowColor"];
        //                    TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
        //                    break;

        //                case ElementTheme.Light:
        //                    TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 0, 0, 0);
        //                    TitleBar.ButtonForegroundColor = Colors.Black;
        //                    break;

        //                case ElementTheme.Dark:
        //                    TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 255, 255, 255);
        //                    TitleBar.ButtonForegroundColor = Colors.White;
        //                    break;
        //            }
        //        }
        //    });
        //}

        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (await ViewModel.SetFilePermissions())
            {
                this.Close();
            }

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

        public class PropertiesPageNavigationArguments
        {
            public object Item { get; set; }
            public IShellPage AppInstanceArgument { get; set; }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedAccessRules = (sender as ListView).SelectedItems.Cast<FileSystemAccessRuleForUI>().ToList();

            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    (item as FileSystemAccessRuleForUI).IsSelected = true;
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems)
                {
                    (item as FileSystemAccessRuleForUI).IsSelected = false;
                }
            }
        }

        private void Grid_Loading(FrameworkElement sender, object args)
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
