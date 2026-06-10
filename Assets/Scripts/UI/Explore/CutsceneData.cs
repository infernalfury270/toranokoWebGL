using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CutsceneData", menuName = "Explore/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    public static System.Action<CutsceneData> OnInvokeCutscene;
    [System.Serializable]
    public class CutsceneSegment {
        public Sprite background;
        public bool clearBackground;
        public string speakerName;
        [TextArea(1,6)]
        public string[] dialogue;
    }
    public List<CutsceneSegment> sequence;
    public void InvokeCutscene() => OnInvokeCutscene?.Invoke(this);
}
