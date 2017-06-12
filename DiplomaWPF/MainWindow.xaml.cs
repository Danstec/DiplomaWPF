using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using System.IO;

namespace DiplomaWPF
{
    public static class ItemCollectionExtention
    {
        public static void AddRange(this ItemCollection itemCollection, IEnumerable items)
        {
            foreach (var item in items)
            {
                itemCollection.Add(item);
            }
        }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Bitmap> pictures = new List<Bitmap>();
        List<byte[,,]> bytePictures = new List<byte[,,]>();
        Bitmap bmp;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            fileDialog.Multiselect = true;
            bool? result = fileDialog.ShowDialog();
            if (result.Value)
            {
                pictures.AddRange(fileDialog.FileNames.Select((fileName) => new Bitmap(fileName)));
                listBox.Items.AddRange(fileDialog.FileNames.Select((fileName) => fileName));
            }
        }

        private byte[,,] BitmapToByteRgb(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] result = new byte[3, height, width];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    System.Drawing.Color color = bmp.GetPixel(x, y);
                    result[0, y, x] = color.R;
                    result[1, y, x] = color.G;
                    result[2, y, x] = color.B;
                }
            }
            return result;
        }

        private Bitmap RgbToBitmap(int[,,] rgb)
        {
            //if ((rgb.GetLength(0) != 3))
            //{
            //    throw new ArrayTypeMismatchException("Розмiр першого вимiру для переданого масиву повинен бути 3 (компоненти RGB)");
            //}

            int width = rgb.GetLength(2),
                height = rgb.GetLength(1);

            Bitmap result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    result.SetPixel(x, y, System.Drawing.Color.FromArgb(rgb[0, y, x], rgb[1, y, x], rgb[2, y, x]));
                }
            }

            return result;
        }

        private void Bracket_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < pictures.Count; i++)
            {
                bytePictures.Add(BitmapToByteRgb(pictures[i]));
            }

            int[,,] finalByteBmp = new int[3, pictures[0].Height, pictures[0].Width];

            for (int i = 0; i < pictures[0].Height; i++)
            {
                for (int j = 0; j < pictures[0].Width; j++)
                {
                    foreach (var item in bytePictures)
                    {
                        finalByteBmp[0, i, j] += item[0, i, j];
                        finalByteBmp[1, i, j] += item[1, i, j];
                        finalByteBmp[2, i, j] += item[2, i, j];
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        finalByteBmp[k, i, j] = Convert.ToByte(finalByteBmp[k, i, j] / bytePictures.Capacity);
                    }
                }
            }

            Bitmap finalBmp = RgbToBitmap(finalByteBmp);
            bmp = finalBmp;

            MemoryStream stream = new MemoryStream();

            finalBmp.Save(stream, ImageFormat.Png);

            System.Windows.Media.Imaging.BitmapImage bmpImage = new System.Windows.Media.Imaging.BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = stream;
            bmpImage.EndInit();

            resultImage.Source = bmpImage;            
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bmp.Save(filePath.Text);
            MessageBox.Show("Виконано!");
        }
    }
}