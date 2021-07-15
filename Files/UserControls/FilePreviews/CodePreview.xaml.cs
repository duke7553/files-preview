﻿using ColorCode;
using Files.ViewModels.Previews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls.FilePreviews
{
    public sealed partial class CodePreview : UserControl
    {
        private RichTextBlockFormatter formatter;
        private bool rendered;

        CodePreviewViewModel ViewModel { get; set; }
        public CodePreview(CodePreviewViewModel model)
        {
            ViewModel = model;
            this.InitializeComponent();
        }

        private void RenderDocument()
        {
            if (codeView != null)
            {
                codeView.Blocks?.Clear();
                formatter = new RichTextBlockFormatter(ActualTheme);

                formatter.FormatRichTextBlock(ViewModel.TextValue, ViewModel.CodeLanguage, codeView);
                rendered = true;
            }
        }

        private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RenderDocument();
        }

        private void UserControl_ActualThemeChanged(Microsoft.UI.Xaml.FrameworkElement sender, object args)
        {
            try
            {
                rendered = false;
                RenderDocument();
            }
            catch (Exception)
            {
            }
        }
    }
}
