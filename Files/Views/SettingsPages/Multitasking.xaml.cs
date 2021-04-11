using Files.ViewModels.SettingsViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Files.SettingsPages
{
    public sealed partial class Multitasking : Page
    {
        public MultitaskingViewModel ViewModel { get; } = new MultitaskingViewModel();

        public Multitasking()
        {
            InitializeComponent();
        }
    }
}