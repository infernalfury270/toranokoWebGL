using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
public class CharacterData : SingletonScriptableObject<CharacterData>
{
    [System.Serializable]
    public class CharacterSet
    {
        public string name;
        public List<CharacterInfo> characterList;
    }
    [System.Serializable]
    public class RelationshipInfo
    {
        public string name;
        public Sprite sprite;
        public Color color;
    }
    public List<RelationshipInfo> relationshipInfo;
    public List<CharacterSet> characterSets;
}
