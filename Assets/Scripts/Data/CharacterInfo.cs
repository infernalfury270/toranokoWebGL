using UnityEngine;
using UnityEngine.Purchasing;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    [TextArea(5, 10)]
    public string description;
    public Sprite fullBody, portrait;
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
    }
}
#endif