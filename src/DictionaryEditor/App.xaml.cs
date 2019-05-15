using System.ComponentModel;
using System.Windows;

namespace DictionaryEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        public static App This => Application.Current as App;

        private FlowDirection _direction = FlowDirection.RightToLeft;
        public FlowDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == _direction) return;
                _direction = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Direction)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
