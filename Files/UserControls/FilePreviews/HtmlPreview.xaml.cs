using Files.ViewModels.Previews;
using Microsoft.UI.Xaml.Controls;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls.FilePreviews
{
    public sealed partial class HtmlPreview : UserControl
    {
        // TODO: Move to WebView2 on WinUI 3.0 release

        public HtmlPreview(HtmlPreviewViewModel model)
        {
            ViewModel = model;
            ViewModel.LoadedEvent += ViewModel_LoadedEvent;
            InitializeComponent();
        }

        public HtmlPreviewViewModel ViewModel { get; set; }

        private void ViewModel_LoadedEvent(object sender, EventArgs e)
        {
            WebViewControl.NavigateToString(ViewModel.TextValue);
        }
    }
}