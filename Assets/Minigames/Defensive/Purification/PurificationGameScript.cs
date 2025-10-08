using System.Collections.Generic;
using UnityEngine;

struct WaitingExpansion
{
    public Direction expansionDirection;
    public PipeStackScript sourceVent;
}
struct VentRouteData
{
    public PipeStackScript SourceVent;
    public List<PipeStackScript> PrimaryExpanded;
    public List<PipeStackScript> SecondaryExpanded;
    public List<WaitingExpansion> WaitingExpansions;
    public bool GoalFound;
}
public class PurificationGameScript : MonoBehaviour
{
    public VentGridScript VentGridData;

    public void Start()
    {
        PipeStackScript.VentRotationEvent += UpdatePipeRoutes;
    }

    public void StartGame()
    {

    }
    public void UpdatePipeRoutes()
    {
        ResetPipeRoutes();

        List<VentRouteData> ventRoutes = new List<VentRouteData>();
        List<WaitingExpansion> DeadEndExpansions = new List<WaitingExpansion>();

        bool GoalsMissing = false;
        bool DeadPipesFound = false;

        //Find the routes for every source.
        foreach (PipeStackScript fumeSource in VentGridData.Sources)
        {
            PipeStackScript currentVent = fumeSource;

            //Find valid expansion directions.
            List<Direction> possibleExpansionDirection = currentVent.GetPossibleExpansions(Direction.NULL);

            foreach (Direction expansionDirection in possibleExpansionDirection)
            {
                VentRouteData newVentRouteData = new VentRouteData();

                newVentRouteData.SourceVent = currentVent;
                currentVent.SetLightActive();

                newVentRouteData.PrimaryExpanded = new List<PipeStackScript>();
                newVentRouteData.SecondaryExpanded = new List<PipeStackScript>();
                newVentRouteData.WaitingExpansions = new List<WaitingExpansion>();
                newVentRouteData.GoalFound = false;

                newVentRouteData.PrimaryExpanded.Add(currentVent);

                WaitingExpansion newWaitingExpansion = new WaitingExpansion();
                newWaitingExpansion.expansionDirection = expansionDirection;
                newWaitingExpansion.sourceVent = currentVent;
                newVentRouteData.WaitingExpansions.Add(newWaitingExpansion); 

                int i = 0; //I is just being used to avoid infinite loops while debugging.
                while (newVentRouteData.WaitingExpansions.Count > 0)
                {
                    //Pop closest expansion:
                    WaitingExpansion expansionData = newVentRouteData.WaitingExpansions[0];
                    newVentRouteData.WaitingExpansions.RemoveAt(0);

                    expansionData.sourceVent.SetLightActive();

                    (bool validVent, bool secondaryVent, PipeStackScript nextVent) = VentGridData.ExpansionCheck(expansionData.expansionDirection, expansionData.sourceVent.VentPosID);

                    Debug.Log("-");
                    Debug.Log(expansionData.sourceVent.VentPosID);
                    Debug.Log(expansionData.expansionDirection);

                    if (!validVent)
                    {
                        DeadEndExpansions.Add(expansionData);
                        continue;
                    }

                    if (nextVent.isGoal)
                    {
                        newVentRouteData.GoalFound = true;
                        nextVent.SetLightActive();
                        continue;
                    }

                    if (nextVent.isSource) continue;

                    Debug.Log(nextVent.VentPosID);
                    Debug.DrawLine(expansionData.sourceVent.transform.position, nextVent.transform.position, Color.red, 30f);

                    if (secondaryVent)
                    {
                        if (newVentRouteData.SecondaryExpanded.Contains(nextVent)) continue;
                        newVentRouteData.SecondaryExpanded.Add(nextVent);
                    } else
                    {
                        if (newVentRouteData.PrimaryExpanded.Contains(nextVent)) continue;
                        newVentRouteData.PrimaryExpanded.Add(nextVent);
                    }

                    possibleExpansionDirection = nextVent.GetPossibleExpansions(expansionData.expansionDirection);

                    foreach (Direction expansionDirection2 in possibleExpansionDirection)
                    {
                        Debug.Log(expansionDirection2);
                        WaitingExpansion newWaitingExpansion2 = new WaitingExpansion();
                        newWaitingExpansion2.expansionDirection = expansionDirection2;
                        newWaitingExpansion2.sourceVent = nextVent;
                        newVentRouteData.WaitingExpansions.Add(newWaitingExpansion2);
                    }

                    //Delete this later, just to make sure I don't lock my computer.
                    i++;
                    if (i > 1000) break;
                }


                foreach (WaitingExpansion deadExpansion in DeadEndExpansions)
                {
                    deadExpansion.sourceVent.SetLightLeaking();
                    DeadPipesFound = true;
                }
                if(!newVentRouteData.GoalFound) GoalsMissing = true;

                ventRoutes.Add(newVentRouteData);
            }
        }
        foreach (VentRouteData ventRoute in ventRoutes)
        {
            Debug.Log(ventRoute.GoalFound);
            if (!ventRoute.GoalFound) ventRoute.SourceVent.SetLightLeaking();
        }

        if(!GoalsMissing && !DeadPipesFound)
        {
            Debug.Log("YOUWIN");
        }
    }
    public void ResetPipeRoutes()
    {
        foreach (GameObject vent in VentGridData.PipeStacks)
        {
            vent.GetComponent<PipeStackScript>().SetLightIdle();
        }
    }
}
