using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OverworldPositionScript : MonoBehaviour
{
    public CharacterSpeechScript NameSource;
    [HideInInspector] public string CharacterName;

    public int InitialNodeValue = 6;
    //public int CurrentStation { get; set; }
    public int CurrentStation;

    public Transform CharacterTransform;
    public GestureScript GestureControl;
    public bool CharacterMobile = false;

    public float Speed = 1f;
    public float RotationSpeed = 360f;

    public static int WaitStation = -1;

    public static List<OverworldPositionScript> PositionScripts = new List<OverworldPositionScript>();

    [HideInInspector] public Coroutine WalkToCoroutine;

    public void Start()
    {
        CharacterName = NameSource.SpeakerName;
        PositionScripts.Add(this);

        SetNewStation(InitialNodeValue);
        GoTo(CurrentStation);

        Lua.RegisterFunction("SetWaitStation", null, SymbolExtensions.GetMethodInfo(() => SetWaitStation(0)));
        Lua.RegisterFunction("StartWalkTo", null, SymbolExtensions.GetMethodInfo(() => StartWalkTo("Name", 0.0f)));
        Lua.RegisterFunction("StartWaitWalkTo", null, SymbolExtensions.GetMethodInfo(() => StartWaitWalkTo("Name", 0.0f, 0.0f)));
    }

    public static void SetWaitStation(float StationIdx)
    {
        WaitStation = (int)StationIdx;

        foreach (OverworldPositionScript overworldPositionScript in PositionScripts)
        {
            if (overworldPositionScript.CurrentStation == WaitStation)
            {
                WaitStation = -1;
                (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
            }
        }
    }

    public static void GoTo(string name, int CurrentStationIdx)
    {
        foreach (OverworldPositionScript overworldPositionScript in PositionScripts)
        {
            if (overworldPositionScript.CharacterName != name && overworldPositionScript.NameSource.NickName != name) continue;
            Debug.Log("Teleporting: " + name);
            overworldPositionScript.GoTo(CurrentStationIdx);
        }
    }
    public void GoTo(int CurrentStationIdx, bool flip = false, bool forceAngle = true)
    {
        Transform targetNode = TravelNodeTracker.Instance.PositionNodes[CurrentStationIdx].transform;

        CharacterTransform.SetPositionAndRotation(targetNode.position, CharacterTransform.rotation);

        if (forceAngle)
        {
            Vector3 currentRotation = CharacterTransform.rotation.eulerAngles;
            float yRot = targetNode.rotation.eulerAngles.y + 180f;
            if (flip) yRot -= 180f;
            Quaternion finalRotation = Quaternion.Euler(currentRotation.x, yRot, currentRotation.z);
            CharacterTransform.rotation = finalRotation;
        }

        SetNewStation(CurrentStationIdx);
    }

    public static void StartWalkTo(string name, float CurrentStationIdx)
    {
        StartWalkTo(name, (int)CurrentStationIdx);
    }
    public static void StartWaitWalkTo(string name, float CurrentStationIdx, float Wait)
    {
        StartWalkTo(name, (int)CurrentStationIdx, Wait);
    }

    public static void StartWalkTo(string name, int CurrentStationIdx, float Wait = 0f)
    {
        foreach (OverworldPositionScript overworldPositionScript in PositionScripts)
        {
            if (overworldPositionScript.CharacterName != name && overworldPositionScript.NameSource.NickName != name) continue;
            overworldPositionScript.StartWalkTo(CurrentStationIdx, Wait);
        }
    }

    public void StartWalkTo(int CurrentStationIdx, float Wait = 0f)
    {
        Debug.Log("AAAAAAAAA");
        WalkToCoroutine = StartCoroutine(FollowRouteTo(CurrentStationIdx, Wait));
    }
    public IEnumerator FollowRouteTo(int CurrentStationIdx, float Wait = 0f)
    {
        yield return new WaitForSeconds(Wait);

        GestureControl.CharacterAnimator.SetBool("Sitting", false);
        GestureControl.CharacterAnimator.SetBool("Looming", false);
        GestureControl.CharacterAnimator.SetBool("WalkTo", true);

        OverworldRoute route = new OverworldRoute();
        route.Initialize();
        route.RouteIdxs.Add(CurrentStation);
        bool routeFound;
        (routeFound, route) = TravelNodeTracker.Instance.FindShortestRoute(route, CurrentStationIdx);
        if (!routeFound) Debug.LogError("No route found.");

        int prev_idx = route.RouteIdxs[0];
        route.RouteIdxs.RemoveAt(0); //Remove Initial Node

        foreach (int idx in route.RouteIdxs)
        {
            TravelNodeConnection connection = TravelNodeTracker.Instance.FindConnection(prev_idx, idx);

            if (connection.ExitWithAnimation)
            {
                yield return StartCoroutine(WaitForMobility(idx, connection));
            }
            else if(connection.EnterWithAnimation)
            {
                yield return StartCoroutine(WaitForAnimation(idx, connection));
            } else
            {
                yield return StartCoroutine(WalkTo(idx, connection));
            }
            prev_idx = idx;
        }

        CharacterMobile = false;
        GestureControl.CharacterAnimator.SetBool("WalkTo", false);
    }

    public IEnumerator WaitForAnimation(int CurrentStationIdx, TravelNodeConnection connection)
    {
        Animator animator = GestureControl.CharacterAnimator;
        GestureControl.CharacterAnimator.SetTrigger("ActivateTransition");

        AnimatorStateInfo stateInfo = GestureControl.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        int startingStateHash = stateInfo.shortNameHash;

        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
        {
            yield return null;
        }

        stateInfo = GestureControl.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        startingStateHash = stateInfo.shortNameHash;

        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
        {
            yield return null;
        }

        GoTo(connection.TravelNode.SceneID, connection.FlipAngle);
    }

    public IEnumerator WaitForMobility(int CurrentStationIdx, TravelNodeConnection connection)
    {
        CharacterMobile = false;
        while (!CharacterMobile)
        {
            yield return null;
        }

        GoTo(connection.TravelNode.SceneID, connection.FlipAngle);
    }
    public IEnumerator WalkTo(int CurrentStationIdx, TravelNodeConnection connection)
    {
        TravelNodeScript targetNodeScript = TravelNodeTracker.Instance.PositionNodes[CurrentStationIdx].GetComponent<TravelNodeScript>();
        Transform targetNode = targetNodeScript.transform;

        while (!CharacterMobile)
        {
            yield return null;
        }

        Vector3 currentRotation = CharacterTransform.rotation.eulerAngles;

        Vector3 direction = CharacterTransform.position-targetNode.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation = Quaternion.Euler(currentRotation.x, targetRotation.eulerAngles.y, currentRotation.z);

        Quaternion finalRotation = Quaternion.Euler(currentRotation.x, targetNode.rotation.eulerAngles.y + 180f, currentRotation.z);

        while (Vector3.Magnitude(CharacterTransform.position - targetNode.position) > 0.01f)
        {
            CharacterTransform.position = Vector3.MoveTowards(CharacterTransform.position, targetNode.position, Speed * Time.deltaTime);
            CharacterTransform.rotation = Quaternion.RotateTowards(CharacterTransform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            yield return null;
        }
        CharacterTransform.position = targetNode.position;
        CharacterTransform.rotation = targetRotation;

        if (connection.ForceAngleOnArrival)
        {
            while (Quaternion.Angle(CharacterTransform.rotation, finalRotation) > 0.01f)
            {
                CharacterTransform.rotation = Quaternion.RotateTowards(CharacterTransform.rotation, finalRotation, RotationSpeed * Time.deltaTime);
                yield return null;
            }
            CharacterTransform.rotation = finalRotation;
        }

        GoTo(connection.TravelNode.SceneID, connection.FlipAngle, connection.ForceAngleOnArrival);
    }

    public void SetNewStation(int stationIdx)
    {
        CurrentStation = stationIdx;
        GestureControl.CharacterAnimator.SetInteger("CurrentStation", CurrentStation);

        if(CurrentStation == WaitStation)
        {
            WaitStation = -1;
            (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
        }
    }
}

