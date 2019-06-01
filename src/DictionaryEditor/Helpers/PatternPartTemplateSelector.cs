using DictionaryEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DictionaryEditor.Helpers
{
    public class PatternPartTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Affix { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PatternPartViewModel part && part.IsAffix)
            {
                return Affix;
            }

            return Normal;
        }
    }
}
