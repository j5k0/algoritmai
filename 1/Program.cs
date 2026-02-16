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

            int b1, b2;

            if(a > resolution){
                b1 = maxY;
                b2 = minY;
            }
            else{
                b1 = (int)Math.Round(minY - a*minX);
                b2 = (int)Math.Round(maxY - a*maxX);
            }

            for(int i=0; i<2; i++){
                DrawAngledLine(bottomLeftAngle, centerX - halfLength + sideLength*i, centerY, sideLength*1.5, l, ref array);
                DrawAngledLine(0, centerX - offsetX + offsetX*2*i, minY + (maxY-minY)*i, sideLength*1.5, l, ref array);
                //DrawAngledLine(bottomLeftAngle, b2 + (b1-b2)/2*i, sideLength*3, l, ref array);
                //DrawAngledLine(0, minY + (maxY-minY)/2*i, sideLength*3, l, ref array);
            }

            double k;

            if(a > resolution)
                k = 0;
            else
                k = a;

            for(int y=minY; y<maxY; y++){
                for(int x=minX; x<maxX; x++){
                    if(y <= k*x + b1 && y >= k*x + b2)
                        ColorPixel(x, y, l, 1, ref array);
                }
            }
        }

        public static void DrawAngledLine(double angle, double cx, double cy, double halfLength, int l, ref byte[] array){
            double rad = angle * Math.PI / 180.0;
            double dx = Math.Cos(rad);
            double dy = Math.Sin(rad);
            int lim = (int)Math.Round(halfLength);
            for(int i=-lim; i<lim; i++){
                int x = (int)Math.Round(cx + i*dx);
                int y = (int)Math.Round(cy + i*dy);
                ColorPixel(x, y, l, 1, ref array);
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

            DrawParallelogram(x, y, squareSize, 89.999, ref array, l);

            double rad = 89.999 * Math.PI / 180;
            double offsetX = (squareSize * Math.Cos(rad))/2;
            double offsetY = (squareSize - squareSize * Math.Sin(rad))/2;

            double my = Math.Sin(rad);
            double mx = Math.Cos(rad);

            // this shit somehow fucking relates offsetX*2.5 ----> halfSquareSize*-2.5 and so on and so on

            DrawRecursiveDepth(currentDepth + 1, maxDepth, x+halfSquareSize*0.5 - offsetX*2.5, y+halfSquareSize*-2.5*my, x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize*2.5 - offsetX*1.5), y+halfSquareSize*-1.5*my, x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize*1.5 - offsetX*0.5), y+halfSquareSize*-0.5*my, x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*-5)+ offsetX*0.5, y+halfSquareSize*0.5*my, x, y, ref array, l);
            DrawRecursiveDepth(currentDepth + 1, maxDepth, (x+halfSquareSize/2*3) + offsetX*2.5, y+halfSquareSize*2.5*my, x, y, ref array, l);
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
                //DrawAngledLine(45, centerPoint+431, l, ref array);

                file.Write(header);
                file.Write(array);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                file.Close();
            }
        }
    }
}
