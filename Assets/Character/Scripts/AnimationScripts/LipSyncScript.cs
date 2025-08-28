using System;
using System.Collections;
using System.Linq;
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

public class LipSyncScript : MonoBehaviour
{
    public AudioSource LipSyncAudioSource;
    public SkinnedMeshRenderer TargetMesh;
    public TextAsset PhenomeAsset;
    public float FPS = 24f;

    private string[] PhenomesText;
    private TimeList<PhenomeTypes> PhenomeMarkers = new TimeList<PhenomeTypes>();

    Coroutine SpeechCoroutine = null;

    public CharacterSpeechScript speechScript;

    public void PlaySpeech()
    {
        if(!(SpeechCoroutine is null)) StopCoroutine(SpeechCoroutine);
        SpeechCoroutine = StartCoroutine(PlaySpeechCoroutine());
    }

    private IEnumerator PlaySpeechCoroutine()
    {
        ProcessAudio();

        while (speechScript.isSpeechPlaying())
        {
            yield return null;

            float audioTimeSec = LipSyncAudioSource.time;
            (
                TimeMarker<PhenomeTypes> currentPhenomeMarker, 
                TimeMarker<PhenomeTypes> nextPhenomeMarker
                ) = PhenomeMarkers.GetNearestData(audioTimeSec);

            float transitionLength = nextPhenomeMarker.timeSec - currentPhenomeMarker.timeSec;

            foreach (PhenomeTypes phenomeType in Enum.GetValues(typeof(PhenomeTypes)).Cast<PhenomeTypes>())
            {
                int blendShapeIndex = TargetMesh.sharedMesh.GetBlendShapeIndex(phenomeType.ToString());
                if (blendShapeIndex == -1) continue; //Skip a blend shape if it doesn't exit.

                float phenomeWeight = 0;
                if(phenomeType == currentPhenomeMarker.data && phenomeType == nextPhenomeMarker.data)
                {
                    phenomeWeight = 1;
                }
                else
                {
                    if (phenomeType == currentPhenomeMarker.data)
                    {
                        phenomeWeight = 1f - (audioTimeSec - currentPhenomeMarker.timeSec) / transitionLength;
                    }
                    if (phenomeType == nextPhenomeMarker.data)
                    {
                        phenomeWeight = (audioTimeSec - currentPhenomeMarker.timeSec) / transitionLength;
                    }
                }

                TargetMesh.SetBlendShapeWeight(blendShapeIndex, phenomeWeight *  100f);
            }
        }

        foreach (PhenomeTypes phenomeType in Enum.GetValues(typeof(PhenomeTypes)).Cast<PhenomeTypes>())
        {
            int blendShapeIndex = TargetMesh.sharedMesh.GetBlendShapeIndex(phenomeType.ToString());
            if (blendShapeIndex == -1) continue; //Skip a blend shape if it doesn't exit.

            float phenomeWeight = 0;
            TargetMesh.SetBlendShapeWeight(blendShapeIndex, phenomeWeight * 100f);
        }
    }

    public void ProcessAudio()
    {
        //Removes first line.
        PhenomesText = PhenomeAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string trimmedText = string.Join("\n", PhenomesText.Skip(1));

        //Convert to time markers.
        Func<string, PhenomeTypes> EnumParse = s =>
        {
            return TextToPhenomeType(s);
        };
        PhenomeMarkers = trimmedText.ToTimeMarkers<PhenomeTypes>(" ", EnumParse, FPS);

        //Adjust for rest marker position.
        PhenomeTypes lastType = PhenomeTypes.FV;//Arbitrarily picked non rest type.
        for (int i = 0; i < PhenomeMarkers.Count; i++)
        {
            TimeMarker<PhenomeTypes> marker = PhenomeMarkers[i];
            if (marker.data == PhenomeTypes.rest && lastType == PhenomeTypes.rest)
            {
                marker.timeSec += (-1.5f / FPS);
            }
            //Debug.Log(marker.data.ToString() + " : " + marker.timeSec);
            lastType = marker.data;
        }
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
