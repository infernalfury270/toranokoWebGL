using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class MonoBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        bool allAssignables = true;
        for (int i = 0; i < targets.Length; i++) {
            if (!(targets[i] is IAssignable))
            {
                allAssignables = false;
                break;
            }
        }
        if (allAssignables)
        {
            if (GUILayout.Button("Assign"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    ((IAssignable)targets[i]).Assign();
                }
            }
        }
    }
}
#endif