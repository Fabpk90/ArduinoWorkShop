using System;
using UnityEngine;

class Dijkstra
{
 
    private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
    {
        int min = int.MaxValue;
        int minIndex = 0;
 
        for (int v = 0; v < verticesCount; ++v)
        {
            if (shortestPathTreeSet[v] == false && distance[v] <= min)
            {
                min = distance[v];
                minIndex = v;
            }
        }
 
        return minIndex;
    }
 
    private static void Print(int[] distance, int verticesCount)
    {
        Debug.Log("Vertex    Distance from source");

        for (int i = 0; i < verticesCount; ++i)
            Debug.Log("" + i + "\n" + distance[i]);
    }
 
    public static int[] DijkstraAlgo(int[,] graph, int source, int verticesCount)
    {
        int[] distance = new int[verticesCount];
        bool[] shortestPathTreeSet = new bool[verticesCount];
 
        for (int i = 0; i < verticesCount; ++i)
        {
            distance[i] = int.MaxValue;
            shortestPathTreeSet[i] = false;
        }
 
        distance[source] = 0;
 
        for (int count = 0; count < verticesCount - 1; ++count)
        {
            int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
            shortestPathTreeSet[u] = true;
 
            for (int v = 0; v < verticesCount; ++v)
                if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                    distance[v] = distance[u] + graph[u, v];
        }
 
        Print(distance, verticesCount);
        return distance;
    }
 
    static void Main(string[] args)
    {
        int[,] graph =  {
            { 0, 6, 0, 0, 0, 0, 0, 9, 0 },
            { 6, 0, 9, 0, 0, 0, 0, 11, 0 },
            { 0, 9, 0, 5, 0, 6, 0, 0, 2 },
            { 0, 0, 5, 0, 9, 16, 0, 0, 0 },
            { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
            { 0, 0, 6, 0, 10, 0, 2, 0, 0 },
            { 0, 0, 0, 16, 0, 2, 0, 1, 6 },
            { 9, 11, 0, 0, 0, 0, 1, 0, 5 },
            { 0, 0, 2, 0, 0, 0, 6, 5, 0 }
        };
 
        DijkstraAlgo(graph, 0, 9);
    }
}