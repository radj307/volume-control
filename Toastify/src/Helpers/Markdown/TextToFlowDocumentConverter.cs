/*
 * All credits go to @theunrepentantgeek (https://github.com/theunrepentantgeek):
 * https://github.com/theunrepentantgeek/Markdown.XAML
 */

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Toastify.Helpers.Markdown
{
    public class TextToFlowDocumentConverter : DependencyObject, IValueConverter
    {
        #region Static Fields and Properties

        public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register("Markdown", typeof(Markdown), typeof(TextToFlowDocumentConverter), new PropertyMetadata(null));

        #endregion

        private readonly Lazy<Markdown> _markdown = new Lazy<Markdown>(() => new Markdown());

        #region Public Properties

        public Markdown Markdown
        {
            get { return (Markdown)this.GetValue(MarkdownProperty); }
            set { this.SetValue(MarkdownProperty, value); }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string text = (string)value;
            Markdown engine = this.Markdown ?? this._markdown.Value;
            return engine.Transform(text);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}