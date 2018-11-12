using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NHunspell;

namespace HunspellTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Hunspell _hunspell;
        private string _currentAffixFilePath;
        private string _currentDictionaryFilePath;
        private FileSystemWatcher _dictionaryWatcher = new FileSystemWatcher();
        private FileSystemWatcher _affixWatcher = new FileSystemWatcher();

        public MainWindow()
        {
            InitializeComponent();

            _dictionaryWatcher.Changed += DictionaryFile_Changed;
            _affixWatcher.Changed += DictionaryFile_Changed;
            _dictionaryWatcher.NotifyFilter = _affixWatcher.NotifyFilter = NotifyFilters.LastWrite;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                var settings = new Settings
                {
                    AffixFilePath = AffixFileTextBox.Text,
                    DictionaryFilePath = DictionaryFileTextBox.Text,
                };

                SettingsHelper.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = SettingsHelper.GetSettings();
                if (settings == null)
                    return;

                AffixFileTextBox.Text = settings.AffixFilePath;
                DictionaryFileTextBox.Text = settings.DictionaryFilePath;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var dialog = new OpenFileDialog();
            var textBox = button.Tag as TextBox;
            dialog.CheckFileExists = true;
            dialog.Filter = textBox.Tag as string;
            dialog.Filter += "|All Files|*.*";

            if (dialog.ShowDialog() == true)
            {
                textBox.Text = dialog.FileName;
            }
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Suggest();
        }

        private void Suggest()
        {
            try
            {
                if (_hunspell == null ||
                _currentDictionaryFilePath != DictionaryFileTextBox.Text ||
                _currentAffixFilePath != AffixFileTextBox.Text)
                {
                    if (!TryLoadDictionary())
                    {
                        return;
                    }
                    else
                    {
                        _dictionaryWatcher.Path = System.IO.Path.GetDirectoryName(_currentDictionaryFilePath);
                        _affixWatcher.Path = System.IO.Path.GetDirectoryName(_currentAffixFilePath);

                        _dictionaryWatcher.Filter = System.IO.Path.GetFileName(_currentDictionaryFilePath);
                        _affixWatcher.Filter = System.IO.Path.GetFileName(_currentAffixFilePath);

                        _dictionaryWatcher.EnableRaisingEvents = true;
                        _affixWatcher.EnableRaisingEvents = true;
                    }
                }

                var watch = new Stopwatch();
                watch.Start();
                var suggestions = _hunspell.Suggest(InputTextBox.Text);
                SuggestionsListView.ItemsSource = suggestions;
                watch.Stop();

                statusTextBlock.Text = $"Done. {suggestions.Count:N0} suggestions in {watch.ElapsedMilliseconds:N0} ms.";

            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void DictionaryFile_Changed(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (TryLoadDictionary(silent: true))
                {
                    Suggest();
                }
            });
        }

        private bool TryLoadDictionary(bool silent = false)
        {
            var message = string.Empty;

            if (string.IsNullOrWhiteSpace(AffixFileTextBox.Text))
                message = $"Please write the affix file path.";

            if (string.IsNullOrWhiteSpace(DictionaryFileTextBox.Text))
                message = $"Please write the diction file path.";

            if (!File.Exists(AffixFileTextBox.Text))
                message = $"Invalid affix file path.";

            if (!File.Exists(DictionaryFileTextBox.Text))
                message = "Invalid dictionary file path.";

            if (!string.IsNullOrEmpty(message))
            {
                if (!silent)
                    MessageBox.Show(message);
                return false;
            }

            ProgressGrid.Visibility = Visibility.Visible;

            _hunspell?.Dispose();
            _hunspell = new Hunspell(AffixFileTextBox.Text.Trim(), DictionaryFileTextBox.Text.Trim());

            _currentAffixFilePath = AffixFileTextBox.Text;
            _currentDictionaryFilePath = DictionaryFileTextBox.Text;

            ProgressGrid.Visibility = Visibility.Collapsed;

            return true;
        }

        private void Log(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            MessageBox.Show(ex.Message);
        }
    }
}
