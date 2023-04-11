using System;
using System.Drawing;
using System.Threading.Tasks;

namespace NegativeAlgoritmCS
{
    public class Class1
    {
        public static Bitmap ConvertToNegative(Bitmap originalImage, int threadCount)
        {
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
                            Color pixelColor = originalImage.GetPixel(x, y);

                            int invertedR = 255 - pixelColor.R;
                            int invertedG = 255 - pixelColor.G;
                            int invertedB = 255 - pixelColor.B;

                            Color invertedColor = Color.FromArgb(invertedR, invertedG, invertedB);
                            negativeImage.SetPixel(x, y, invertedColor);
                        }
                    }
                });
            }

            Task.WaitAll(tasks);

            return negativeImage;
        }
    }


}
