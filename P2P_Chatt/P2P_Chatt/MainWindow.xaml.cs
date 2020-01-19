using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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

namespace P2P_Chatt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = vm;
           
        }

        public void sub_stuff(object sender, RoutedEventArgs e)
        {
            string whodis = sender.ToString().Split(' ')[1];
            vm.DoAFlip(whodis);
        }

        public void get_mes(object sender, RoutedEventArgs e)
        {
            string whodis = sender.ToString().Split(' ')[1];
            vm.fetch_mess(whodis);
        }


        public void Sound(object sender, RoutedEventArgs e)
        {
            string audio = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Ring.wav";
            Console.WriteLine(audio);
            SoundPlayer snd = new SoundPlayer(audio);
            snd.Play();
        }
        
        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png;)|*.jpg; *.jpeg; *.gif; *.bmp; *.png;";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                byte[] bytes = File.ReadAllBytes(filename);
                vm.SendImg(bytes);
                /*
                System.Uri uri = new System.Uri(filename);
                BitmapImage bm = new BitmapImage(uri);
                pic.Source = bm;
                PngBitmapEncoder pbe = new PngBitmapEncoder();
                byte[] streaming = BufferFromImage(bm);
                Console.WriteLine(streaming);
                //Console.WriteLine("Done");
                vm.SendImg(streaming);
                */

            }
        }

        public Byte[] BufferFromImage(BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            Console.WriteLine(stream.Length);
            if (stream != null && stream.Length > 0)
            {

                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }
            Console.WriteLine(buffer);

            return buffer;
        }
    }
}
