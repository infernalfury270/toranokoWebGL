using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CutsceneData", menuName = "Explore/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [System.Serializable]
    public class CutsceneSegment {
        public Sprite background;
        public bool clearBackground;
        public string speakerName;
        public string[] dialogue;
    }
    public List<CutsceneSegment> sequence;
}
