using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    [System.Serializable]
    public class CharacterRelationship
    {
        [System.Serializable]
        public class RelationshipStage
        {
            public enum RELATIONSHIP
            {
                NEUTRAL,
                FRIEND_ACQUAINTANCE,
                FRIEND_FRIENDLY,
                FRIEND_PLEASANT,
                FRIEND_CLOSE,
                FRIEND_BEST,
                ENEMY_UNEASY,
                ENEMY_IRRITATION,
                ENEMY_ANGER,
                ENEMY_HATE,
                ENEMY_DESPISE,
                LOVE_CONSIDERING,
                LOVE_CONFIDENT,
                LOVE_YEARNING,
                LOVE_INTENSE,
                LOVE_EXTREME, // basically married
                LOVE_UNCERTAIN,
                LOVE_HEARTBREAK,
                LOVE_AGONY,
                LOVE_ABUSIVE,
                LOVE_DESTROYED,
            }
#if UNITY_EDITOR
            [HideInInspector] public string name;
#endif
            [Range(0,300)]
            public int storyProgression;
            public RELATIONSHIP relationship;
            public string comment;
        }
#if UNITY_EDITOR
        [HideInInspector] public string name;
#endif
        public CharacterInfo character;
        public List<RelationshipStage> stages;
    }

    public string characterName;
    [TextArea(5, 10)]
    public string description;
    public Sprite fullBody, portrait;
    public List<CharacterRelationship> relationships;
}