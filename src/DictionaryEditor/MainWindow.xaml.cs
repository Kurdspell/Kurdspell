using DictionaryEditor.Helpers;
using DictionaryEditor.Models;
using Kurdspell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DictionaryEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _canSave = false;
        private string _filePath = null;
        private Preferences _preferences;
        private readonly IReadOnlyCollection<MenuItem> _originalFileSubMenues;

        public MainWindow()
        {
            InitializeComponent();

            var list = new List<MenuItem>();
            foreach (MenuItem item in fileMenuItem.Items)
            {
                list.Add(item);
            }
            _originalFileSubMenues = list;

            LoadPreferences();
            Closed += MainWindow_Closed;
            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_isBusy) return;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (e.Key >= Key.D1 && e.Key <= Key.D9))
            {
                var index = e.Key - Key.D1;
                OpenRecentFile(index);
            }
        }

        private async void OpenRecentFile(int index)
        {
            if (_preferences.RecentFiles.Count >= index + 1)
            {
                await OpenDictionary(_preferences.RecentFiles[index]);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            SavePreferences();
        }

        private async void OpenDictionaryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
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

        private void LoadPreferences()
        {
            _preferences = PreferencesHelper.GetPreferences();
            var direction = _preferences.RightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            ToggleDirection(direction);
            ReloadRecentFileMenuItems();
        }

        private void SavePreferences()
        {
            if (_preferences == null) return;

            PreferencesHelper.SavePreferences(_preferences);
        }

        private void RtlDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleDirection(FlowDirection.RightToLeft);
        }

        private void LtrDictionaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleDirection(FlowDirection.LeftToRight);
        }

        private void ToggleDirection(FlowDirection direction)
        {
            if (App.This.Direction == direction)
                return;

            if (mainContent.FlowDirection == FlowDirection.RightToLeft)
            {
                App.This.Direction = FlowDirection.LeftToRight;
                ltrDictionaryMenuItem.IsChecked = true;
                rtlDictionaryMenuItem.IsChecked = false;
            }
            else
            {
                App.This.Direction = FlowDirection.RightToLeft;
                ltrDictionaryMenuItem.IsChecked = false;
                rtlDictionaryMenuItem.IsChecked = true;
            }

            _preferences.RightToLeft = App.This.Direction == FlowDirection.RightToLeft;
        }

        private async void SaveDictionaryAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_canSave == false) return;

            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            dialog.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                await Save(dialog.FileName);
            }
        }

        private async void SaveDictionaryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_canSave)
            {
                await Save(_filePath);
            }
        }

        private async Task Save(string path)
        {
            try
            {
                SetIsBusy(true);
                var editor = mainContent.Content as Views.DictionaryEditor;

                var vm = editor.DataContext as ViewModels.DictionaryEditorViewModel;

                var spellChecker = new SpellChecker(
                    vm.GetPatterns(),
                    vm.GetAffixes(),
                    vm.GetProperties());

                await spellChecker.SaveAsync(path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SetIsBusy(false);
            }
        }

        private async Task OpenDictionary(string path)
        {
            try
            {
                SetIsBusy(true);
                var spellChecker = await SpellChecker.FromFileAsync(path);
                mainContent.Content = new Views.DictionaryEditor(spellChecker);
                _canSave = true;
                _filePath = path;
                _preferences.AddPathToRecentFiles(path);
                ReloadRecentFileMenuItems();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SetIsBusy(false);
            }
        }

        private void ReloadRecentFileMenuItems()
        {
            fileMenuItem.Items.Clear();
            foreach (var item in _originalFileSubMenues)
            {
                fileMenuItem.Items.Add(item);
            }

            if (_preferences.RecentFiles.Any())
            {
                fileMenuItem.Items.Add(new Separator());

                int number = 1;
                foreach (var file in _preferences.RecentFiles)
                {
                    const int limit = 25;
                    var path = file;
                    if (file.Length > limit)
                    {
                        var startIndex = file.Length - limit - 1;
                        path = "..." + path.Substring(startIndex);
                    }

                    var item = new MenuItem
                    {
                        Header = $"Open '{path}'",
                        InputGestureText = $"Ctrl+{number}",
                        ToolTip = file,
                        Tag = number - 1,
                    };

                    item.Click += RecentFileMenuItemClick;

                    fileMenuItem.Items.Add(item);
                    number++;
                }
            }
        }

        private void RecentFileMenuItemClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var index = (int)item.Tag;
            OpenRecentFile(index);
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                await OpenDictionary(files[0]);
            }
        }

        private bool _isBusy = false;
        void SetIsBusy(bool busy)
        {
            _isBusy = busy;
            progressBorder.Visibility = _isBusy ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
