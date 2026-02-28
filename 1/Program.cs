using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BMP_example
{
    class Program
    {
        static int resolution = 4000;
        static int gridSize = 3000;
        static int angle = 90;
        static int minDetail = 1;
        static int centerPoint = resolution/2;

        public static void ColorPixel(int x, int y, int l, ref byte[] array)
        {
            if(x > 0 && y > 0 && x < resolution && y < resolution){
                long byteIndex = x/8 + y*l;
                int bitPosition = 7 - (x%8);
                array[byteIndex] |= (byte)(1 << bitPosition);
            }
        }

        public static void DrawParallelogram(double centerX, double centerY, double sideLength, double bottomLeftAngle, bool finalLayer, ref byte[] array, int l){
            double rad = bottomLeftAngle * Math.PI / 180.0;
            double offsetX = (sideLength * Math.Cos(rad))/2;
            double offsetY = (sideLength - sideLength * Math.Sin(rad))/2;
            double halfLength = sideLength/2;
            double a = Math.Tan(rad);
            
            int minY = (int)Math.Round(centerY + Math.Abs(offsetY) - halfLength);
            int maxY = (int)Math.Round(centerY - Math.Abs(offsetY) + halfLength);

            int increase = finalLayer ? 2 : 1;

            for(int i=-3; i<=3; i+=increase){
                DrawAngledLine(bottomLeftAngle, centerX + halfLength*i, centerY, sideLength*1.5, l, ref array);
                DrawAngledLine(0, centerX + (offsetX)*i, centerY + (maxY-minY)*i/2, sideLength*1.5, l, ref array);
            }

            for(int y=minY; y<maxY; y++){
                double start = (centerX - halfLength) + (y-centerY)/a;
                for(int x=(int)start; x < start+sideLength; x++){
                    ColorPixel(x, y, l, ref array);
                }
            }
        }

        public static void DrawAngledLine(double angle, double cx, double cy, double halfLength, int l, ref byte[] array){
            double rad = angle * Math.PI / 180.0;
            double dx = Math.Cos(rad);
            double dy = Math.Sin(rad);
            int lim = (int)Math.Round(halfLength);
            for(int i=-lim; i<lim; i++){
                int x = (int)(cx + i*dx);
                int y = (int)(cy + i*dy);
                ColorPixel(x, y, l, ref array);
            }
        }

        public static void DrawRecursiveDepth(int currentDepth, int maxDepth, int angle, double squareSize, double x, double y, ref byte[] array, int l){
            if(currentDepth >= maxDepth)
                return;

            double halfSquareSize = squareSize/2;

            DrawParallelogram(x, y, squareSize, angle, currentDepth + 1 == maxDepth, ref array, l);

            double rad = angle * Math.PI / 180.0;
            double offsetX = (squareSize * Math.Cos(rad))/2;
            double offsetY = (squareSize - squareSize * Math.Sin(rad))/2;

            double my = Math.Sin(rad);
            double mx = Math.Cos(rad);

            int nextDepth = currentDepth + 1;
            double nextSize = squareSize/6;

            DrawRecursiveDepth(nextDepth, maxDepth, angle, nextSize, x+halfSquareSize*0.5 - offsetX*2.5,  y+halfSquareSize*-2.5*my, ref array, l);
            DrawRecursiveDepth(nextDepth, maxDepth, angle, nextSize, x+halfSquareSize*2.5 - offsetX*1.5,  y+halfSquareSize*-1.5*my, ref array, l);
            DrawRecursiveDepth(nextDepth, maxDepth, angle, nextSize, x+halfSquareSize*1.5 - offsetX*0.5,  y+halfSquareSize*-0.5*my, ref array, l);
            DrawRecursiveDepth(nextDepth, maxDepth, angle, nextSize, x+halfSquareSize*-2.5 + offsetX*0.5, y+halfSquareSize*0.5*my,  ref array, l);
            DrawRecursiveDepth(nextDepth, maxDepth, angle, nextSize, x+halfSquareSize*1.5 + offsetX*2.5,  y+halfSquareSize*2.5*my,  ref array, l);
        }

        public static void DrawRecursiveSize(int angle, double squareSize, double x, double y, ref byte[] array, int l){
            if(squareSize <= minDetail)
                return;

            double halfSquareSize = squareSize/2;

            DrawParallelogram(x, y, squareSize, angle, squareSize/6 <= minDetail, ref array, l);

            double rad = angle * Math.PI / 180.0;
            double offsetX = (squareSize * Math.Cos(rad))/2;
            double offsetY = (squareSize - squareSize * Math.Sin(rad))/2;

            double my = Math.Sin(rad);
            double mx = Math.Cos(rad);

            double nextSize = squareSize/6;

            DrawRecursiveSize(angle, nextSize, x+halfSquareSize*0.5 - offsetX*2.5,  y+halfSquareSize*-2.5*my, ref array, l);
            DrawRecursiveSize(angle, nextSize, x+halfSquareSize*2.5 - offsetX*1.5,  y+halfSquareSize*-1.5*my, ref array, l);
            DrawRecursiveSize(angle, nextSize, x+halfSquareSize*1.5 - offsetX*0.5,  y+halfSquareSize*-0.5*my, ref array, l);
            DrawRecursiveSize(angle, nextSize, x+halfSquareSize*-2.5 + offsetX*0.5, y+halfSquareSize*0.5*my,  ref array, l);
            DrawRecursiveSize(angle, nextSize, x+halfSquareSize*1.5 + offsetX*2.5,  y+halfSquareSize*2.5*my,  ref array, l);
        }

        public static void SetImageSize(ref byte[] header, int resolution, ref byte[] array, ref int l){
                byte[] resBytes = BitConverter.GetBytes(resolution);
                l = (resolution + 31) / 32 * 4;
                array = new byte[resolution*l];
                header[18] = resBytes[0];
                header[19] = resBytes[1];
                header[20] = resBytes[2];
                header[21] = resBytes[3];
                header[22] = resBytes[0];
                header[23] = resBytes[1];
                header[24] = resBytes[2];
                header[25] = resBytes[3];

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

            using (FileStream file = new FileStream("sample.bmp", FileMode.Create, FileAccess.Write))
            {
                byte[] array = {};
                int l = 0;
                SetImageSize(ref header, resolution, ref array, ref l);

                Stopwatch stopwatch = new Stopwatch();
                int[] depthArray = { 1, 1, 2, 4, 8, 12 };
                int[] resolutionArray = { 1024, 1024, 2048, 4096, 8192, 16384 };

                /*for(int i=0; i<depthArray.Length; i++){
                    stopwatch.Reset();
                    stopwatch.Start();
                    DrawRecursiveDepth(0, depthArray[i], angle, gridSize/3, centerPoint, centerPoint, ref array, l);
                    stopwatch.Stop();
                    Console.WriteLine($"Rekursinio gylio {depthArray[i]} vykdymo trukmė: {stopwatch.ElapsedMilliseconds} ms.");
                }*/

                for(int i=0; i<resolutionArray.Length; i++){
                    resolution = resolutionArray[i];
                    SetImageSize(ref header, resolution, ref array, ref l);
                    stopwatch.Reset();
                    stopwatch.Start();
                    DrawRecursiveSize(angle, resolution/3, resolution/2, resolution/2, ref array, l);
                    stopwatch.Stop();
                    Console.WriteLine($"{resolution} matmenų paveikslėlio piešimo trukmė: {stopwatch.ElapsedMilliseconds} ms.");
                }

                file.Write(header);
                file.Write(array);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                file.Close();
            }
        }
    }
}
