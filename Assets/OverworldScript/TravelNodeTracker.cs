using System.Collections.Generic;
using UnityEngine;

public struct OverworldRoute
{
    public List<int> RouteIdxs { get; set; }
    public int Cost { get; set; }

    public void Initialize()
    {
        RouteIdxs = new List<int>();
        Cost = 0;
    }
    public OverworldRoute Clone()
    {
        return new OverworldRoute
        {
            Cost = this.Cost,
            RouteIdxs = this.RouteIdxs != null ? new List<int>(this.RouteIdxs) : null
        };
    }
}

[ExecuteInEditMode]
public class TravelNodeTracker : MonoBehaviour
{
    public GameObject[] PositionNodes;
    
    public static TravelNodeTracker Instance;

    public GameObject DebugTarget;
    public int DebugNodeInt = 0;

    public void OnEnable()
    {
        Instance = this;
    }

    public TravelNodeConnection FindConnection(int source, int endNode)
    {
        foreach (TravelNodeConnection connection in PositionNodes[source].GetComponent<TravelNodeScript>().ConnectedNodes)
        {
            if(connection.TravelNode.SceneID == endNode) return connection;
        }
        return new TravelNodeConnection();
    }

    public (bool, OverworldRoute) FindShortestRoute(OverworldRoute currentRoute, int endingNodeIdx)
    {
        TravelNodeScript latestNode = PositionNodes[currentRoute.RouteIdxs[^1]].GetComponent<TravelNodeScript>();
        TravelNodeScript endingNode = PositionNodes[endingNodeIdx].GetComponent<TravelNodeScript>();

        foreach (TravelNodeConnection nextNode in latestNode.ConnectedNodes)
        {
            if (nextNode.TravelNode == endingNode)
            {
                currentRoute.RouteIdxs.Add(nextNode.TravelNode.SceneID);
                currentRoute.Cost += 1;
                return (true, currentRoute);
            }
        }

        int cheapestRoute = 1000000;
        bool routeFound = false;
        OverworldRoute bestRoute = new OverworldRoute();

        foreach (TravelNodeConnection nextNode in latestNode.ConnectedNodes)
        {
            if (currentRoute.RouteIdxs.Contains(nextNode.TravelNode.SceneID)) continue;

            OverworldRoute newRoute = currentRoute.Clone();
            newRoute.RouteIdxs.Add(nextNode.TravelNode.SceneID);
            newRoute.Cost += 1;

            bool complete;
            (complete, newRoute) = FindShortestRoute(newRoute, endingNodeIdx);

            if (complete && newRoute.Cost < cheapestRoute)
            {
                cheapestRoute = newRoute.Cost;
                routeFound = true;
                bestRoute = newRoute;
            }
        }

        if (routeFound)
        {
            return (true, bestRoute);
        }

        return (false, currentRoute);
    }
}
