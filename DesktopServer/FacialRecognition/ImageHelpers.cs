using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FacialRecognition
{
    public static class ImageHelpers
    {
        public static Size GetImageSize(string imagePath)
        {
            var img = System.Drawing.Image.FromFile(imagePath);
            return new Size(img.Size.Width, img.Size.Height);
        }
        public static Face MapToImageControl(string imagePath, Face originalCoordinates, Size imageControlSize)
        {
            Size imageSize = GetImageSize(imagePath);

            double left = originalCoordinates.Left;
            double top = originalCoordinates.Top;
            double width = originalCoordinates.Width;
            double height = originalCoordinates.Height;

            double ratioX = imageSize.Width / imageControlSize.Width;
            double ratioY = imageSize.Height / imageControlSize.Height;


            left /= ratioX;
            top /= ratioY;
            width /= ratioX;
            height /= ratioY;

            

            return new Face((int)left - 10, (int)top - 10, (int)width, (int)height);
        }
    }
}
