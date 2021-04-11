using Files.Controllers;
using Files.Filesystem;
using Files.Filesystem.FilesystemHistory;
using Files.Helpers;
using Files.SettingsInterfaces;
using Files.UserControls.MultitaskingControl;
using Files.ViewModels;
using Files.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Notifications;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using CommunityToolkit.WinUI;

namespace Files
{
    public partial class App : Application
    {
       // private static bool ShowErrorNotification = false;

        public static StorageHistoryWrapper HistoryWrapper = new StorageHistoryWrapper();

        public static IBundlesSettings BundlesSettings = new BundlesSettingsViewModel();
        public static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        public static SettingsViewModel AppSettings { get; set; }
        public static InteractionViewModel InteractionViewModel { get; set; }
        public static JumpListManager JumpList { get; } = new JumpListManager();
        public static SidebarPinnedController SidebarPinnedController { get; set; }
        public static CloudDrivesManager CloudDrivesManager { get; set; }
        public static DrivesManager DrivesManager { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class AppData
        {
            // Get the extensions that are available for this host.
            // Extensions that declare the same contract string as the host will be recognized.
            internal static ExtensionManager FilePreviewExtensionManager { get; set; } = new ExtensionManager("com.files.filepreview");
        }

        public App()
        {
            //UnhandledException += OnUnhandledException;
            //TaskScheduler.UnobservedTaskException += OnUnobservedException;

            InitializeComponent();
            //Suspending += OnSuspending;
            //LeavingBackground += OnLeavingBackground;
            Clipboard.ContentChanged += Clipboard_ContentChanged;
            // Initialize NLog
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            LogManager.Configuration.Variables["LogPath"] = storageFolder.Path;
            AppData.FilePreviewExtensionManager.Initialize(); // The extension manager can update UI, so pass it the UI dispatcher to use for UI updates

            StartAppCenter();
        }

        private async void StartAppCenter()
        {
            JObject obj;
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Resources/AppCenterKey.txt"));
                var lines = await FileIO.ReadTextAsync(file);
                obj = JObject.Parse(lines);
            }
            catch
            {
                return;
            }

            AppCenter.Start((string)obj.SelectToken("key"), typeof(Analytics), typeof(Crashes));
        }

        private void OnLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            DrivesManager?.ResumeDeviceWatcher();
        }

        public static INavigationControlItem RightClickedItem;

        public static void UnpinItem_Click(object sender, RoutedEventArgs e)
        {
            if (RightClickedItem.Path.Equals(AppSettings.RecycleBinPath, StringComparison.OrdinalIgnoreCase))
            {
                AppSettings.PinRecycleBinToSideBar = false;
            }
            else
            {
                SidebarPinnedController.Model.RemoveItem(RightClickedItem.Path.ToString());
            }
        }

        public static Microsoft.UI.Xaml.UnhandledExceptionEventArgs ExceptionInfo { get; set; }
        public static string ExceptionStackTrace { get; set; }
        public static List<string> pathsToDeleteAfterPaste = new List<string>();
        public static MainWindow mainWindow;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            //start tracking app usage
            //SystemInformation.Instance.TrackAppUse(args.);

            Logger.Info("App launched");

