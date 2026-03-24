using UnityEngine;
using System.Collections.Generic;




#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterInfo))]
public class CharacterInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targ = (CharacterInfo)target;
        var texture1 = AssetPreview.GetAssetPreview(targ.fullBody != null ? targ.fullBody : Global.Instance.placeholderSprite);
        GUILayout.Label(texture1);
        var texture2 = AssetPreview.GetAssetPreview(targ.portrait != null ? targ.portrait : Global.Instance.placeholderSprite);
        GUILayout.Label(texture2);
        if (GUILayout.Button("Refresh"))
        {
            for (int i =0; i < targ.relationships.Count; i++)
            {
                targ.relationships[i].name = targ.relationships[i].character.characterName;
                for (int j = 0; j < targ.relationships[i].stages.Count; j++)
                {
                    targ.relationships[i].stages[j].name = targ.relationships[i].stages[j].storyProgression.ToString();
                }
            }
        }
    }
}
#endif