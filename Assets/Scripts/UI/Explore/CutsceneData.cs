using System;
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
    [System.Serializable]
    public class AlternateSequence
    {
        public DayOfWeek dayOfTheWeek;
        public List<CutsceneSegment> sequence;
    }
    public List<AlternateSequence> alternateSequences;
    public void InvokeCutscene() => OnInvokeCutscene?.Invoke(this);
    public List<CutsceneSegment> GetSequence(DateTime dateTime)
    {
        var seq = sequence;
        for (int i = 0; i < alternateSequences.Count; i++)
        {
            if (dateTime.DayOfWeek == alternateSequences[i].dayOfTheWeek)
                seq = alternateSequences[i].sequence;
        }
        return seq;
    }
}
