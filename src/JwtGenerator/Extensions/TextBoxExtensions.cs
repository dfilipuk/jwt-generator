using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace JwtGenerator.Extensions
{
    public static class TextBoxExtensions
    {
        public static bool ValidateText(this TextBox textBox)
        {
            var result = true;
            var brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            if (String.IsNullOrEmpty(textBox.Text))
            {
                result = false;
                brush = new SolidColorBrush(Color.FromArgb(32, 255, 0, 0));
            }

            textBox.Background = brush;
            return result;
        }
    }
}
