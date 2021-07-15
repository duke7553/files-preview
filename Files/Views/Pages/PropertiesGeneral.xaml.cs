using Files.Common;
using Files.Filesystem;
using Files.Helpers;
using Files.ViewModels.Properties;
using CommunityToolkit.WinUI;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Windows.Foundation.Collections;

namespace Files.Views
{
    public sealed partial class PropertiesGeneral : PropertiesTab
    {
        public PropertiesGeneral()
        {
            this.InitializeComponent();
            base.hashProgress = new Progress<float>();

            (base.hashProgress as Progress<float>).ProgressChanged += PropertiesGeneral_ProgressChanged;
        }

        private void PropertiesGeneral_ProgressChanged(object sender, float e)
        {
            ItemMD5HashProgress.Value = (double)e;
        }

        public override async Task<bool> SaveChangesAsync(ListedItem item)
        {
            if (BaseProperties is DriveProperties driveProps)
            {
                var drive = driveProps.Drive;
                ViewModel.ItemName = ItemFileName.Text; // Make sure ItemName is updated
                if (!string.IsNullOrWhiteSpace(ViewModel.ItemName) && ViewModel.OriginalItemName != ViewModel.ItemName)
                {
                    if (AppInstance.ServiceConnection != null && AppInstance.FilesystemViewModel != null)
                    {
                        _ = await AppInstance.ServiceConnection.SendMessageForResponseAsync(new ValueSet()
                        {
                            { "Arguments", "SetVolumeLabel" },
                            { "drivename", drive.Path },
                            { "newlabel", ViewModel.ItemName }
                        });
                        await DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
                        {
                            await drive.UpdateLabelAsync();
                            await AppInstance.FilesystemViewModel?.SetWorkingDirectoryAsync(drive.Path);
                        });
                        return true;
                    }
                }
            }
            else if (BaseProperties is LibraryProperties libProps)
            {
                var library = libProps.Library;
                ViewModel.ItemName = ItemFileName.Text; // Make sure ItemName is updated
                var newName = ViewModel.ItemName;
                if (!string.IsNullOrWhiteSpace(newName) && ViewModel.OriginalItemName != newName)
                {
                    if (AppInstance.FilesystemViewModel != null && App.LibraryManager.CanCreateLibrary(newName).result)
                    {
                        var libraryPath = library.ItemPath;
                        var renamed = await AppInstance.FilesystemHelpers.RenameAsync(new StorageFileWithPath(null, libraryPath), newName, Windows.Storage.NameCollisionOption.FailIfExists, false);
                        if (renamed == Enums.ReturnResult.Success)
                        {
                            var newPath = Path.Combine(Path.GetDirectoryName(libraryPath), $"{newName}{ShellLibraryItem.EXTENSION}");
                            _ = DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
                            {
                                await AppInstance.FilesystemViewModel?.SetWorkingDirectoryAsync(newPath);
                            });
                            return true;
                        }
                    }
                }
            }
            else
            {
                ViewModel.ItemName = ItemFileName.Text; // Make sure ItemName is updated
                if (!string.IsNullOrWhiteSpace(ViewModel.ItemName) && ViewModel.OriginalItemName != ViewModel.ItemName)
                {
                    return await DispatcherQueue.GetForCurrentThread().EnqueueAsync(() => UIFilesystemHelpers.RenameFileItemAsync(item,
                          ViewModel.OriginalItemName,
                          ViewModel.ItemName,
                          AppInstance));
                }

                // Handle the hidden attribute
                if (BaseProperties is CombinedProperties combinedProps)
                {
                    // Handle each file independently
                    if (AppInstance?.SlimContentPage?.ItemManipulationModel != null) // null on homepage
                    {
                        foreach (var fileOrFolder in combinedProps.List)
                        {
                            await DispatcherQueue.GetForCurrentThread().EnqueueAsync(() => UIFilesystemHelpers.SetHiddenAttributeItem(fileOrFolder, ViewModel.IsHidden, AppInstance.SlimContentPage.ItemManipulationModel));
                        }
                    }
                    return true;
                }
                else
                {
                    // Handle the visibility attribute for a single file
                    if (AppInstance?.SlimContentPage?.ItemManipulationModel != null) // null on homepage
                    {
                        await DispatcherQueue.GetForCurrentThread().EnqueueAsync(() => UIFilesystemHelpers.SetHiddenAttributeItem(item, ViewModel.IsHidden, AppInstance.SlimContentPage.ItemManipulationModel));
                    }
                    return true;
                }
            }

            return false;
        }

        public override void Dispose()
        {
            (base.hashProgress as Progress<float>).ProgressChanged -= PropertiesGeneral_ProgressChanged;
        }
    }
}