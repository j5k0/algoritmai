using System.Globalization;

namespace IP 
{
    public class Place
    {
        public int Num { get; set; }
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Place(int Num, string Name, string UniqueIdentifier, double X, double Y)
        {
            this.Num = Num;
            this.Name = Name;
            this.UniqueIdentifier = UniqueIdentifier;
            this.X = X;
            this.Y = Y;
        }
    }
    class Program
    {
        static List<Place> ReadFile(string filePath)
        {
            List<Place> array = new List<Place>();
            using(StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while((line = reader.ReadLine()!) != null)
                {
                    string[] parts = line.Split(';');
                    if(parts.Length == 5)
                    {
                        int num = int.Parse(parts[0]);
                        string name = parts[1];
                        string uniqueIdentifier = parts[2];
                        double x = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        double y = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        Place place = new Place(num, name, uniqueIdentifier, x, y);
                        array.Add(place);
                    }
                }
                return array;
            }
        }
        static void Main(string[] args)
        {
            string filePath = "punktai.csv";
            List<Place> places = ReadFile(filePath);
            List<Place>[] busRoutes = GreedyAlgorithm.Solve(places, 4, 67-1);
            foreach(var busRoute in busRoutes)
            {
                foreach(var place in busRoute)
                {
                    Console.Write($"{place.X};{place.Y};\n");
                }
                Console.WriteLine();
            }
        }
    }
}
