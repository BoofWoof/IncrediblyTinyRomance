using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public OverworldPositionScript overworldPositionScript;
    public Animator animator;

    public HandScript RightHand;
    public HandScript LeftHand;

    public LookScript LookControl;
    private float LastHeadLookWeight;
    private float SetToLookWeight;

    private Transform LastHeadTarget;
    private Transform SetToTarget;

    public void SetLookWeight(float newWeight)
    {
        LastHeadLookWeight = LookControl.HeadLookWeight;
        LookControl.HeadLookWeight = newWeight;
        SetToLookWeight = newWeight;
    }
    public void ResumeLookWeight()
    {
        if (SetToLookWeight != LookControl.HeadLookWeight) return;
        LookControl.HeadLookWeight = LastHeadLookWeight;
    }

    public void FocusLeftSpawnedItem()
    {
        LastHeadTarget = LookControl.Target;
        LookControl.Target = LeftHand.SpawnedObject.transform;
        SetToTarget = LeftHand.SpawnedObject.transform;
    }
    public void ResumeLookTarget()
    {
        if (SetToTarget != LookControl.Target) return;
        LookControl.Target = LastHeadTarget;
    }

    public void Impact(float Strength)
    {
        MoveCamera.moveCamera.ShakeScreen(1f, Strength);
    }

    public void EnableMobility()
    {
        overworldPositionScript.CharacterMobile = true;
    }

    public void HoldCity() //This is now misnamed but annoying to fix. It is now holding anything.
    {
        animator.SetBool("Holding", true);
    }

    public void ReleaseCity() //This is now misnamed but annoying to fix. It is now releasing anything.
    {
        animator.SetBool("Holding", false);
    }

    public void RightHandSpawn(int SpawnID)
    {
        RightHand.SpawnInHand(SpawnID);
    }

    public void RightHandDelete()
    {
        RightHand.DestroyHeldObject();
    }

    public void RightHandRelease(int ReleaseID)
    {
        RightHand.ReleaseHandObject(ReleaseID);
    }

    public void RightHandPickup(string objectName)
    {
        RightHand.PickupHandObject(objectName);
    }

    public void RightHandActivate()
    {
        RightHand.Activate();
    }

    public void LeftHandSpawn(int SpawnID)
    {
        LeftHand.SpawnInHand(SpawnID);
    }

    public void LeftHandDelete()
    {
        LeftHand.DestroyHeldObject();
    }

    public void LeftHandRelease(int ReleaseID)
    {
        LeftHand.ReleaseHandObject(ReleaseID);
    }

    public void LeftHandPickup(string objectName)
    {
        LeftHand.PickupHandObject(objectName);
    }

    public void LeftHandActivate()
    {
        LeftHand.Activate();
    }

    public void LeftHandDropAndActivate(int ReleaseID)
    {
        LeftHand.ReleaseHandObject(ReleaseID);
        LeftHand.Activate();
    }
}
