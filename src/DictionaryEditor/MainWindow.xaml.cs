using DictionaryEditor.Views;
using Kurdspell;
using System.Windows;

namespace DictionaryEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            var result = dialog.ShowDialog();

            if (result == true)
            {
                var spellchecker = await SpellChecker.FromFileAsync(dialog.FileName);
                mainContent.Content = new DictionaryBrowser(spellchecker);
            }
        }

        private void RtlDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleDirection(FlowDirection.RightToLeft);
            ltrDictionaryMenuItem.IsChecked = false;
            rtlDictionaryMenuItem.IsChecked = true;
        }

        private void LtrDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            rtlDictionaryMenuItem.IsChecked = false;
            ltrDictionaryMenuItem.IsChecked = true;
            ToggleDirection(FlowDirection.LeftToRight);
        }

        private void ToggleDirection(FlowDirection direction)
        {
            if (mainContent.FlowDirection == direction)
                return;

            if (mainContent.FlowDirection == FlowDirection.RightToLeft)
            {
                mainContent.FlowDirection = FlowDirection.LeftToRight;
            }
            else
            {
                mainContent.FlowDirection = FlowDirection.RightToLeft;
            }
        }
    }
}
