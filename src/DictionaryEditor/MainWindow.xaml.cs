using Kurdspell;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                await OpenDictionary(dialog.FileName);
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
            if (App.This.Direction == direction)
                return;

            if (mainContent.FlowDirection == FlowDirection.RightToLeft)
            {
                App.This.Direction = FlowDirection.LeftToRight;
            }
            else
            {
                App.This.Direction = FlowDirection.RightToLeft;
            }
        }

        private async void SaveDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(mainContent.Content is Views.DictionaryEditor editor))
                return;

            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            dialog.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    var vm = editor.DataContext as ViewModels.DictionaryEditorViewModel;

                    var spellChecker = new SpellChecker(
                        vm.GetPatterns(),
                        vm.GetAffixes(),
                        vm.GetProperties());

                    await spellChecker.SaveAsync(dialog.FileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async Task OpenDictionary(string path)
        {
            try
            {
                var spellChecker = await SpellChecker.FromFileAsync(path);
                mainContent.Content = new Views.DictionaryEditor(spellChecker);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                await OpenDictionary(files[0]);
            }
        }
    }
}
