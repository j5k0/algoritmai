using System.Collections;
using System.Runtime.InteropServices.Marshalling;

namespace IP
{
    public static class GreedyAlgorithm
    {
        public static List<Place>[] Solve(List<Place> places, int busCount, int startIndex)
        {
            bool[] visited = new bool[places.Count];
            List<Place>[] busRoutes = new List<Place>[busCount];
            visited[startIndex] = true;
            int leftPlaces = places.Count - 1;
            for(int i=0; i<busCount; i++){
                busRoutes[i] = new List<Place>();
                busRoutes[i].Add(places[startIndex]);
            }
            while(leftPlaces > 0)
            {
                for(int i=0; i<busCount; i++)
                {
                    double minDistance = double.MaxValue;
                    int index = -1;
                    for(int j=0; j<places.Count; j++)
                    {
                        if (!visited[j])
                        {
                            Place p1 = busRoutes[i][busRoutes[i].Count - 1];
                            Place p2 = places[j];
                            double distance = Util.GetDistance(p1, p2);
                            if(distance < minDistance)
                            {
                                minDistance = distance;
                                index = j;
                            }
                        }
                    }
                    if(index != -1)
                    {
                        busRoutes[i].Add(places[index]);
                        visited[index] = true;
                        leftPlaces--;
                    }
                }
            }
            for(int i=0; i<busCount; i++)
                busRoutes[i].Add(places[startIndex]);
            return busRoutes;

        }
    }
}