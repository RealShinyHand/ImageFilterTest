using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ComputerVision.model
{

    class Mask
    {
        public int Lenght { get; set; }

        byte[,] maskelemnet;

        public Mask(int lenght)
        {
            maskelemnet = new byte[lenght, lenght];
            Lenght = lenght;
        }


    }

    interface IImageFilter
    {
        void FilterProcess(byte[,] target, Mask mask, byte[,] imageArray2, int bytesPerPixel);


    }

    class MedianFilter : IImageFilter
    {
        List<Byte> tempList;

        public void FilterProcess(byte[,] target, Mask mask, byte[,] result, int bytesPerPixel)
        {

            tempList = new List<byte>(mask.Lenght * mask.Lenght);

            Func<byte[,], int, int, int, int, int> avefunc = null;

            if (mask.Lenght == 3)
            {

                avefunc = MedianMask3;
            }
            else if (mask.Lenght == 5)
            {
                avefunc = MedianMask5;
            }
            else if (mask.Lenght == 7)
            {
                avefunc = MedianMask7;
            }

            int maskSize = mask.Lenght / 2; //픽셀 거리 단위
            int bytedis = bytesPerPixel * maskSize; //픽셀 rgb 거리 단위

            for (int i = maskSize; i < target.GetLength(0) - maskSize; i++)
            {
                for (int j = bytedis; j < target.GetLength(1) - bytedis; j++)
                {
                    result[i, j] = (byte)avefunc(target, maskSize, i, j, bytesPerPixel);
                    tempList.Clear();
                }
            }
        }

        private int MedianMask7(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            for (int i = -3; i <= 3; i++)
                for (int j = -(bytesPerPixel * 3); j <= bytesPerPixel * 3; j += bytesPerPixel)
                    tempList.Add(arry[x - i, y - j]);

            tempList.Sort();
            return tempList[24];
        }

        private int MedianMask5(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {

            for (int i = -2; i <= 2; i++)
                for (int j = -(bytesPerPixel * 2); j <= bytesPerPixel * 2; j += bytesPerPixel)
                    tempList.Add(arry[x - i, y - j]);

            tempList.Sort();
            return tempList[12];
        }

        private int MedianMask3(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {

            int distance = (masksize / 2) * bytesPerPixel;

            tempList.Add(arry[x, y]);//1,1

            tempList.Add(arry[x - masksize, y - bytesPerPixel]);
            tempList.Add(arry[x - masksize, y]);
            tempList.Add(arry[x - masksize, y + bytesPerPixel]);

            tempList.Add(arry[x, y - bytesPerPixel]);
            tempList.Add(arry[x, y + bytesPerPixel]);

            tempList.Add(arry[x + masksize, y - bytesPerPixel]);
            tempList.Add(arry[x + masksize, y]);
            tempList.Add(arry[x + masksize, y + bytesPerPixel]);

            tempList.Sort();

            return tempList[4];
        }
    }
    class MeanFilter : IImageFilter
    {
        public void FilterProcess(byte[,] target, Mask mask, byte[,] result, int bytesPerPixel)
        {


            Func<byte[,], int, int, int, int, int> avefunc = null;

            if (mask.Lenght == 3)
            {

                avefunc = MeanMask3;
            }
            else if (mask.Lenght == 5)
            {
                avefunc = MeanMask5;
            }
            else if (mask.Lenght == 7)
            {
                avefunc = MeanMask7;
            }

            int maskSize = mask.Lenght / 2; //픽셀 거리 단위
            int bytedis = bytesPerPixel * maskSize; //픽셀 rgb 거리 단위

            for (int i = maskSize; i < target.GetLength(0) - maskSize; i++)
            {
                for (int j = bytedis; j < target.GetLength(1) - bytedis; j++)
                {
                    result[i, j] = (byte)avefunc(target, maskSize, i, j, bytesPerPixel);
                }
            }


        }

        private int MeanMask3(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8

            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            sum = arry[x, y] / 9.0;//1,1

            sum += arry[x - masksize, y - bytesPerPixel] / 9.0; //0,0
            sum += arry[x - masksize, y] / 9.0;//0,1
            sum += arry[x - masksize, y + bytesPerPixel] / 9.0;//0,2

            sum += arry[x, y - bytesPerPixel] / 9.0;
            sum += arry[x, y + bytesPerPixel] / 9.0;

            sum += arry[x + masksize, y - bytesPerPixel] / 9.0;
            sum += arry[x + masksize, y] / 9.0;
            sum += arry[x + masksize, y + bytesPerPixel] / 9.0;

            return (int)Math.Round(sum);
        }
        private int MeanMask5(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8

            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            //sum = arry[x, y] / 25.0;//2,2

            //sum += arry[x - masksize, y - bytesPerPixel*2] / 25.0; //0,0
            //sum += arry[x - masksize, y - bytesPerPixel] / 25.0; //0,1
            //sum += arry[x - masksize, y] / 25.0; //0,2
            //sum += arry[x - masksize, y + bytesPerPixel] / 25.0;//0,3
            //sum += arry[x - masksize, y + bytesPerPixel * 2] / 25.0;//0,4

            //sum += arry[x - 1, y - bytesPerPixel * 2] / 25.0; //1,0
            //sum += arry[x - 1, y - bytesPerPixel] / 25.0; //1,1
            //sum += arry[x - 1, y] / 25.0; //1,2
            //sum += arry[x - 1, y + bytesPerPixel] / 25.0;//1,3
            //sum += arry[x - 1, y + bytesPerPixel * 2] / 25.0;//1,4

            for (int i = -2; i <= 2; i++)
                for (int j = -(bytesPerPixel * 2); j <= bytesPerPixel * 2; j += bytesPerPixel)
                    sum += arry[x - i, y - j] / 25.0;

            return (int)Math.Round(sum);
        }
        private int MeanMask7(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8

            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            for (int i = -3; i <= 3; i++)
                for (int j = -(bytesPerPixel * 3); j <= bytesPerPixel * 3; j += bytesPerPixel)
                    sum += arry[x - i, y - j] / 49.0;

            return (int)Math.Round(sum);
        }
    }
    class LafilasianFilter : IImageFilter
    {
        public void FilterProcess(byte[,] target, Mask mask, byte[,] result, int bytesPerPixel)
        {
            int maskSize = mask.Lenght / 2; //픽셀 거리 단위
            int bytedis = bytesPerPixel * maskSize; //픽셀 rgb 거리 단위

            for (int i = maskSize; i < target.GetLength(0) - maskSize; i++)
            {
                for (int j = bytedis; j < target.GetLength(1) - bytedis; j++)
                {
                    result[i, j] = (byte)Laflasian3x3(target, maskSize, i, j, bytesPerPixel);
                }
            }
        }

        private int Laflasian3x3(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8

            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            sum = arry[x, y] * -4;//1,1

            //sum += arry[x - masksize, y - bytesPerPixel] / 9.0; //0,0
            sum += arry[x - masksize, y] ;//0,1
           // sum += arry[x - masksize, y + bytesPerPixel] / 9.0;//0,2

            sum += arry[x, y - bytesPerPixel] ;
            sum += arry[x, y + bytesPerPixel] ;

            //sum += arry[x + masksize, y - bytesPerPixel] / 9.0;
            sum += arry[x + masksize, y] ;
            //sum += arry[x + masksize, y + bytesPerPixel] / 9.0;

            if(sum < 0)
            {
                sum = 0;
            }else if(sum > 255)
            {
                sum = 255;
            }
            return (int)Math.Round(sum);
        }
    }

    class BigLafilasianFilter : IImageFilter
    {
        public void FilterProcess(byte[,] target, Mask mask, byte[,] result, int bytesPerPixel)
        {
            int maskSize = mask.Lenght / 2; //픽셀 거리 단위
            int bytedis = bytesPerPixel * maskSize; //픽셀 rgb 거리 단위

            for (int i = maskSize; i < target.GetLength(0) - maskSize; i++)
            {
                for (int j = bytedis; j < target.GetLength(1) - bytedis; j++)
                {
                    result[i, j] = (byte)Laflasian5x5(target, maskSize, i, j, bytesPerPixel);
                }
            }
        }

        private int Laflasian5x5(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8
            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            int[,] maskWeight =
            {
                { 0, 0, 1,0, 0     },
                { 0,1, 2,1,0   },
                {1,2,-16,2,1  },
                {0, 1, 2,1,0    },
                {0, 0, 1, 0, 0      },
            };
            //sum = arry[x, y] / 25.0;//2,2

            //sum += arry[x - masksize, y - bytesPerPixel*2] / 25.0; //0,0
            //sum += arry[x - masksize, y - bytesPerPixel] / 25.0; //0,1
            //sum += arry[x - masksize, y] / 25.0; //0,2
            //sum += arry[x - masksize, y + bytesPerPixel] / 25.0;//0,3
            //sum += arry[x - masksize, y + bytesPerPixel * 2] / 25.0;//0,4

            //sum += arry[x - 1, y - bytesPerPixel * 2] / 25.0; //1,0
            //sum += arry[x - 1, y - bytesPerPixel] / 25.0; //1,1
            //sum += arry[x - 1, y] / 25.0; //1,2
            //sum += arry[x - 1, y + bytesPerPixel] / 25.0;//1,3
            //sum += arry[x - 1, y + bytesPerPixel * 2] / 25.0;//1,4

            for (int i = -2; i <= 2; i++)
                for (int j = -(bytesPerPixel * 2),k =0; j <= bytesPerPixel * 2; j += bytesPerPixel,k++)
                    sum += arry[x - i, y - j] * maskWeight[i+2,k];




            if (sum < 0)
            {
                sum = 0;
            }
            else if (sum > 255)
            {
                sum = 255;
            }


            return (int)Math.Round(sum);
        }
    }

    class EnhanceLafilasianFilter : IImageFilter
    {
        public void FilterProcess(byte[,] target, Mask mask, byte[,] result, int bytesPerPixel)
        {
            int maskSize = mask.Lenght / 2; //픽셀 거리 단위
            int bytedis = bytesPerPixel * maskSize; //픽셀 rgb 거리 단위

            for (int i = maskSize; i < target.GetLength(0) - maskSize; i++)
            {
                for (int j = bytedis; j < target.GetLength(1) - bytedis; j++)
                {
                    result[i, j] = (byte)Laflasian3x3(target, maskSize, i, j, bytesPerPixel);
                }
            }
        }

        private int Laflasian3x3(byte[,] arry, int masksize, int x, int y, int bytesPerPixel)
        {
            //1,4 픽셀이 처음온다. 그렇다면 0,0 0,4 0,8 1,0 1,4 1,8 2,0 2,4 2,8

            double sum = 0;
            int distance = (masksize / 2) * bytesPerPixel;

            sum = arry[x, y] * 5;//1,1

            //sum += arry[x - masksize, y - bytesPerPixel] / 9.0; //0,0
            sum -= arry[x - masksize, y];//0,1
                                         // sum += arry[x - masksize, y + bytesPerPixel] / 9.0;//0,2

            sum -= arry[x, y - bytesPerPixel];
            sum -= arry[x, y + bytesPerPixel];

            //sum += arry[x + masksize, y - bytesPerPixel] / 9.0;
            sum -= arry[x + masksize, y];
            //sum += arry[x + masksize, y + bytesPerPixel] / 9.0;

            if (sum < 0)
            {
                sum = 0;
            }
            else if (sum > 255)
            {
                sum = 255;
            }
            return (int)Math.Round(sum);
        }
    }
    interface IWrapArray<T>
    {
        T[,] WrapArray(T[,] array_2_demension, int side, int bytesPerPixel);
        T[,] UnrapArray(T[,] array_2_demension, int side, int bytesPerPixel);
    }


    class WrapArray<T> : IWrapArray<T> where T : struct
    {
        public T[,] UnrapArray(T[,] array_2_demension, int side, int bytesPerPixel)
        {
            int width = array_2_demension.GetLength(1);
            int height = array_2_demension.GetLength(0);

            int realside = side / 2;
            side = (side / 2) * bytesPerPixel;

            T[,] unwrappedArray = new T[height - (side * 2), width - (side * 2)];

            for (int i = 0; i < unwrappedArray.GetLength(0); i++)
                for (int j = 0; j < unwrappedArray.GetLength(1); j++)
                {
                    unwrappedArray[i, j] = array_2_demension[i + realside, j + side];
                }

            return unwrappedArray;
        }

        T[,] IWrapArray<T>.WrapArray(T[,] array_2_demension, int side, int bytesPerPixel)
        {
            int width = array_2_demension.GetLength(1);
            int height = array_2_demension.GetLength(0);

            int realside = side / 2;
            side = (side / 2) * bytesPerPixel;

            T[,] wrappedArray = new T[height + (side * 2), width + (side * 2)];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    wrappedArray[i + realside, j + side] = array_2_demension[i, j];
                }
            }

            return wrappedArray;
        }
    }

    class ImageProcessing
    {
        public IImageFilter Filtering { get; set; }
        IWrapArray<byte> wrapArray = new WrapArray<byte>();

        public BitmapSource ImageProcess(BitmapSource targetImage, int sizeMaskWhat)
        {

            int width = targetImage.PixelWidth;
            int height = targetImage.PixelHeight;
            var bitFormat = targetImage.Format;

            BitmapSource bitmapSource = new FormatConvertedBitmap(targetImage.Clone(), bitFormat, null, 0);  //복사본 생성, 

            WriteableBitmap tinted = new WriteableBitmap(bitmapSource);  //비트맵 변경 가능으로 수정

            int bytesPerPixel = ((tinted.Format.BitsPerPixel + 7) / 8); //현재 시험하는 이미지는 전부 1바이트였다.
            int stride = tinted.PixelWidth * bytesPerPixel;     //한 행에 필요한 크기를 구함./
            int arrayLength = stride * tinted.PixelHeight;      //전체 크기
            byte[] tintedImage = new byte[arrayLength];         //비트맵을 복사할 배열 생성

            byte[] originalImage = new byte[arrayLength];
            bitmapSource.CopyPixels(originalImage, stride, 0);  //비트맵을 배열에 복사.

            var k = originalImage[0];
            byte[,] ImageArray2 = new byte[tinted.PixelHeight, tinted.PixelWidth * bytesPerPixel];

            for (int i = 0, off = 0; i < tinted.PixelHeight; i++) // 비트형 배열을  2차원 비트형 배열로 변환.
            {
                for (int j = 0; j < tinted.PixelWidth * bytesPerPixel; j++, off++)
                {
                    ImageArray2[i, j] = originalImage[off];
                }
            }

            var ImageArray3 = wrapArray.WrapArray(ImageArray2, sizeMaskWhat, bytesPerPixel); // 2번쨰 매개변수 만큼 감싸기/

            var result = new byte[ImageArray3.GetLength(0), ImageArray3.GetLength(1)];

            // Filtering.ImagePreprocessing(ImageArray3,new Mask(3), ImageArray2, bytesPerPixel);

            if (sizeMaskWhat == 3)
            {
                Filtering.FilterProcess(ImageArray3, new Mask(3), result, bytesPerPixel);
            }
            else if (sizeMaskWhat == 5)
            {
                Filtering.FilterProcess(ImageArray3, new Mask(5), result, bytesPerPixel);
            }
            else if (sizeMaskWhat == 7)
            {
                Filtering.FilterProcess(ImageArray3, new Mask(7), result, bytesPerPixel);
            }

            ImageArray2 = wrapArray.UnrapArray(result, sizeMaskWhat, bytesPerPixel);


            ////using (StreamWriter outputFile = new StreamWriter(@"C:\Users\start\Desktop\New_TEXT_File.txt"))
            ////{
            ////    StringBuilder strb = new StringBuilder();
            ////    for (int i = 0; i < tinted.PixelHeight; i++) {
            ////        strb.Clear();
            ////        for (int j = 0;j< 1024;j++)
            ////        {
            ////            if(newImage[i, j] == 255)
            ////            {
            ////                strb.Append("0");
            ////            }
            ////            else
            ////            {
            ////                strb.Append("8");
            ////            }

            ////        }
            ////        outputFile.WriteLine(strb.ToString());
            ////    }



            var kkkkk = new byte[ImageArray2.GetLength(0) * ImageArray2.GetLength(1)];

            for (int i = 0, off = 0; i < ImageArray2.GetLength(0); i++)  //2차원 비트형 배열을 1차원으로 전환.
            {
                for (int j = 0; j < ImageArray2.GetLength(1); j++, off++)
                {
                    kkkkk[off] = ImageArray2[i, j];
                }
            }

            //BitmapImage bitmapImage = new BitmapImage();

            //using (var stream = new MemoryStream(kkkkk))
            //{

            //    bitmapImage.BeginInit();
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.StreamSource = stream;
            //    bitmapImage.EndInit();

            //}
            //return bitmapImage;


            var dpiX = 96d;
            var dpiY = 96d;
            var pixelFormat = bitFormat;
            var bytesPerPixel2 = (pixelFormat.BitsPerPixel + 7) / 8;
            var stride2 = bytesPerPixel2 * width;

            var bitmap = BitmapSource.Create(width, height, dpiX, dpiY,
                                             pixelFormat, null, kkkkk, stride2);  //수정된 비트형 배열을 비트맵으로 전환.

            MessageBox.Show("완료");
            return bitmap;
            //var dpiX = 96d;
            //var dpiY = 96d;
            //var pixelFormat = targetImage.Format; // for example
            //var bytesPerPixel2 = (pixelFormat.BitsPerPixel + 7) / 8;
            //var stride2 = bytesPerPixel * width;

            //BitmapImage bitmap = (BitmapImage)BitmapSource.Create(width, height, dpiX, dpiY, pixelFormat, null, kkkkk, stride);
            //bitmap.EndInit();
            //return (BitmapImage)bitmap;

        }

       
    }
}


