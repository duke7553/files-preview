using Files.Enums;
using Files.Helpers;
using Files.Interacts;
using Files.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls
{
    public sealed partial class StatusCenter : UserControl
    {
        public StatusCenterViewModel StatusCenterViewModel { get; set; }
        public StatusCenter()
        {
            this.InitializeComponent();
        }

        // Dismiss banner button event handler
        private void DismissBanner(object sender, RoutedEventArgs e)
        {
            StatusBanner itemToDismiss = (sender as Button).DataContext as StatusBanner;
            StatusCenterViewModel.CloseBanner(itemToDismiss);
        }

        // Primary action button click
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StatusBanner itemToDismiss = (sender as Button).DataContext as StatusBanner;
            await Task.Run(itemToDismiss.PrimaryButtonClick);
            StatusCenterViewModel.CloseBanner(itemToDismiss);
        }
    }
}