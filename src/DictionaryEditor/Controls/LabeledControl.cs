using System.Windows;
using System.Windows.Controls;

namespace DictionaryEditor.Controls
{
    public class LabeledControl : ContentControl
    {
        static LabeledControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LabeledControl),
                new FrameworkPropertyMetadata(typeof(LabeledControl)));
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabeledControl), new PropertyMetadata(string.Empty));
    }
}
