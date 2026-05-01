using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class ReadPage : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] CanvasGroup _storyGrp;
    [SerializeField] RectTransform _storyGridContent;
    [SerializeField] RectTransform _storyGridEntry;
    [SerializeField] GridLayoutGroup _layout;
    [SerializeField] DisableScrollWithHeight _gridScroll;
    [Header("Read")]
    [SerializeField] CanvasGroup _readGrp;
    [SerializeField] RectTransform _storyListContent, _storyEntryContent;
    [SerializeField] DisableScrollWithHeight _listScroll, _entryScroll;
    [SerializeField] TMP_Text _storyTitle, _storyDesc;
    [SerializeField] Image _storyCover;
    [SerializeField] Button _storyBtn, _storyEntryBtn;

    private static readonly float _themeInterpolTime = 0.5f;
    private static readonly float _switchTime = 0.25f;
    private int _switchCount;
    private bool _readActive;
    private LibraryData.Book.THEME _currTheme;
    [System.Serializable]
    public class ThemeColourClass
    {
        public Color[] color;
        public List<Image> assets;
    }
    [System.Serializable]
    public class ThemePropGroup
    {
        public List<CanvasGroup> toHide; // on by default
        public List<CanvasGroup> toShow;
    }
    [SerializeField] List<ThemeColourClass> _colourClasses;
    [SerializeField] List<ThemePropGroup> _propGroups;

    private List<CanvasGroup> _toHideOnSwitch;
    private List<CanvasGroup> _toRevealOnSwitch;
    int _entriesPerRow;
    private void Awake()
    {
        //_entriesPerRow = (int)Mathf.Ceil((_charContent.rect.width - _charLayout.padding.left - _charLayout.padding.right) 
        //    / (_charLayout.cellSize.x + _charLayout.spacing.x));
        //Debug.Log(_entriesPerRow);
        _toHideOnSwitch = new();
        _toRevealOnSwitch = new();
        for (int i = 0; i < _propGroups[0].toShow.Count; i++)
            _toHideOnSwitch.Add(_propGroups[0].toShow[i]);
        for (int i = 0; i < _propGroups[0].toHide.Count; i++)
            _toRevealOnSwitch.Add(_propGroups[0].toHide[i]);
        _currTheme = LibraryData.Book.THEME.DEFAULT;
        _entriesPerRow = 3;
        _switchCount = 0;
    }
    public void LoadStories()
    {
        for (int i = _storyListContent.childCount - 1; i >= 0; i--)
            Destroy(_storyListContent.GetChild(i).gameObject);
        for (int i = _storyEntryContent.childCount - 1; i >= 0; i--)
            Destroy(_storyEntryContent.GetChild(i).gameObject);
        for (int i = 0; i < LibraryData.Instance.books.Count; i++) {
            var newBtn = Instantiate(_storyBtn, _storyListContent);
            int index = i;
            newBtn.GetComponentInChildren<TMP_Text>().text = LibraryData.Instance.books[index].name;
            newBtn.onClick.AddListener(() => SelectStory(index));
            _colourClasses[4].assets.Add(newBtn.GetComponent<Image>());
        }
        StartCoroutine(RebuildList());
    }
    private void SelectStory(int index)
    {
        if (index < 0 || index >= LibraryData.Instance.books.Count) return;
        for (int i = _storyEntryContent.childCount - 1; i >= 0; i--)
            Destroy(_storyEntryContent.GetChild(i).gameObject);
        var book = LibraryData.Instance.books[index];
        _storyTitle.text = book.name;
        _storyDesc.text = book.description;
        _storyCover.sprite = book.cover;

        for (int i = 0; i < book.chapters.Count; i++) {
            var chpt = book.chapters[i];
            var newEntry = Instantiate(_storyEntryBtn, _storyEntryContent);
            newEntry.transform.Find("EntryTitle").GetComponent<TMP_Text>().text = chpt.name;
            newEntry.transform.Find("EntryDate").GetComponent<TMP_Text>().text = chpt.dateAdded;
            if (!string.IsNullOrEmpty(chpt.fileUrl))
            {
                newEntry.onClick.AddListener(() => OpenFile(chpt.fileUrl));
            }
            _colourClasses[4].assets.Add(newEntry.GetComponent<Image>());
        }
        StartCoroutine(RebuildEntries());
        ThemeSwitch(book.theme);
    }
    private IEnumerator RebuildList()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_storyListContent);
        yield return new WaitForEndOfFrame();
        _storyListContent.sizeDelta = new Vector2(_storyListContent.sizeDelta.x, _storyBtn.GetComponent<RectTransform>().sizeDelta.y * _storyListContent.childCount);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _listScroll.ToggleCheck();
    }
    private IEnumerator RebuildEntries()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_storyEntryContent);
        yield return new WaitForEndOfFrame();
        _storyEntryContent.sizeDelta = new Vector2(_storyEntryContent.sizeDelta.x, _storyEntryBtn.GetComponent<RectTransform>().sizeDelta.y * _storyEntryContent.childCount);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _entryScroll.ToggleCheck();
    }
    private void OpenFile(string fileUrl)
    {
        Application.OpenURL(fileUrl);
    }
    public void SwitchPages(bool isRead)
    {
        _readActive = isRead;
        CanvasGroup activeGrp = _readActive ? _readGrp : _storyGrp;
        CanvasGroup inactiveGrp = _readActive ? _storyGrp : _readGrp;
        activeGrp.blocksRaycasts = true;
        inactiveGrp.blocksRaycasts = false;
        StartCoroutine(InterpolatePage(activeGrp, inactiveGrp));
    }
    private IEnumerator InterpolatePage(CanvasGroup active, CanvasGroup inactive)
    {
        bool readCheck = _readActive;
        float t = 0;
        while (t <= _switchTime)
        {
            if (readCheck != _readActive) yield break;
            float alphaStep = Time.deltaTime / _switchTime;
            active.alpha += alphaStep;
            inactive.alpha -= alphaStep;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    public void LoadStoryGrid()
    {
        for (int i = _storyGridContent.childCount - 1; i >= 0; i--)
            Destroy(_storyGridContent.GetChild(i).gameObject);
        for (int i = 0; i < LibraryData.Instance.books.Count; i++)
        {
            var newEntry = Instantiate(_storyGridEntry, _storyGridContent);
            int index = i;
            Button cover = newEntry.Find("Frame").Find("Cover").GetComponent<Button>();
            _colourClasses[1].assets.Add(newEntry.Find("Frame").Find("Backing").GetComponent<Image>());
            cover.GetComponent<Image>().sprite = LibraryData.Instance.books[index].cover;
            newEntry.GetComponentInChildren<TMP_Text>().text = LibraryData.Instance.books[index].name;
            cover.onClick.AddListener(delegate
            {
                SwitchPages(true);
                SelectStory(index);
            });
        }
        StartCoroutine(RebuildGrid());
    }
    private IEnumerator RebuildGrid()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_storyGridContent);
        yield return new WaitForEndOfFrame();
        int rows = (int)Mathf.Ceil((float)_storyGridContent.childCount / _entriesPerRow);
        _storyGridContent.sizeDelta = new Vector2(_storyGridContent.sizeDelta.x, (_layout.cellSize.y + _layout.spacing.y) * rows);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _gridScroll.ToggleCheck();
    }
    public void ThemeSwitch(LibraryData.Book.THEME theme)
    {
        if (theme == _currTheme) return;
        _currTheme = theme;
        _switchCount++;
        if (_colourClasses == null) _colourClasses = new();
        for (int i = 0; i < _colourClasses.Count; i++) { 
            if (_colourClasses[i].assets.Count == 0 || _colourClasses[i].color.Length == 0) continue;
            int colIndex = (int)theme;
            if (colIndex >= _colourClasses[i].color.Length)
                colIndex = 0;
            Color newCol = _colourClasses[i].color[colIndex];
            Color ogCol = _colourClasses[i].assets[0].color;
            StartCoroutine(InterpolateColor(ogCol, newCol, _colourClasses[i]));
        }
        ThemePropGroup propGrp = _propGroups[(int)theme];
        StartCoroutine(InterpolateProps(propGrp));
    }
    public void RevertTheme() => ThemeSwitch(LibraryData.Book.THEME.DEFAULT);
    private IEnumerator InterpolateColor(Color ogCol, Color newCol, ThemeColourClass colourCls)
    {
        float t = 0;
        int ind = _switchCount;
        while (t <= _themeInterpolTime)
        {
            if (_switchCount != ind) yield break;
            t += Time.deltaTime;
            float a = t/_themeInterpolTime;
            for (int i = 0; i < colourCls.assets.Count; i++)
            {
                if (colourCls.assets[i] == null) continue;
                colourCls.assets[i].color = Color.Lerp(ogCol, newCol, a);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator InterpolateProps(ThemePropGroup newPropGrp)
    {
        List<CanvasGroup> newHides = new List<CanvasGroup>();
        List<CanvasGroup> newReveals = new List<CanvasGroup>();
        for (int i = 0; i < newPropGrp.toShow.Count; i++)
        {
            if (_toHideOnSwitch.Contains(newPropGrp.toShow[i]))
                _toHideOnSwitch.Remove(newPropGrp.toShow[i]);
            newHides.Add(newPropGrp.toShow[i]);
        }
        for (int i = 0; i < newPropGrp.toHide.Count; i++)
        {
            if (_toRevealOnSwitch.Contains(newPropGrp.toHide[i]))
                _toRevealOnSwitch.Remove(newPropGrp.toHide[i]);
            newReveals.Add(newPropGrp.toHide[i]);
        }
        float t = 0;
        int ind = _switchCount;
        while (t <= _themeInterpolTime)
        {
            if (_switchCount != ind)
            {
                for (int i = 0; i < newHides.Count; i++)
                    if (!_toHideOnSwitch.Contains(newHides[i]))
                        _toHideOnSwitch.Add(newHides[i]);
                for (int i = 0; i < newReveals.Count; i++)
                    if (!_toRevealOnSwitch.Contains(newReveals[i]))
                        _toRevealOnSwitch.Add(newReveals[i]);
                yield break;
            }
            t += Time.deltaTime;
            float alphaStep = Time.deltaTime / _themeInterpolTime;
            if (_toHideOnSwitch.Count > 0)
            {
                for (int i =  _toHideOnSwitch.Count - 1; i >= 0; i--)
                {
                    _toHideOnSwitch[i].alpha -= alphaStep;
                    if (_toHideOnSwitch[i].alpha == 0)
                        _toHideOnSwitch.RemoveAt(i);
                }
            }
            if (_toRevealOnSwitch.Count > 0)
            {
                for (int i = _toRevealOnSwitch.Count - 1; i >= 0; i--)
                {
                    _toRevealOnSwitch[i].alpha += alphaStep;
                    if (_toRevealOnSwitch[i].alpha == 1)
                        _toRevealOnSwitch.RemoveAt(i);
                }
            }
            if (newHides.Count > 0)
            {
                for (int i = 0; i < newHides.Count; i++)
                {
                    if (newHides[i].alpha == 1) continue;
                    newHides[i].alpha += alphaStep;
                }
            }
            if (newReveals.Count > 0)
            {
                for (int i = 0; i < newReveals.Count; i++)
                {
                    if (newReveals[i].alpha == 0) continue;
                    newReveals[i].alpha -= alphaStep;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        _toHideOnSwitch = newHides;
        _toRevealOnSwitch = newReveals;
    }
}
