using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData")]
public class CharacterData : SingletonScriptableObject<CharacterData>
{
    [System.Serializable]
    public class CharacterInfo
    {
        public string name;
        [TextArea(5, 10)]
        public string description;
        public Sprite fullBody, portrait;
    }
    [System.Serializable]
    public class CharacterSet
    {
        public string name;
        public List<CharacterInfo> characterList;
    }
    public List<CharacterSet> characterSets;
}
