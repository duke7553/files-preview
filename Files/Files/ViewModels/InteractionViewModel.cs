using Files.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Files.ViewModels
{
    public class InteractionViewModel : ObservableObject
    {
        public SettingsViewModel AppSettings => App.AppSettings;
        private MainWindow mainWindowInstance = null;
        public InteractionViewModel()
        {
            SetDefaultValues();
        }

        private async void SetDefaultValues()
        {
            await App.mainWindow.DispatcherQueue.EnqueueAsync(() =>
            {
                mainWindowInstance = App.mainWindow;
                mainWindowInstance.SizeChanged += Current_SizeChanged;
            });
            isWindowCompactSize = IsWindowResizedToCompactWidth();
            isHorizontalTabStripVisible = App.AppSettings.IsMultitaskingExperienceAdaptive ? !IsWindowResizedToCompactWidth() : App.AppSettings.IsHorizontalTabStripEnabled;
            isVerticalTabFlyoutVisible = App.AppSettings.IsMultitaskingExperienceAdaptive ? IsWindowResizedToCompactWidth() : App.AppSettings.IsVerticalTabFlyoutEnabled;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            IsWindowCompactSize = IsWindowResizedToCompactWidth();

            if (AppSettings.IsMultitaskingExperienceAdaptive)
            {
                if (IsWindowCompactSize)
                {
                    IsVerticalTabFlyoutVisible = true;
                    IsHorizontalTabStripVisible = false;
                }
                else if (!IsWindowCompactSize)
                {
                    IsVerticalTabFlyoutVisible = false;
                    IsHorizontalTabStripVisible = true;
                }
            }
            else
            {
                IsVerticalTabFlyoutVisible = false;
                IsHorizontalTabStripVisible = false;
            }
        }

        private int tabStripSelectedIndex = 0;

        public int TabStripSelectedIndex
        {
            get => tabStripSelectedIndex;
            set
            {
                if (value >= 0)
                {
                    if (tabStripSelectedIndex != value)
                    {
                        SetProperty(ref tabStripSelectedIndex, value);
                    }
                    if (value < MainWindow.MultitaskingControl.Items.Count)
                    {
                        mainWindowInstance.SelectedTabItem = MainWindow.MultitaskingControl.Items[value];
                    }
                }
            }
        }

        private bool isPasteEnabled = false;

        public bool IsPasteEnabled
        {
            get => isPasteEnabled;
            set => SetProperty(ref isPasteEnabled, value);
        }

        private bool isHorizontalTabStripVisible;

        public bool IsHorizontalTabStripVisible
        {
            get => isHorizontalTabStripVisible;
            set => SetProperty(ref isHorizontalTabStripVisible, value);
        }

        private bool isVerticalTabFlyoutVisible;

        public bool IsVerticalTabFlyoutVisible
        {
            get => isVerticalTabFlyoutVisible;
            set => SetProperty(ref isVerticalTabFlyoutVisible, value);
        }

        private bool isWindowCompactSize;

        public bool IsWindowResizedToCompactWidth()
        {
            return mainWindowInstance.Bounds.Width <= 750;
        }

        public bool IsWindowCompactSize
        {
            get => isWindowCompactSize;
            set
            {
                SetProperty(ref isWindowCompactSize, value);
                if (value)
                {
                    IsHorizontalTabStripVisible = false;
                    IsVerticalTabFlyoutVisible = true;
                }
                else
                {
                    IsHorizontalTabStripVisible = true;
                    IsVerticalTabFlyoutVisible = false;
                }
            }
        }
    }
}