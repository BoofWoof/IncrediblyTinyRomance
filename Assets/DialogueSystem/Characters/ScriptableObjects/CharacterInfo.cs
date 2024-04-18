using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Custom/Make New Character")]
public class CharacterInfo : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private Sprite defaultCharacterSprite;
    [SerializeField] private AudioClip defaultCharacterNoise;

    [SerializeField] public List<string> spriteUuid;
    [SerializeField] public List<string> emotionSpriteNames;
    [SerializeField] public List<Sprite> emotionSprites;

    [SerializeField] public List<string> noiseUuid;
    [SerializeField] public List<string> emotionNoiseNames;
    [SerializeField] public List<AudioClip> emotionNoises;
}
