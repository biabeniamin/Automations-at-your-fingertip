using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FacialRecognition
{
    public static class ImageHelpers
    {
        public static Tuple<int, int> GetImageSize(string imagePath)
        {
            var im = new BitmapImage();
            im.BeginInit();
            im.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            im.EndInit();
            return new Tuple<int, int>((int)im.Width, (int)im.Height);
        }
    }
}
