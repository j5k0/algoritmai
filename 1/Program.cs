using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BMP_example
{
    class Program
    {
        public static void ColorPixel(int x, int y, int l, int color, ref byte[] array)
        {
            int byteIndex = x/8 + y*l;
            int bitPosition = 7 - (x%8);
            array[byteIndex] |= (byte)(color << bitPosition);
        }

        public static void ColorCenter(ref byte[] array, int l, int imageSize){
            int bottomLimit = imageSize/3;
            int upperLimit = imageSize*2;
            for(int x=bottomLimit; x<upperLimit; x++){
                for(int y=bottomLimit; y<upperLimit; y++){
                    ColorPixel(x, y, l, 1, ref array);
                }
            }
        }

        public static void CopyToLocation(int x, int y, ref byte[] array, int l, byte[] copyArray, int copyL, int previousImageSize){
            int startIndex = x*previousImageSize/8 + y*l*previousImageSize;
            int startOffset = (x*previousImageSize)%8;
            for(int i=0; i<previousImageSize; i++){
                byte pushedOut;
                byte previousPushed = (byte)0;
                for(int j=i*copyL; j<i*copyL + copyL; j++){
                    pushedOut = (byte) (copyArray[j] << (8 - startOffset));

                    int index = startIndex + (j-i*copyL) + i*l;
                    int limit = (y*previousImageSize + i)*l + l;

                    if(index < limit){
                        array[index] |= (byte)(copyArray[j] >> startOffset);
                        array[index] |= previousPushed;
                    }
                    previousPushed = pushedOut;
                }
            }
        }

        static void Main(string[] args)
        {
            var header = new byte[62]
                {
                    //Antraštė
                    0x42, 0x4d,
                    0x0, 0x0, 0x0, 0x0, //0x3e, 0xf4, 0x1, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x3e, 0x0, 0x0, 0x0,
                    //Antraštės informacija
                    0x28, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0, // image width
                    0x0, 0x0, 0x0, 0x0, // image height
                    0x1, 0x0,
                    0x1, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    //Spalvų lentelė 
                    0xFF, 0xFF, 0xFF, 0x0,
                    0x0, 0x0, 0x0, 0x0
                };

            
            byte[] copyArray;
            int lastGridSize;
            int lastL;
            int currentGridSize = 3;

            using (FileStream file = new FileStream("sample2.bmp", FileMode.Create, FileAccess.Write))
            {
                file.Write(header);

                int resolution = 23328;
                byte[] resBytes = BitConverter.GetBytes(resolution);

                header[18] = resBytes[0];
                header[19] = resBytes[1];
                header[20] = resBytes[2];
                header[21] = resBytes[3];
                header[22] = resBytes[0];
                header[23] = resBytes[1];
                header[24] = resBytes[2];
                header[25] = resBytes[3];


                var watch = System.Diagnostics.Stopwatch.StartNew();

                int l = (currentGridSize + 31) / 32 * 4;
                var t = new byte[currentGridSize * l];
                ColorPixel(1, 1, l, 1, ref t);

                copyArray = t;
                lastGridSize = currentGridSize;
                currentGridSize *= 6;
                lastL = l;

                l = (currentGridSize + 31) / 32 * 4;
                t = new byte[currentGridSize * l];
                ColorCenter(ref t, l, currentGridSize);
                CopyToLocation(3, 0, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(5, 1, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 2, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(0, 3, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 5, ref t, l, copyArray, lastL, lastGridSize);

                copyArray = t;
                lastGridSize = currentGridSize;
                currentGridSize *= 6;
                lastL = l;

                l = (currentGridSize + 31) / 32 * 4;
                t = new byte[currentGridSize*l];
                ColorCenter(ref t, l, currentGridSize);
                CopyToLocation(3, 0, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(5, 1, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 2, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(0, 3, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 5, ref t, l, copyArray, lastL, lastGridSize);

                copyArray = t;
                lastGridSize = currentGridSize;
                currentGridSize *= 6;
                lastL = l;

                l = (currentGridSize + 31) / 32 * 4;
                t = new byte[currentGridSize*l];
                ColorCenter(ref t, l, currentGridSize);
                CopyToLocation(3, 0, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(5, 1, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 2, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(0, 3, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 5, ref t, l, copyArray, lastL, lastGridSize);
                
                copyArray = t;
                lastGridSize = currentGridSize;
                currentGridSize *= 6;
                lastL = l;

                l = (currentGridSize + 31) / 32 * 4;
                t = new byte[currentGridSize*l];
                ColorCenter(ref t, l, currentGridSize);
                CopyToLocation(3, 0, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(5, 1, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 2, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(0, 3, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 5, ref t, l, copyArray, lastL, lastGridSize);

                copyArray = t;
                lastGridSize = currentGridSize;
                currentGridSize *= 6;
                lastL = l;

                l = (currentGridSize + 31) / 32 * 4;
                t = new byte[currentGridSize*l];
                ColorCenter(ref t, l, currentGridSize);
                CopyToLocation(3, 0, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(5, 1, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 2, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(0, 3, ref t, l, copyArray, lastL, lastGridSize);
                CopyToLocation(4, 5, ref t, l, copyArray, lastL, lastGridSize);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
                file.Write(t);
                file.Close();
            }
        }
    }
}
