using Files.Common;
using Files.DataModels;
using Files.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Files.Helpers
{
    public static class UIHelpers
    {
        public static bool IsAnyContentDialogOpen()
        {
            var openedPopups = VisualTreeHelper.GetOpenPopups(App.MainWindow);
            return openedPopups.Any(popup => popup.Child is ContentDialog);
        }

        public static async Task<IList<IconFileInfo>> LoadSelectedIconsAsync(string filePath, IList<int> indexes, int iconSize = 48, bool rawDataOnly = true)
        {
            var connection = await AppServiceConnectionHelper.Instance;
            if (connection != null)
            {
                var value = new ValueSet
                {
                    { "Arguments", "GetSelectedIconsFromDLL" },
                    { "iconFile", filePath },
                    { "requestedIconSize", iconSize },
                    { "iconIndexes", JsonConvert.SerializeObject(indexes) }
                };
                var (status, response) = await connection.SendMessageForResponseAsync(value);
                if (status == AppServiceResponseStatus.Success)
                {
                    var icons = JsonConvert.DeserializeObject<IList<IconFileInfo>>((string)response["IconInfos"]);

                    if (icons != null && !rawDataOnly)
                    {
                        foreach (IconFileInfo iFInfo in icons)
                        {
                            await iFInfo.LoadImageFromModelString();
                        }
                    }

                    return icons;
                }
            }
            return null;
        }

        public static BitmapImage GetImageForIconOrNull(object image)
        {
            if (SidebarPinnedModel.IconResources is null)
            {
                return null;
            }
            else
            {
                return (BitmapImage)image;
            }
        }
    }
}