using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(MainPageManager))]
public class MainPageEditor : Editor
{
    private int _pageToOpen;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _pageToOpen = EditorGUILayout.IntField("To Open", _pageToOpen);
        if (GUILayout.Button("Open Page"))
        {
            ((MainPageManager)target).OpenPage(_pageToOpen);
        }
    }
}
#endif

public class MainPageManager : MonoBehaviour
{
    [SerializeField] List<Page> _pages;
    [SerializeField] RectTransform _contentFrame;
    [SerializeField] float _yOffset = 125;
    private Page _currOpened;
    private void Awake()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            if (_pages == null) continue;
            _pages[i].OnPreload?.Invoke();
            HidePage(_pages[i]);
        }
        OpenPage(0);
    }
    public void OpenPage(int index)
    {
        if (index < 0 || index >= _pages.Count || _pages[index] == null) return;
        if (_currOpened != null)
        {
            _currOpened.OnClose?.Invoke();
            HidePage(_currOpened);
        }
        _currOpened = _pages[index];
        _currOpened.OnOpen?.Invoke();
        ShowPage(_currOpened);
    }
    private void ShowPage(Page page)
    {
        page.canvasGrp.alpha = 1;
        _contentFrame.sizeDelta = new Vector2(_contentFrame.sizeDelta.x, page.rT.sizeDelta.y + _yOffset);
    }
    private void HidePage(Page page)
    {
        page.canvasGrp.alpha = 0;
    }
}
