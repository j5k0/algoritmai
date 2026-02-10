using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BMP_example
{
    // TODO: important note - change int everywhere to double and only round when drawing in order to minimise issues
    class Program
    {
        static int resolution = 20000;
        static int centerPoint = resolution/2;

        public static void ColorPixel(int x, int y, int l, int color, ref byte[] array)
        {
            if(x+y != resolution*2){
                int byteIndex = x/8 + y*l;
                int bitPosition = 7 - (x%8);
                array[byteIndex] |= (byte)(color << bitPosition);
            }
        }

        public static void DrawRectangle(double x0, double y0, double w, double h, ref byte[] array, int l){
            int rx0 = (int)Math.Round(x0);
            int ry0 = (int)Math.Round(y0);
            int rw = (int)Math.Round(w);
            int rh = (int)Math.Round(h);
            for(int y = ry0; y < rh; y++){
                for(int x = rx0; x < rw; x++){
                    ColorPixel(x, y, l, 1, ref array);
                }
            }
        }

        public static void DrawLine(double x0, double y0, double x1, double y1, ref byte[] array, int l){
            int rx0 = (int)Math.Round(x0);
            int ry0 = (int)Math.Round(y0);
            int rx1 = (int)Math.Round(x1);
            int ry1 = (int)Math.Round(y1);

            for(int y = ry0; y <= ry1; y++){
                for(int x = rx0; x <= rx1; x++){
                    if(x >= 0 && y >= 0 && y < resolution && x < resolution){
                        ColorPixel(x, y, l, 1, ref array);
                    }
                }
            }
        }

        public static void DrawRecursiveDepth(int currentDepth, int maxDepth, double x, double y, ref byte[] array, int l){
            if(currentDepth >= maxDepth)
                return;

            int mult = (int)Math.Pow(6, currentDepth);
            double currentGridSize = resolution/mult;
            //double previousGridSize = currentGridSize*6;
            double squareSize = currentGridSize/3;
            //double previousSquareSize = squareSize*6;
            double halfSquareSize = squareSize/2;
            //double previousHalfSquareSize = halfSquareSize*6;

            for(int i=0; i<=currentGridSize/squareSize; i++){

                DrawLine((x - halfSquareSize*3 + squareSize*i), (y - halfSquareSize*3), (x - halfSquareSize*3 + squareSize*i), (y + halfSquareSize*3), ref array, l);
                DrawLine((x - halfSquareSize*3), (y - halfSquareSize*3 + squareSize*i), (x + halfSquareSize*3), (y - halfSquareSize*3 + squareSize*i), ref array, l);
            }

            DrawRectangle((x - halfSquareSize), (y - halfSquareSize), (x + halfSquareSize), (y + halfSquareSize), ref array, l);

            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*1), (y+halfSquareSize/2*-5), ref array, l);
            //DrawRecursiveDepth(currentDepth + 1, maxDepth, (halfSquareSize*5 + resolution/(mult*12)), (halfSquareSize*1 + resolution/(mult*12)), ref array, l);
            //DrawRecursiveDepth(currentDepth + 1, maxDepth, (halfSquareSize*4 + resolution/(mult*12)), (halfSquareSize*2 + resolution/(mult*12)), ref array, l);
            //DrawRecursiveDepth(currentDepth + 1, maxDepth, (halfSquareSize*0 + resolution/(mult*12)), (halfSquareSize*3 + resolution/(mult*12)), ref array, l);
            //DrawRecursiveDepth(currentDepth + 1, maxDepth, (halfSquareSize*4 + resolution/(mult*12)), (halfSquareSize*5 + resolution/(mult*12)), ref array, l);
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
                //int squareSize = resolution/3;
                //int smallerSquareSize = squareSize/6;

                //int cubeSize = resolution/6;

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

                DrawRecursiveDepth(0, 6, centerPoint, centerPoint, ref array, l);

                /*DrawRectangle(squareSize, squareSize, squareSize*2, squareSize*2, ref array, l);

                // Y axis
                DrawLine(0, squareSize, resolution, squareSize, ref array, l);
                DrawLine(0, squareSize*2, resolution, squareSize*2, ref array, l);
                DrawLine(0, (int)(squareSize*1.5), resolution, (int)(squareSize*1.5), ref array, l);
                DrawLine(0, (int)(squareSize*0.5), resolution, (int)(squareSize*0.5), ref array, l);
                DrawLine(0, (int)(squareSize*2.5), resolution, (int)(squareSize*2.5), ref array, l);

                // X axis
                DrawLine(squareSize, 0, squareSize, resolution, ref array, l);
                DrawLine(squareSize*2, 0, squareSize*2, resolution, ref array, l);
                DrawLine((int)(squareSize*1.5), 0, (int)(squareSize*1.5), resolution, ref array, l);
                DrawLine((int)(squareSize*0.5), 0, (int)(squareSize*0.5), resolution, ref array, l);
                DrawLine((int)(squareSize*2.5), 0, (int)(squareSize*2.5), resolution, ref array, l);

                DrawRectangle(cubeSize*3 + smallerSquareSize, smallerSquareSize, cubeSize*3 + 2*smallerSquareSize, 2*smallerSquareSize, ref array, l);

                DrawLine(cubeSize*3, cubeSize/3, cubeSize*(3+1), cubeSize/3, ref array, l);
                DrawLine(cubeSize*3, cubeSize/3*2, cubeSize*(3+1), cubeSize/3*2, ref array, l);

                DrawLine(cubeSize*3 + cubeSize/3, cubeSize*0, cubeSize*3 + cubeSize/3, cubeSize*(0+1), ref array, l);
                DrawLine(cubeSize*3 + cubeSize/3*2, cubeSize*0, cubeSize*3 + cubeSize/3*2, cubeSize*(0+1), ref array, l);*/

                file.Write(header);
                file.Write(array);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                file.Close();
            }
        }
    }
}
