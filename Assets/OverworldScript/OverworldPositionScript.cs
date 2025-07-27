using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPositionScript : MonoBehaviour
{
    public CharacterSpeechScript NameSource;
    [HideInInspector] public string CharacterName;

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
            Transform targetNode = TravelNodeTracker.Instance.PositionNodes[positionNodeIdx].transform;

            overworldPositionScript.RootTransform.position = targetNode.position;

            Vector3 currentRotation = overworldPositionScript.RootTransform.rotation.eulerAngles;
            Quaternion finalRotation = Quaternion.Euler(currentRotation.x, targetNode.rotation.eulerAngles.y + 180f, currentRotation.z);
            overworldPositionScript.RootTransform.rotation = finalRotation;
        }
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
        WalkToCoroutine = StartCoroutine(WalkTo(positionNodeIdx));
    }
    public IEnumerator WalkTo(int positionNodeIdx)
    {
        Transform targetNode = TravelNodeTracker.Instance.PositionNodes[positionNodeIdx].transform;
        GestureControl.CharacterAnimator.SetBool("Sitting", false);
        GestureControl.CharacterAnimator.SetBool("Looming", false);
        GestureControl.CharacterAnimator.SetBool("WalkTo", true);

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

        GestureControl.CharacterAnimator.SetBool("WalkTo", false);

        while (Quaternion.Angle(RootTransform.rotation, finalRotation) > 0.01f)
        {
            RootTransform.rotation = Quaternion.RotateTowards(RootTransform.rotation, finalRotation, RotationSpeed * Time.deltaTime);
            yield return null;
        }
        RootTransform.rotation = finalRotation;
        
        CharacterMobile = false;
    }
}
