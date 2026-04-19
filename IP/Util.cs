namespace IP
{
    class Util
    {
        public static double GetDistance(Place a, Place b)
        {
            double distance = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            return distance;
        }
    }
}