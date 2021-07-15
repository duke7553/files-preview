using Files.Dialogs;
using Files.ViewModels;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;

namespace Files.SettingsPages
{
    public sealed partial class Appearance : Page
    {
        public Appearance()
        {
            InitializeComponent();
        }

        private void ThemesLearnMoreButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ThemesTeachingTip.IsOpen = true;
        }

        private void OpenThemesFolderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.FindAscendant<SettingsDialog>()?.Hide();
            SettingsViewModel.OpenThemesFolder();
        }
    }
}