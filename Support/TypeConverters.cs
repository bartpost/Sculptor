using Sculptor.Properties;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class Type_IDToImage : IValueConverter
    {
        public object Convert(object Type_ID, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var typeItem in TypeViewModelLocator.GetTypeVM().Types)
            {
                if (typeItem.ID == (int)Type_ID) return typeItem.Image;
            }
            Image NewImage = Resources.NewItem;
            return NewImage;
        }

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class BinaryToImage : IValueConverter
    {
        public object Convert(object binaryImage, Type targetType, object parameter, CultureInfo culture)
        {
            var byteArrayImage = binaryImage as byte[];

            if (byteArrayImage != null && byteArrayImage.Length > 0)
            {
                var ms = new MemoryStream(byteArrayImage);

                var bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = ms;
                bitmapImg.EndInit();
                return bitmapImg;
            }

            return null;
        }

        public object ConvertBack(object image, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmapImage = image as Bitmap;

            if (bitmapImage != null)
            {
                System.Drawing.ImageConverter _imageConverter = new System.Drawing.ImageConverter();
                byte[] byteArrayImage = (byte[])_imageConverter.ConvertTo(bitmapImage, typeof(byte[]));
                return byteArrayImage;
            }
            return null;
        }
    }
}
