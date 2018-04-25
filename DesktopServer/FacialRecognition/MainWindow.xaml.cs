using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacialRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Face> _faces;



        public ObservableCollection<Face> Faces
        {
            get { return _faces; }
            set
            {
                _faces = value;
                OnPropertyChanged("Faces");

            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            image.Source = new BitmapImage(new Uri(@"C:\Users\biabe\Pictures\Camera Roll\WIN_20180425_16_24_50_Pro.jpg"));
            Faces = new ObservableCollection<Face>();
            Faces.Add(new Face(0, 0, 50, 50));
        }

        private async void RunFacialRecognition()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var facial = new FacialRecognitionApi();
                List<Face> faces = await facial.DoesExists(dlg.FileName);
                foreach (Face face in faces)
                {
                    int left = face.Left;
                    int top = face.Top;
                    int width = face.Width;
                    int height = face.Height;
                    Faces.Add(new Face(left, top, width, height));
                }
                image.Source = new BitmapImage(new Uri(dlg.FileName));
            }
            GC.Collect();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RunFacialRecognition();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
