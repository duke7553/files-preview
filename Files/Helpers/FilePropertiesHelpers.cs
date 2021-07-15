using Files.Dialogs;
using Files.Views;
using CommunityToolkit.WinUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;


using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using static Files.Views.Properties;

namespace Files.Helpers
{
    public static class FilePropertiesHelpers
    {
        public static void ShowProperties(IShellPage associatedInstance)
        {
            if (associatedInstance.SlimContentPage.IsItemSelected)
            {
                if (associatedInstance.SlimContentPage.SelectedItems.Count > 1)
                {
                    OpenPropertiesWindow(associatedInstance.SlimContentPage.SelectedItems, associatedInstance);
                }
                else
                {
                    OpenPropertiesWindow(associatedInstance.SlimContentPage.SelectedItem, associatedInstance);
                }
            }
            else
            {
                if (!System.IO.Path.GetPathRoot(associatedInstance.FilesystemViewModel.CurrentFolder.ItemPath)
                    .Equals(associatedInstance.FilesystemViewModel.CurrentFolder.ItemPath, StringComparison.OrdinalIgnoreCase))
                {
                    OpenPropertiesWindow(associatedInstance.FilesystemViewModel.CurrentFolder, associatedInstance);
                }
                else
                {
                    OpenPropertiesWindow(App.DrivesManager.Drives
                        .SingleOrDefault(x => x.Path.Equals(associatedInstance.FilesystemViewModel.CurrentFolder.ItemPath)), associatedInstance);
                }
            }
        }

        public static void OpenPropertiesWindow(object item, IShellPage associatedInstance)
        {
            if (item == null)
            {
                return;
            }

            Properties newWindow = new Properties();
            newWindow.displayArgs = new PropertiesPageNavigationArguments()
            {
                Item = item,
                AppInstanceArgument = associatedInstance
            };
            newWindow.ExtendsContentIntoTitleBar = true;
            newWindow.Title = "PropertiesTitle".GetLocalized();
            newWindow.Activate();
            //TODO: Set bounds
        }
    }
}