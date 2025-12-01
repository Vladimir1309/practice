using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Practice
{
    public class ImagePathToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    // Создаем Uri из строки
                    Uri uri;
                    if (imagePath.StartsWith("/"))
                    {
                        // Для относительных путей
                        uri = new Uri($"pack://application:,,,{imagePath}", UriKind.Absolute);
                    }
                    else
                    {
                        uri = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    }

                    return new ImageBrush(new BitmapImage(uri));
                }
                catch (Exception)
                {
                    return Brushes.LightGray;
                }
            }
            return Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}