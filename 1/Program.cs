using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BMP_example
{
    class Program
    {
        static int resolution = 2000;
        static int centerPoint = resolution/2;

        public static void ColorPixel(int x, int y, int l, int color, ref byte[] array)
        {
            if(x > 0 && y > 0 && x < resolution && y < resolution){
                int byteIndex = x/8 + y*l;
                int bitPosition = 7 - (x%8);
                array[byteIndex] |= (byte)(color << bitPosition);
            }
        }

        public static void DrawParallelogram(double centerX, double centerY, double sideLength, double bottomLeftAngle, ref byte[] array, int l){
            double rad = bottomLeftAngle * Math.PI / 180.0;
            double offsetX = (sideLength * Math.Cos(rad))/2;
            double offsetY = (sideLength - sideLength * Math.Sin(rad))/2;
            double halfLength = sideLength/2;
            double a = Math.Tan(rad);

            int minX = (int)Math.Round(centerX - offsetX - halfLength);
            int maxX = (int)Math.Round(centerX + offsetX + halfLength);

            int minY = (int)Math.Round(centerY + offsetY - halfLength);
            int maxY = (int)Math.Round(centerY - offsetY + halfLength);

            int b1 = (int)Math.Round(minY - a*minX);
            int b2 = (int)Math.Round(maxY - a*maxX);

            //for(int i=0; i<resolution; i++){
            //    ColorPixel(i, (int)(a*i + b1), l, 1, ref array);
            //    ColorPixel(i, (int)(a*i + b2), l, 1, ref array);
            //}

            for(int y=minY; y<maxY; y++){
                for(int x=minX; x<maxX; x++){
                    if(y <= a*x + b1 && y >= a*x + b2)
                        ColorPixel(x, y, l, 1, ref array);

                }
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

        public static void DrawRecursiveDepth(int currentDepth, int maxDepth, double x, double y, double prevX, double prevY, ref byte[] array, int l){
            if(currentDepth >= maxDepth)
                return;

            int mult = (int)Math.Pow(6, currentDepth);
            double currentGridSize = resolution/mult;
            double previousGridSize = currentGridSize*6;
            double previousHalfSquare = previousGridSize/6;
            double squareSize = currentGridSize/3;
            double halfSquareSize = squareSize/2;

            // inner grid line drawing loop
            // only fill with inner lines inside the grid, the outside lines will be drawn by the outer line drawing loop
            for(int i=1; i<3; i++){
                DrawLine((x - halfSquareSize*3 + squareSize*i), (y - halfSquareSize*3), (x - halfSquareSize*3 + squareSize*i), (y + halfSquareSize*3), ref array, l);
                DrawLine((x - halfSquareSize*3), (y - halfSquareSize*3 + squareSize*i), (x + halfSquareSize*3), (y - halfSquareSize*3 + squareSize*i), ref array, l);
            }

            // outer line drawing loop
            // for filling the previous square with additional vertical and horizontal lines
            for(int i=1; i<6; i++){
                DrawLine((prevX - previousHalfSquare*3 + currentGridSize*i), (prevY - previousHalfSquare*3), (prevX - previousHalfSquare*3 + currentGridSize*i), (prevY + previousHalfSquare*3), ref array, l);
                DrawLine((prevX - previousHalfSquare*3), (prevY - previousHalfSquare*3 + currentGridSize*i), (prevX + previousHalfSquare*3), (prevY - previousHalfSquare*3 + currentGridSize*i), ref array, l);
            }

            //DrawRectangle((x - halfSquareSize), (y - halfSquareSize), (x + halfSquareSize), (y + halfSquareSize), ref array, l);
            DrawParallelogram(x, y, squareSize, 45, ref array, l);

            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*1), (y+halfSquareSize/2*-5), x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*5), (y+halfSquareSize/2*-3), x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*3), (y+halfSquareSize/2*-1), x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*-5), (y+halfSquareSize/2*1), x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*3), (y+halfSquareSize/2*5),  x, y, ref array, l);
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

                DrawRecursiveDepth(0, 3, centerPoint, centerPoint, centerPoint, centerPoint, ref array, l);
                //DrawParallelogram(1000, 1000, 500, 90, ref array, l);

                file.Write(header);
                file.Write(array);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                file.Close();
            }
        }
    }
}
