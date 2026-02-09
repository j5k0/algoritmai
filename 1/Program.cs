using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BMP_example
{
    // TODO: important note - change int everywhere to double and only round when drawing in order to minimise issues
    class Program
    {
        public static void ColorPixel(int x, int y, int l, int color, ref byte[] array)
        {
            int byteIndex = x/8 + y*l;
            int bitPosition = 7 - (x%8);
            array[byteIndex] |= (byte)(color << bitPosition);
        }

        public static void DrawRectangle(int x0, int y0, int w, int h, ref byte[] array, int l){
            for(int y = y0; y < h; y++){
                for(int x = x0; x < w; x++){
                    ColorPixel(x, y, l, 1, ref array);
                }
            }
        }

        public static void DrawLine(int x0, int y0, int x1, int y1, ref byte[] array, int l){
            for(int y = y0; y <= y1; y++){
                for(int x = x0; x <= x1; x++){
                    if(y < 1000 & x < 1000){
                        ColorPixel(x, y, l, 1, ref array);
                    }
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

            using (FileStream file = new FileStream("sample2.bmp", FileMode.Create, FileAccess.Write))
            {
                int resolution = 1000;
                int squareSize = resolution/3;

                byte[] resBytes = BitConverter.GetBytes(resolution);

                header[18] = resBytes[0];
                header[19] = resBytes[1];
                header[20] = resBytes[2];
                header[21] = resBytes[3];
                header[22] = resBytes[0];
                header[23] = resBytes[1];
                header[24] = resBytes[2];
                header[25] = resBytes[3];


                int l = (resolution + 31) / 32 * 4;
                byte[] array = new byte[resolution * l];

                DrawRectangle(squareSize, squareSize, squareSize*2, squareSize*2, ref array, l);
                DrawLine(0, squareSize, resolution, squareSize, ref array, l);
                DrawLine(0, squareSize*2, resolution, squareSize*2, ref array, l);
                DrawLine(squareSize, 0, squareSize, resolution, ref array, l);
                DrawLine(squareSize*2, 0, squareSize*2, resolution, ref array, l);

                file.Write(header);
                file.Write(array);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                file.Close();
            }
        }
    }
}
