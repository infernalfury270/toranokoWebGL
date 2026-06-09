#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CharacterInfo))]
public class CharacterInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var targ = (CharacterInfo)target;
        if (GUILayout.Button("Edit Relationships"))
        {
            CharacterInfoTimeline.EditCharacterRelationships(targ);
        }
        if (GUILayout.Button("Refresh"))
        {
            for (int i = 0; i < targ.relationships.Count; i++)
            {
                targ.relationships[i].name = targ.relationships[i].character.characterName;
                for (int j = 0; j < targ.relationships[i].stages.Count; j++)
                {
                    targ.relationships[i].stages[j].name = targ.relationships[i].stages[j].storyProgression.ToString()
                        + " | " + targ.relationships[i].stages[j].relationship.ToString();
                }
            }
        }
        base.OnInspectorGUI();
        var texture1 = AssetPreview.GetAssetPreview(targ.fullBody != null ? targ.fullBody : Global.Instance.placeholderSprite);
        GUILayout.Label(texture1);
        var texture2 = AssetPreview.GetAssetPreview(targ.portrait != null ? targ.portrait : Global.Instance.placeholderSprite);
        GUILayout.Label(texture2);
        if (GUILayout.Button("Refresh"))
        {
            for (int i = 0; i < targ.relationships.Count; i++)
            {
                targ.relationships[i].name = targ.relationships[i].character.characterName;
                for (int j = 0; j < targ.relationships[i].stages.Count; j++)
                {
                    targ.relationships[i].stages[j].name = targ.relationships[i].stages[j].storyProgression.ToString() 
                        + " | " + targ.relationships[i].stages[j].relationship.ToString();
                }
            }
        }
    }
}

public class CharacterInfoTimeline : EditorWindow
{
    public static CharacterInfo selectedInfo;
    [MenuItem("Window/UI Toolkit/CharacterInfoTimeline")]
    public static void ShowExample()
    {
        CharacterInfoTimeline wnd = GetWindow<CharacterInfoTimeline>();
        wnd.titleContent = new GUIContent("CharacterInfoTimeline");
    }

    public static void EditCharacterRelationships(CharacterInfo info)
    {
        selectedInfo = info;
        CharacterInfoTimeline wnd = GetWindow<CharacterInfoTimeline>();
        wnd.Close();
        wnd = GetWindow<CharacterInfoTimeline>();
        wnd.titleContent = new GUIContent(selectedInfo.characterName + "'s Relationships");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        if (selectedInfo == null)
        {
            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("No character selected.");
            root.Add(label);
            return;
        }
        VisualElement label2 = new Label(selectedInfo.characterName);
        root.Add(label2);
        return;
    }
}
#endif