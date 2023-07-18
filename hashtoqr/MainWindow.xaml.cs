using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Security.Cryptography;
using System.IO;
using QRCoder;
using System.Drawing;

namespace hashtoqr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private bool isMD5 = true;
        public MainWindow()
        {
            InitializeComponent();
        }
        private String getMD5File(string filepath)
        {
            if (filepath == null)
            {
                return "";
            }
            if (!File.Exists(filepath))
            {
                return "";
            }
            using (var md5 = MD5.Create())
            {
                using (var sr = File.OpenRead(filepath))
                {
                    var hash = md5.ComputeHash(sr);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private String GetSHA1File(string filepath)
        {
            if (filepath == null)
            {
                return "";
            }
            if (!File.Exists(filepath))
            {
                return "";
            }
            using (var sha1 = SHA1.Create())
            {
                using (var sr = File.OpenRead(filepath))
                {
                    var hash = sha1.ComputeHash(sr);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private void btn_browse_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if (openFileDlg.ShowDialog() == true)
            {
                txt_file_path.Text = openFileDlg.FileName;
            }
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        private void GenQR(string filepath)
        {
            string hash = "";
            if (isMD5 == true)
            {
                hash = getMD5File(filepath);
            }
            else
            {
                hash = GetSHA1File(filepath);
            }
            if (hash == "")
            {
                MessageBox.Show("Cannot get hash");
                return;
            }
            //MessageBox.Show(hash);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            String searchString = String.Format("https://www.virustotal.com/gui/search/{0}", hash);
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(searchString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap bitmap = qrCode.GetGraphic(20);
            qr_image.Source = BitmapToImageSource(bitmap);
        }
        private void txt_file_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_file_path.Text != "")
            {
                GenQR(txt_file_path.Text);
            }    
        }

        private void radio_md5_Checked(object sender, RoutedEventArgs e)
        {
            isMD5 = radio_md5.IsChecked == true;
            if (txt_file_path.Text != "")
            {
                GenQR(txt_file_path.Text);
            }
        }

        private void radio_sha1_Checked(object sender, RoutedEventArgs e)
        {
            isMD5 = radio_md5.IsChecked == true;
            if (txt_file_path.Text != "")
            {
                GenQR(txt_file_path.Text);
            }    
        }
    }
    
}
