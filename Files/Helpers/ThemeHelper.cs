using Files.Extensions;
using System;
using Windows.Storage;
using Microsoft.UI;

using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using Windows.UI;
using CommunityToolkit.WinUI;

namespace Files.Helpers
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private const string selectedAppThemeKey = "theme";

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                var savedTheme = ApplicationData.Current.LocalSettings.Values[selectedAppThemeKey]?.ToString();

                if (!string.IsNullOrEmpty(savedTheme))
                {
                    return EnumExtensions.GetEnum<ElementTheme>(savedTheme);
                }
                else
                {
                    return ElementTheme.Default;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[selectedAppThemeKey] = value.ToString();
                ApplyTheme();
            }
        }

        public static void Initialize()
        {
            //Apply the desired theme based on what is set in the application settings
            ApplyTheme();

            // Registering to color changes, thus we notice when user changes theme system wide
            // TODO
        }

        //private static async void UiSettings_ColorValuesChanged(UISettings sender, object args)
        //{
        //    // Make sure we have a reference to our window so we dispatch a UI change
        //    if (App.MainWindow != null)
        //    {
        //        // Dispatch on UI thread so that we have a current appbar to access and change
        //        await DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
        //        {
        //            ApplyTheme();
        //        }, DispatcherQueuePriority.High);
        //    }
        //}

        private static void ApplyTheme()
        {
            var rootTheme = RootTheme;

            if (App.MainWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = rootTheme;
            }

            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            switch (rootTheme)
            {
                case ElementTheme.Default:
                    App.AppSettings.AcrylicTheme.SetDefaultTheme();
                    //titleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemBaseLowColor"];
                    //titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
                    break;

                case ElementTheme.Light:
                    App.AppSettings.AcrylicTheme.SetLightTheme();
                    //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                    //titleBar.ButtonForegroundColor = Colors.Black;
                    break;

                case ElementTheme.Dark:
                    App.AppSettings.AcrylicTheme.SetDarkTheme();
                    //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                    //titleBar.ButtonForegroundColor = Colors.White;
                    break;
            }
            App.AppSettings.UpdateThemeElements.Execute(null);
        }
    }
}