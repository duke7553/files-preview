using Files.Filesystem.Cloud;
using Files.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Files.Filesystem
{
    public class CloudDrivesManager : ObservableObject
    {
        private static readonly Task<CloudDrivesManager> _instanceTask = CreateSingleton();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<DriveItem> drivesList = new List<DriveItem>();

        public IReadOnlyList<DriveItem> Drives
        {
            get
            {
                lock (drivesList)
                {
                    return drivesList.ToList().AsReadOnly();
                }
            }
        }

        //Private as we want to prevent CloudDriveManager being constructed manually
        private CloudDrivesManager()
        { }

        private async Task<CloudDrivesManager> EnumerateDrivesAsync()
        {
            var cloudProviderController = new CloudProviderController();
            await cloudProviderController.DetectInstalledCloudProvidersAsync();

            foreach (var provider in cloudProviderController.CloudProviders)
            {
                var cloudProviderItem = new DriveItem()
                {
                    Text = provider.Name,
                    Path = provider.SyncFolder,
                    Type = DriveType.CloudDrive,
                };
                lock (drivesList)
                {
                    drivesList.Add(cloudProviderItem);
                }
            }

            RefreshUI();

            return this;
        }

        private static async Task<CloudDrivesManager> CreateSingleton()
        {
            var drives = new CloudDrivesManager();
            return await drives.EnumerateDrivesAsync();
        }

        public static Task<CloudDrivesManager> Instance => _instanceTask;

        private void RefreshUI()
        {
            try
            {
                SyncSideBarItemsUI();
            }
            catch (Exception) // UI Thread not ready yet, so we defer the pervious operation until it is.
            {
                System.Diagnostics.Debug.WriteLine($"RefreshUI Exception");
                // Defer because UI-thread is not ready yet (and DriveItem requires it?)
                App.mainWindow.Activated += RefreshUIOnActivated;
            }
        }

        private void RefreshUIOnActivated(object sender, WindowActivatedEventArgs args)
        {
            SyncSideBarItemsUI();
            App.mainWindow.Activated -= RefreshUIOnActivated;
        }

        private void SyncSideBarItemsUI()
        {
            lock (MainWindow.SideBarItems)
                {
                    var drivesSection = MainWindow.SideBarItems.FirstOrDefault(x => x is HeaderTextItem && x.Text == "SidebarCloudDrives".GetLocalized());

                    if (drivesSection != null && Drives.Count == 0)
                    {
                        //No drives - remove the header
                        MainWindow.SideBarItems.Remove(drivesSection);
                    }

                    if (drivesSection == null && Drives.Count > 0)
                    {
                        drivesSection = new HeaderTextItem()
                        {
                            Text = "SidebarCloudDrives".GetLocalized()
                        };

                        //Get the last location item in the sidebar
                        var lastLocationItem = MainWindow.SideBarItems.LastOrDefault(x => x is LocationItem);

                        if (lastLocationItem != null)
                        {
                            //Get the index of the last location item
                            var lastLocationItemIndex = MainWindow.SideBarItems.IndexOf(lastLocationItem);
                            //Insert the drives title beneath it
                            MainWindow.SideBarItems.Insert(lastLocationItemIndex + 1, drivesSection);
                        }
                        else
                        {
                            MainWindow.SideBarItems.Add(drivesSection);
                        }
                    }

                    var sectionStartIndex = MainWindow.SideBarItems.IndexOf(drivesSection);

                    //Remove all existing cloud drives from the sidebar
                    foreach (var item in MainWindow.SideBarItems
                        .Where(x => x.ItemType == NavigationControlItemType.CloudDrive)
                        .ToList())
                    {
                        MainWindow.SideBarItems.Remove(item);
                    }

                    //Add all cloud drives to the sidebar
                    var insertAt = sectionStartIndex + 1;
                    foreach (var drive in Drives.OrderBy(o => o.Text))
                    {
                        MainWindow.SideBarItems.Insert(insertAt, drive);
                        insertAt++;
                    }
                }
        }
    }
}