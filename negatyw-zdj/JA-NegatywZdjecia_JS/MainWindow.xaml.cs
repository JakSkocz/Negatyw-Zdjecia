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
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using NegativeAlgoritmCS;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace JA_NegatywZdjecia_JS
{
    public partial class MainWindow : Window
    {
        [DllImport(@"C:\Users\kubas\Desktop\JA-proj\JA-NegatywZdjecia_JS\x64\Debug\JaAsm.dll", CallingConvention=CallingConvention.Cdecl)]
        unsafe static extern void MyProc1(byte[] bytes, byte[] bytes1);


        private Bitmap originalImage;
        private Bitmap negativeImage;
        private int threadCount = 1;

        public MainWindow()
        {
            InitializeComponent();
            int processorCount = Environment.ProcessorCount;
            slider.Value = processorCount;
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            threadCount = (int)slider.Value;
        }


        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                originalPictureBox.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    originalImage.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                if (CS.IsChecked == true)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    negativeImage = Class1.ConvertToNegative(originalImage, threadCount);

                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;

                    negativePictureBox.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    negativeImage.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                    timeLabel.Text = +(int)Math.Round(elapsedTime.TotalMilliseconds) + " ms";
                }
                else if(Asm.IsChecked == true)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Bitmap negativeImage = new Bitmap(originalImage.Width, originalImage.Height);

                    int heightPerThread = originalImage.Height / threadCount;

                    Task[] tasks = new Task[threadCount];

                    for (int i = 0; i < threadCount; i++)
                    {
                        int start = i * heightPerThread;
                        int end = (i + 1) * heightPerThread;

                        if (i == threadCount - 1)
                        {
                            end = originalImage.Height;
                        }

                        int threadIndex = i;

                        tasks[threadIndex] = Task.Run(() =>
                        {
                            for (int x = 0; x < originalImage.Width; x++)
                            {
                                for (int y = start; y < end; y++)
                                {
                                    
                                    System.Drawing.Color pixelColor = originalImage.GetPixel(x, y);

                                    byte[] newtable = new byte[] { pixelColor.R, pixelColor.G, pixelColor.B };
                                    byte[] newtable2 = new byte[] { 255, 255, 255 };
                                    MyProc1(newtable, newtable2);
                                    System.Drawing.Color invertedColor = System.Drawing.Color.FromArgb(newtable[0], newtable[1], newtable[2]);
                                    originalImage.SetPixel(x, y, invertedColor);
                                    
                                }
                            }
                        });
                    }

                    Task.WaitAll(tasks);
                    negativePictureBox.Source = ConvertBitmap(originalImage);
                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    timeLabel.Text = +(int)Math.Round(elapsedTime.TotalMilliseconds) + " ms";
                }

            }
        }

        public BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }




        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        negativeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        negativeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 3:
                        negativeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
                fs.Close();
            }
        }

        private void RadioButtonCSharp(object sender, RoutedEventArgs e)
        {
            
        }

        private void RadioButtonAsm(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
        