using KurdspellForWord.ViewModels;
using System.Windows.Controls;

namespace KurdspellForWord.Views
{
    /// <summary>
    /// Interaction logic for MistakesList.xaml
    /// </summary>
    public partial class MistakesList : UserControl
    {
        private readonly MistakesListViewModel _viewModel;

        public MistakesList(MistakesListViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        private void ListView_DoubleClicke(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.ShowSelectedItem();
        }

        private void ListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_viewModel.SelectedItem == null) return;
            var contextMenu = listView.ContextMenu;
            contextMenu.Items.Clear();

            foreach (var fix in _viewModel.SelectedItem.Suggestions)
            {
                var item = new MenuItem { Header = fix.Text };
                item.Command = _viewModel.ApplyFixCommand;
                item.CommandParameter = fix;
                contextMenu.Items.Add(item);
            }

            contextMenu.Items.Add(new Separator());

            var addToDictItem = new MenuItem { Header = "Add to dictionary" };
            addToDictItem.Command = _viewModel.AddToDictionaryCommand;
            addToDictItem.CommandParameter = _viewModel.SelectedItem;
            contextMenu.Items.Add(addToDictItem);

            listView.ContextMenu = contextMenu;
        }
    }
}