            //bool canEnablePrelaunch = ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            mainWindow = new MainWindow();
            mainWindow.Activate();
            mainWindow.ExtendsContentIntoTitleBar = true;
            //ShowErrorNotification = true;
            ApplicationData.Current.LocalSettings.Values["INSTANCE_ACTIVE"] = Process.GetCurrentProcess().Id;
            Clipboard_ContentChanged(null, null);
        }

        private void Clipboard_ContentChanged(object sender, object e)
        {
            if (App.InteractionViewModel == null)
            {
                return;
            }

            try
            {
                // Clipboard.GetContent() will throw UnauthorizedAccessException
                // if the app window is not in the foreground and active
                DataPackageView packageView = Clipboard.GetContent();
                if (packageView.Contains(StandardDataFormats.StorageItems) || packageView.Contains(StandardDataFormats.Bitmap))
                {
                    App.InteractionViewModel.IsPasteEnabled = true;
                }
                else
                {
                    App.InteractionViewModel.IsPasteEnabled = false;
                }
            }
            catch
            {
                App.InteractionViewModel.IsPasteEnabled = false;
            }
        }

        //protected override async void OnActivated(IActivatedEventArgs args)
        //{
        //    Logger.Info("App activated");

        //    await EnsureSettingsAndConfigurationAreBootstrapped();

        //    // Window management
        //    if (!(mainWindow.Content is Frame rootFrame))
        //    {
        //        rootFrame = new Frame();
        //        rootFrame.CacheSize = 1;
        //        mainWindow.Content = rootFrame;
        //    }

        //    //var currentView = SystemNavigationManager.GetForCurrentView();
        //    switch (args.Kind)
        //    {
        //        case ActivationKind.Protocol:
        //            var eventArgs = args as ProtocolActivatedEventArgs;

        //            if (eventArgs.Uri.AbsoluteUri == "files-uwp:")
        //            {
        //                rootFrame.Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());
        //            }
        //            else
        //            {
        //                var parsedArgs = eventArgs.Uri.Query.TrimStart('?').Split('=');
        //                var unescapedValue = Uri.UnescapeDataString(parsedArgs[1]);
        //                switch (parsedArgs[0])
        //                {
        //                    case "tab":
        //                        rootFrame.Navigate(typeof(MainPage), TabItemArguments.Deserialize(unescapedValue), new SuppressNavigationTransitionInfo());
        //                        break;

        //                    case "folder":
        //                        rootFrame.Navigate(typeof(MainPage), unescapedValue, new SuppressNavigationTransitionInfo());
        //                        break;
        //                }
        //            }

        //            // Ensure the current window is active.
        //            mainWindow.Activate();
        //            mainWindow.Activated += MainWindow_Activated;
        //            return;

        //        case ActivationKind.CommandLineLaunch:
        //            var cmdLineArgs = args as CommandLineActivatedEventArgs;
        //            var operation = cmdLineArgs.Operation;
        //            var cmdLineString = operation.Arguments;
        //            var activationPath = operation.CurrentDirectoryPath;

        //            var parsedCommands = CommandLineParser.ParseUntrustedCommands(cmdLineString);

        //            if (parsedCommands != null && parsedCommands.Count > 0)
        //            {
        //                foreach (var command in parsedCommands)
        //                {
        //                    switch (command.Type)
        //                    {
        //                        case ParsedCommandType.OpenDirectory:
        //                            rootFrame.Navigate(typeof(MainPage), command.Payload, new SuppressNavigationTransitionInfo());

        //                            // Ensure the current window is active.
        //                            mainWindow.Activate();
        //                            mainWindow.Activated += MainWindow_Activated;
        //                            return;

        //                        case ParsedCommandType.OpenPath:

        //                            try
        //                            {
        //                                var det = await StorageFolder.GetFolderFromPathAsync(command.Payload);

        //                                rootFrame.Navigate(typeof(MainPage), command.Payload, new SuppressNavigationTransitionInfo());

        //                                // Ensure the current window is active.
        //                                mainWindow.Activate();
        //                                mainWindow.Activated += MainWindow_Activated;

        //                                return;
        //                            }
        //                            catch (System.IO.FileNotFoundException ex)
        //                            {
        //                                //Not a folder
        //                                Debug.WriteLine($"File not found exception App.xaml.cs\\OnActivated with message: {ex.Message}");
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Debug.WriteLine($"Exception in App.xaml.cs\\OnActivated with message: {ex.Message}");
        //                            }

        //                            break;

        //                        case ParsedCommandType.Unknown:
        //                            rootFrame.Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());
        //                            // Ensure the current window is active.
        //                            mainWindow.Activate();
        //                            mainWindow.Activated += MainWindow_Activated;

        //                            return;
        //                    }
        //                }
        //            }
        //            break;

        //        case ActivationKind.ToastNotification:
        //            var eventArgsForNotification = args as ToastNotificationActivatedEventArgs;
        //            if (eventArgsForNotification.Argument == "report")
        //            {
        //                // Launch the URI and open log files location
        //                //SettingsViewModel.OpenLogLocation();
        //                SettingsViewModel.ReportIssueOnGitHub();
        //            }
        //            break;
        //    }

        //    rootFrame.Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());

        //    // Ensure the current window is active.
        //    mainWindow.Activate();
        //    mainWindow.Activated += MainWindow_Activated;
        //}

        //private void TryEnablePrelaunch()
        //{
        //    //CoreApplication.EnablePrelaunch(true);
        //}

        ///// <summary>
        ///// Invoked when Navigation to a certain page fails
        ///// </summary>
        ///// <param name="sender">The Frame which failed navigation</param>
        ///// <param name="e">Details about the navigation failure</param>
        //private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        //{
        //    throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        //}

        ///// <summary>
        ///// Invoked when application execution is being suspended.  Application state is saved
        ///// without knowing whether the application will be terminated or resumed with the contents
        ///// of memory still intact.
        ///// </summary>
        ///// <param name="sender">The source of the suspend request.</param>
        ///// <param name="e">Details about the suspend request.</param>
        //private void OnSuspending(object sender, SuspendingEventArgs e)
        //{
        //    SaveSessionTabs();

        //    var deferral = e.SuspendingOperation.GetDeferral();
        //    //TODO: Save application state and stop any background activity

        //    DrivesManager?.Dispose();
        //    deferral.Complete();
        //}

        public static void SaveSessionTabs() // Enumerates through all tabs and gets the Path property and saves it to AppSettings.LastSessionPages
        {
            if (AppSettings != null)
            {
                AppSettings.LastSessionPages = MainWindow.AppInstances.DefaultIfEmpty().Select(tab =>
                {
                    if (tab != null && tab.TabItemArguments != null)
                    {
                        return tab.TabItemArguments.Serialize();
                    }
                    else
                    {
                        var defaultArg = new TabItemArguments() { InitialPageType = typeof(PaneHolderPage), NavigationArg = "NewTab".GetLocalized() };
                        return defaultArg.Serialize();
                    }
                }).ToArray();
            }
        }

        //// Occurs when an exception is not handled on the UI thread.
        //private static void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) => AppUnhandledException(e.Exception);

        //// Occurs when an exception is not handled on a background thread.
        //// ie. A task is fired and forgotten Task.Run(() => {...})
        //private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e) => AppUnhandledException(e.Exception);

        //private static void AppUnhandledException(Exception ex)
        //{
        //    Logger.Error(ex, ex.Message);
        //    if (ShowErrorNotification)
        //    {
        //        var toastContent = new ToastContent()
        //        {
        //            Visual = new ToastVisual()
        //            {
        //                BindingGeneric = new ToastBindingGeneric()
        //                {
        //                    Children =
        //                    {
        //                        new AdaptiveText()
        //                        {
        //                            Text = "ExceptionNotificationHeader".GetLocalized()
        //                        },
        //                        new AdaptiveText()
        //                        {
        //                            Text = "ExceptionNotificationBody".GetLocalized()
        //                        }
        //                    },
        //                    AppLogoOverride = new ToastGenericAppLogo()
        //                    {
        //                        Source = "ms-appx:///Assets/error.png"
        //                    }
        //                }
        //            },
        //            Actions = new ToastActionsCustom()
        //            {
        //                Buttons =
        //                {
        //                    new ToastButton("ExceptionNotificationReportButton".GetLocalized(), "report")
        //                    {
        //                        ActivationType = ToastActivationType.Foreground
        //                    }
        //                }
        //            }
        //        };

        //        // Create the toast notification
        //        var toastNotif = new ToastNotification(toastContent.GetXml());

        //        // And send the notification
        //        ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        //    }
        //}

        public static void CloseApp()
        {
            Application.Current.Exit();
        }
    }

    public class WSLDistroItem : INavigationControlItem
    {
        public string Glyph { get; set; } = null;

        public string Text { get; set; }

        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                HoverDisplayText = Path.Contains("?") ? Text : Path;
            }
        }
        public string HoverDisplayText { get; private set; }

        public NavigationControlItemType ItemType => NavigationControlItemType.LinuxDistro;

        public Uri Logo { get; set; }
    }
}