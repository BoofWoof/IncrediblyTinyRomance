using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data
{
    [Serializable]
    public class DSSpeakerInfo
    {
        [field: SerializeField] public CharacterInfo CharacterInfoSO { get; set; }
        [field: SerializeField] public string SpriteUid { get; set; }
        [field: SerializeField] public string NoiseUid { get; set; }

    }

}
