using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Files.Helpers
{
    internal static class BitmapHelper
    {
        public static async Task<BitmapImage> ToBitmapAsync(this byte[] data)
        {
            if (data is null)
            {
                return null;
            }

            using var ms = new MemoryStream(data);
            var image = new BitmapImage();
            await image.SetSourceAsync(ms.AsRandomAccessStream());
            return image;
        }
    }
}