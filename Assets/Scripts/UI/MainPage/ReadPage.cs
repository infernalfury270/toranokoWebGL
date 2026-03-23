using System.Collections;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class ReadPage : MonoBehaviour
{
    [SerializeField] RectTransform _storyListContent, _storyEntryContent;
    [SerializeField] DisableScrollWithHeight _listScroll, _entryScroll;
    [SerializeField] TMP_Text _storyTitle, _storyDesc;
    [SerializeField] Image _storyCover;
    [SerializeField] Button _storyBtn, _storyEntryBtn;
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
        }
        StartCoroutine(RebuildList());
        SelectStory(0);
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
        }
        StartCoroutine(RebuildEntries());
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
//#if UNITY_EDITOR
//        if (Global.Instance.DebugMode)
//            Debug.Log(AssetDatabase.GetAssetPath(file));
//        AssetDatabase.OpenAsset(file);
//#endif
    }
}
