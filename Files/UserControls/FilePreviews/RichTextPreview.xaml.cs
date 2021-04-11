using Files.ViewModels.Previews;
using Microsoft.UI.Xaml.Controls;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls.FilePreviews
{
    public sealed partial class RichTextPreview : UserControl
    {
        public RichTextPreview(RichTextPreviewViewModel viewModel)
        {
            viewModel.LoadedEvent += ViewModel_LoadedEvent;
            ViewModel = viewModel;
            InitializeComponent();
        }

        public RichTextPreviewViewModel ViewModel { get; set; }

        private void ViewModel_LoadedEvent(object sender, EventArgs e)
        {
            TextPreviewControl.Document.LoadFromStream(Microsoft.UI.Text.TextSetOptions.FormatRtf, ViewModel.Stream);
        }
    }
}