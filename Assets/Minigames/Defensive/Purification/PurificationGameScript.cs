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
    public PurificationLevelPacksSO CurrentLevelPack;
    public int CurrentLevelInPack = 0;

    public VentGridScript VentGridData;
    private List<WaitingExpansion> PrevDeadEndExpansions;

    public GameObject MainBackdrop;
    public GameObject WinText;

    public void Start()
    {
        VentGridData.SpawnGridFromSaveData(CurrentLevelPack.Levels[CurrentLevelInPack]);
        PipeStackScript.VentRotationEvent += UpdatePipeRoutes;
        UpdatePipeRoutes();
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
            Debug.Log("Purification: YOUWIN");
            SpawnWinScreen();
        }
        if(PrevDeadEndExpansions != null)
        {
            foreach (WaitingExpansion prevDeadEnd in PrevDeadEndExpansions)
            {
                bool stillDeadEnd = false;
                foreach (WaitingExpansion newDeadEnd in DeadEndExpansions)
                {
                    if(prevDeadEnd.sourceVent == newDeadEnd.sourceVent && prevDeadEnd.expansionDirection == newDeadEnd.expansionDirection)
                    {
                        stillDeadEnd = true;
                        break;
                    }
                }
                if (!stillDeadEnd)
                {
                    int adjacentVentID = VentGridData.ConvertVector2ToPosID(prevDeadEnd.sourceVent.VentPosID, prevDeadEnd.expansionDirection);
                    if (adjacentVentID > 0)
                    {
                        PipeStackScript adjacentVentScript = VentGridData.PipeStacks[adjacentVentID].GetComponent<PipeStackScript>();
                        switch (prevDeadEnd.expansionDirection)
                        {
                            case Direction.UP:
                                adjacentVentScript.DownLeakParticles.SetActive(false);
                                break;
                            case Direction.RIGHT:
                                adjacentVentScript.LeftLeakParticles.SetActive(false);
                                break;
                            case Direction.DOWN:
                                adjacentVentScript.UpLeakParticles.SetActive(false);
                                break;
                            case Direction.LEFT:
                                adjacentVentScript.RightLeakParticles.SetActive(false);
                                break;
                        }
                    }
                }
            }
        }
        PrevDeadEndExpansions = DeadEndExpansions;
    }

    public void ResetPipeRoutes()
    {
        foreach (GameObject vent in VentGridData.PipeStacks)
        {
            vent.GetComponent<PipeStackScript>().SetLightIdle();
        }
    }

    public void NextLevel()
    {
        CurrentLevelInPack += 1;
        if(CurrentLevelInPack < CurrentLevelPack.Levels.Count)
        {
            Debug.Log("LoadingNextLevel");
            VentGridData.SpawnGridFromSaveData(CurrentLevelPack.Levels[CurrentLevelInPack]);
            UpdatePipeRoutes();
        }
        else
        {
            Debug.Log("LevelPackComplete");
            VentGridData.ClearGrid();
        }
    }

    public void SpawnWinScreen()
    {
        GameObject newWinText = Instantiate(WinText);
        newWinText.transform.parent = MainBackdrop.transform;
        newWinText.transform.localScale = Vector3.one;
        newWinText.transform.rotation = Quaternion.identity;
        newWinText.transform.localPosition = Vector3.zero;

        newWinText.GetComponent<MaterialGradient>().OnCompletion.AddListener(NextLevel);
    }
}
