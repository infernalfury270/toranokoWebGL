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
    public Sprite characterPortraitPlaceholder;
    public List<CharacterSet> characterSets;
}
