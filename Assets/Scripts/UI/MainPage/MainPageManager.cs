using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;








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
            ((MainPageManager)target).HideAllPages();
            ((MainPageManager)target).OpenPage(_pageToOpen);
        }
    }
}
#endif

public class MainPageManager : MonoBehaviour//, IPointerClickHandler
{
    [SerializeField] Scrollbar _mainScrollbar;
    [SerializeField] List<Page> _pages;
    [SerializeField] RectTransform _contentFrame;
    [SerializeField] float _yOffset = 125;
    [SerializeField] RectTransform _particleRect;
    [SerializeField] ParticleSystem _clickParticle;
    [SerializeField] Canvas _parentCanvas;
    [SerializeField] Animator _transition;
    [SerializeField] AudioSource _exploreEnterSFX;

    private InputAction _action;
    private static readonly float _transTime = 0.25f;
    private Page _currOpened;
    private int _transCount;
    private void Awake()
    {
        _action = InputSystem.actions.FindAction("Click");
        _transCount = 0;
        _mainScrollbar.value = 1;
        for (int i = 0; i < _pages.Count; i++)
        {
            if (_pages == null) continue;
            _pages[i].OnPreload?.Invoke();
            HidePage(_pages[i]);
        }
        _currOpened = _pages[0];
        _currOpened.OnOpen?.Invoke();
        ShowPage(_currOpened);
    }
    private void Update()
    {
        if (_action.WasPressedThisFrame())
        {
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
            {
                var screenpoint = Mouse.current.position.ReadValue();
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform,
                    screenpoint,
                    null,
                    out localPoint
                );
                _particleRect.anchoredPosition = localPoint;
                _clickParticle.Emit(3);
            }
        }
    }
    public void OpenPage(int index)
    {
        if (index < 0 || index >= _pages.Count || _pages[index] == null || _currOpened == _pages[index]) return;
        _transCount++;
        if (_currOpened != null)
        {
            _currOpened.OnClose?.Invoke();
            HidePage(_currOpened, true);
        }
        _currOpened = _pages[index];
        _currOpened.OnOpen?.Invoke();
        ShowPage(_currOpened, true);
    }
    private void ShowPage(Page page, bool transition = false)
    {
        page.canvasGrp.blocksRaycasts = true;
        _contentFrame.sizeDelta = new Vector2(_contentFrame.sizeDelta.x, page.rT.sizeDelta.y + _yOffset);
        if (Application.isPlaying && transition)
        {
            StartCoroutine(TransitionPage(page, true));
        } else
        {
            page.canvasGrp.alpha = 1;
        }
    }
    private IEnumerator TransitionPage(Page page, bool transIn)
    {
        float t = 0;
        int ind = _transCount;
        while (t <= _transTime)
        {
            if (ind != _transCount)
            {
                yield break;
            }
            float a = Time.deltaTime / _transTime;
            if (transIn)
                page.canvasGrp.alpha += a;
            else
                page.canvasGrp.alpha -= a;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    public void HideAllPages()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            if (_pages == null) continue;
            HidePage(_pages[i]);
        }
    }
    private void HidePage(Page page, bool transition = false)
    {
        page.canvasGrp.blocksRaycasts = false;
        if (Application.isPlaying && transition)
        {
            StartCoroutine(TransitionPage(page, false));
        }
        else
        {
            page.canvasGrp.alpha = 0;
        }
    }

    public void ToExploreScene() => StartCoroutine(ToExplore());
    private IEnumerator ToExplore() {
        _transition.Play("FootstepRecover");
        _exploreEnterSFX.Play();
        yield return new WaitForEndOfFrame();
        while (_transition.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("ExploreScene");
    }
    public void ToOGWebsite() => Application.OpenURL("https://infernalfury270.github.io/toranoko/");
    public void OpenLink(string link) => Application.OpenURL(link);

//    public void OnPointerClick(PointerEventData eventData)
//    {
//#if UNITY_EDITOR
//        if (Global.Instance.DebugMode)
//            Debug.Log(eventData.pointerClick);
//#endif
//    }
}
