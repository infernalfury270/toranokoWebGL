using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class DisableScrollWithHeight : MonoBehaviour
{
    bool _hrOG, _vtOG;
    ScrollRect _scrollRect;
    Image _viewport;
    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _hrOG = _scrollRect.horizontal;
        _vtOG = _scrollRect.vertical;
        _viewport = _scrollRect.viewport.GetComponent<Image>();
        ToggleCheck();
    }
    public void ToggleCheck()
    {
        bool horizontal = _scrollRect.horizontalScrollbar == null || _scrollRect.horizontalScrollbar.gameObject.activeSelf;
        bool vertical = _scrollRect.verticalScrollbar == null || _scrollRect.verticalScrollbar.gameObject.activeSelf;
        _scrollRect.horizontal = horizontal ? _hrOG : false;
        _scrollRect.vertical = vertical ? _vtOG : false;
        if (_viewport != null)
            _viewport.raycastTarget = _scrollRect.horizontal || _scrollRect.vertical;
    }
}
