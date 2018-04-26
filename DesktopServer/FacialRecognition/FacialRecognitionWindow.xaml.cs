using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
    public partial class FacialRecognitionWindow : Window,INotifyPropertyChanged
    {
        private ObservableCollection<Face> _faces;
        private FacialRecognitionApi _facialRecognitionApi;
        private Action<int> _actionFaceDetected;


        public ObservableCollection<Face> Faces
        {
            get { return _faces; }
            set
            {
                _faces = value;
                OnPropertyChanged("Faces");

            }
        }

        public FacialRecognitionWindow(Action<int> actionFaceDetected)
        {
            DataContext = this;
            InitializeComponent();
            Faces = new ObservableCollection<Face>();
            _facialRecognitionApi = new FacialRecognitionApi();
            _actionFaceDetected = actionFaceDetected;
        }

        private async void RunFacialRecognition()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.EndInit();

                drawingContext.DrawImage(bitmap, new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(bitmap.Width, bitmap.Height)));

                List<Person> people = await _facialRecognitionApi.DoesExists(dlg.FileName);
                foreach (Person person in people)
                {
                    var mar = image.Margin;
                    //Graphics g = Graphics.FromImage()
                    drawingContext.DrawRectangle(System.Windows.Media.Brushes.Transparent, new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 6),
                        new Rect(person.Face.Left, person.Face.Top, person.Face.Width, person.Face.Height));

                    if (null != person.PersonA)
                    {
                        if (person.PersonA.Name.Equals("Ben"))
                        {
                            _actionFaceDetected(1);
                        }

                        else
                        {
                            _actionFaceDetected(3);
                        }
                    }
                    else
                    {
                        _actionFaceDetected(3);
                    }
                }
                drawingContext.Close();

                RenderTargetBitmap renderTarget = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Pbgra32);
                renderTarget.Render(drawingVisual);
                image.Source = renderTarget;
            }
            GC.Collect();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //Faces.Add(ImageHelpers.MapToImageControl(@"D:\Beni\DSC05280.JPG", new Face(950, 300, 60, 60), image.RenderSize));

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

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(personName.Text))
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".jpg";
                dlg.Filter = "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
                var result = dlg.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    _facialRecognitionApi.AddFace(dlg.FileName, personName.Text);
                }
            }
            else
            {
                MessageBox.Show("Please enter a name for person");
            }
        }
    }
}
