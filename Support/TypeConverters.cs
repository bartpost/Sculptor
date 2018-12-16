using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class ObjectType_IDToImage : IValueConverter
    {
        public object Convert(object ObjectType_ID,
            Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var typeItem in ObjectViewModelLocator.GetObjectVM().ObjectTypes)
            {
                if (typeItem.ID == (int)ObjectType_ID) return typeItem.Image;
            }
            return null;
        }

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class TemplateType_IDToImage : IValueConverter
    {
        public object Convert(object TemplateType_ID,
            Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var typeItem in TemplateViewModelLocator.GetTemplateVM().TemplateTypes)
            {
                if (typeItem.ID == (int)TemplateType_ID) return typeItem.Image;
            }
            return null;
        }

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class PropertyType_IDToImage : IValueConverter
    {
        public object Convert(object PropertyType_ID,
            Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var typeItem in PropertyViewModelLocator.GetPropertyVM().PropertyTypes)
            {
                if (typeItem.ID == (int)PropertyType_ID) return typeItem.Image;
            }
            return null;
        }

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class RequirementType_IDToImage : IValueConverter
    {
        public object Convert(object RequirementType_ID,
            Type targetType, object parameter, CultureInfo culture)
        {

            switch (RequirementType_ID)
            {
                case 1:
                    return "../Resources/Images/RequirementSpecification32.png";
                case 2:
                    return "../Resources/Images/RequirementClarification32.png";
                case 3:
                    return "../Resources/Images/RequirementChange32.png";
                case 4:
                    return "../Resources/Images/Regulation32.png";
                case 5:
                    return "../Resources/Images/Unit32.png";
                case 6:
                    return "../Resources/Images/Equipment32.png";
                case 7:
                    return "../Resources/Images/ControlModule32.png";
                default:
                    return "../Resources/Images/Question32.png";
            }
        }

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class BinaryToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteArrayImage = value as byte[];

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

        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
