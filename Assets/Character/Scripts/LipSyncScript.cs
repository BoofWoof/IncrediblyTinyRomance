using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

enum PhenomeTypes
{
    AI,
    E,
    U,
    O,
    CDGKNRSThYZ,
    FV,
    L,
    MBP,
    WQ,
    rest
}
struct PhenomeMarker
{
    public PhenomeTypes phenomeType;
    public float timeSec;

    public PhenomeMarker(PhenomeTypes phenomeType, float timeSec)
    {
        this.phenomeType = phenomeType;
        this.timeSec = timeSec;
    }

    public override string ToString()
    {
        return phenomeType.ToString() + " : " + timeSec.ToString();
    }
}
public class LipSyncScript : MonoBehaviour
{
    public AudioSource LipSyncAudioSource;
    public SkinnedMeshRenderer TargetMesh;
    public TextAsset PhenomeAsset;
    public float FPS = 24f;

    private string[] PhenomesText;
    private List<PhenomeMarker> PhenomeMarkers = new List<PhenomeMarker>();

    private int CurrentPhenomeIdx = 0;
    private PhenomeMarker CurrentPhenomeMarker;
    private PhenomeMarker NextPhenomeMarker;

    private void Update()
    {
        CurrentPhenomeIdx = 0;
        if (LipSyncAudioSource.isPlaying)
        {
            float audioTimeSec = LipSyncAudioSource.time;
            for (int i = CurrentPhenomeIdx; i < PhenomeMarkers.Count; i++)
            {
                if(audioTimeSec < PhenomeMarkers[i].timeSec)
                {
                    CurrentPhenomeIdx = i-1;
                    break;
                }
                CurrentPhenomeIdx = i;
            }
            int nextPhenomeIdx = CurrentPhenomeIdx + 1;
            if (nextPhenomeIdx >= PhenomeMarkers.Count) nextPhenomeIdx = CurrentPhenomeIdx;

            CurrentPhenomeMarker = PhenomeMarkers[CurrentPhenomeIdx];
            NextPhenomeMarker = PhenomeMarkers[nextPhenomeIdx];
            float transitionLength = NextPhenomeMarker.timeSec - CurrentPhenomeMarker.timeSec;

            foreach (PhenomeTypes phenomeType in System.Enum.GetValues(typeof(PhenomeTypes)).Cast<PhenomeTypes>())
            {
                int blendShapeIndex = TargetMesh.sharedMesh.GetBlendShapeIndex(phenomeType.ToString());
                if (blendShapeIndex == -1) continue; //Skip a blend shape if it doesn't exit.

                float phenomeWeight = 0;
                if(phenomeType == CurrentPhenomeMarker.phenomeType && phenomeType == NextPhenomeMarker.phenomeType)
                {
                    phenomeWeight = 1;
                }
                else
                {
                    if (phenomeType == CurrentPhenomeMarker.phenomeType)
                    {
                        phenomeWeight = 1f - (audioTimeSec - CurrentPhenomeMarker.timeSec) / transitionLength;
                    }
                    if (phenomeType == NextPhenomeMarker.phenomeType)
                    {
                        phenomeWeight = (audioTimeSec - CurrentPhenomeMarker.timeSec) / transitionLength;
                    }
                }

                TargetMesh.SetBlendShapeWeight(blendShapeIndex, phenomeWeight *  100f);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhenomesText = PhenomeAsset.text.Split('\n');

        for (int i = 1;  i < PhenomesText.Length; i++)
        {
            if (PhenomesText[i].Length < 3) continue;
            PhenomeMarker newPhenomeMarker = TextToPhenomeMarker(PhenomesText[i]);
            Debug.Log(newPhenomeMarker);
            PhenomeMarkers.Add(newPhenomeMarker);
        }
    }

    private PhenomeMarker TextToPhenomeMarker(string text)
    {
        string[] textSplit = text.Split(" ");
        PhenomeTypes phenomeType = TextToPhenomeType(textSplit[1]);
        int phenomeFrame = int.Parse(textSplit[0]) - 1;

        //This line is added because of weird Papagayo behavior.
        if (phenomeType != PhenomeTypes.rest) phenomeFrame += 2;

        float phenomeTimeSec = phenomeFrame / FPS;

        return new PhenomeMarker(
                phenomeType,
                phenomeTimeSec
                );
    }

    private PhenomeTypes TextToPhenomeType(string text)
    {
        switch (text.Trim().ToLower())
        {
            case "ai":
                return PhenomeTypes.AI;
            case "e":
                return PhenomeTypes.E;
            case "u":
                return PhenomeTypes.U;
            case "o":
                return PhenomeTypes.O;
            case "etc":
                return PhenomeTypes.CDGKNRSThYZ;
            case "fv":
                return PhenomeTypes.FV;
            case "l":
                return PhenomeTypes.L;
            case "mbp":
                return PhenomeTypes.MBP;
            case "wq":
                return PhenomeTypes.WQ;
        }
        return PhenomeTypes.rest;
    }
}
