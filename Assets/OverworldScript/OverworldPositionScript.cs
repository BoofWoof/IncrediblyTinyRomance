using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPositionScript : MonoBehaviour
{
    public CharacterSpeechScript NameSource;
    [HideInInspector] public string CharacterName;

    public int CurrentNode = 0;

    public Transform RootTransform;
    public GestureScript GestureControl;
    public bool CharacterMobile = false;

    public float Speed = 1f;
    public float RotationSpeed = 360f;

    public static List<OverworldPositionScript> PositionScripts = new List<OverworldPositionScript>();

    [HideInInspector] public Coroutine WalkToCoroutine;

    public void Start()
    {
        CharacterName = NameSource.SpeakerName;
        PositionScripts.Add(this);
    }

    public static void GoTo(string name, int positionNodeIdx)
    {
        foreach (OverworldPositionScript overworldPositionScript in PositionScripts)
        {
            if (overworldPositionScript.CharacterName != name && overworldPositionScript.NameSource.NickName != name) continue;
            overworldPositionScript.GoTo(positionNodeIdx);
        }
    }
    public void GoTo(int positionNodeIdx)
    {
        Transform targetNode = TravelNodeTracker.Instance.PositionNodes[positionNodeIdx].transform;

        RootTransform.position = targetNode.position;

        Vector3 currentRotation = RootTransform.rotation.eulerAngles;
        Quaternion finalRotation = Quaternion.Euler(currentRotation.x, targetNode.rotation.eulerAngles.y + 180f, currentRotation.z);
        RootTransform.rotation = finalRotation;
    }

    public static void StartWalkTo(string name, int positionNodeIdx)
    {
        foreach (OverworldPositionScript overworldPositionScript in PositionScripts)
        {
            if (overworldPositionScript.CharacterName != name && overworldPositionScript.NameSource.NickName != name) continue;
            overworldPositionScript.StartWalkTo(positionNodeIdx);
        }
    }

    public void StartWalkTo(int positionNodeIdx)
    {
        WalkToCoroutine = StartCoroutine(FollowRouteTo(positionNodeIdx));
    }
    public IEnumerator FollowRouteTo(int positionNodeIdx)
    {
        GestureControl.CharacterAnimator.SetBool("Sitting", false);
        GestureControl.CharacterAnimator.SetBool("Looming", false);
        GestureControl.CharacterAnimator.SetBool("WalkTo", true);

        OverworldRoute route = new OverworldRoute();
        route.Initialize();
        route.RouteIdxs.Add(CurrentNode);
        bool routeFound;
        (routeFound, route) = TravelNodeTracker.Instance.FindShortestRoute(route, positionNodeIdx);
        if (!routeFound) Debug.LogError("No route found.");

        int prev_idx = route.RouteIdxs[0];
        route.RouteIdxs.RemoveAt(0); //Remove Initial Node
        foreach (int idx in route.RouteIdxs)
        {
            TravelNodeConnection connection = TravelNodeTracker.Instance.FindConnection(prev_idx, idx);

            Debug.Log(idx);

            if (connection.UseAnimationInsteadOfWalking)
            {
                yield return StartCoroutine(WaitForMobility(idx));
            } else
            {
                yield return StartCoroutine(WalkTo(idx));
            }
            prev_idx = idx;
        }

        GestureControl.CharacterAnimator.SetBool("WalkTo", false);
        CharacterMobile = false;
    }

    public IEnumerator WaitForMobility(int positionNodeIdx)
    {
        CharacterMobile = false;
        while (CharacterMobile == false)
        {
            yield return null;
        }

        SetNewStation(positionNodeIdx);
    }
    public IEnumerator WalkTo(int positionNodeIdx)
    {
        Transform targetNode = TravelNodeTracker.Instance.PositionNodes[positionNodeIdx].transform;

        while (!CharacterMobile)
        {
            yield return null;
        }

        Vector3 currentRotation = RootTransform.rotation.eulerAngles;

        Vector3 direction = RootTransform.position-targetNode.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation = Quaternion.Euler(currentRotation.x, targetRotation.eulerAngles.y, currentRotation.z);

        Quaternion finalRotation = Quaternion.Euler(currentRotation.x, targetNode.rotation.eulerAngles.y + 180f, currentRotation.z);

        while (Vector3.Magnitude(RootTransform.position - targetNode.position) > 0.01f)
        {
            RootTransform.position = Vector3.MoveTowards(RootTransform.position, targetNode.position, Speed * Time.deltaTime);
            RootTransform.rotation = Quaternion.RotateTowards(RootTransform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            yield return null;
        }
        RootTransform.position = targetNode.position;
        RootTransform.rotation = targetRotation;

        while (Quaternion.Angle(RootTransform.rotation, finalRotation) > 0.01f)
        {
            RootTransform.rotation = Quaternion.RotateTowards(RootTransform.rotation, finalRotation, RotationSpeed * Time.deltaTime);
            yield return null;
        }
        RootTransform.rotation = finalRotation;

        SetNewStation(positionNodeIdx);
    }

    public void SetNewStation(int stationIdx)
    {
        CurrentNode = stationIdx;
        GestureControl.CharacterAnimator.SetInteger("CurrentStation", CurrentNode);
    }
}

