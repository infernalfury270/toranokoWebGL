using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterPage : MonoBehaviour
{
    [SerializeField] CanvasGroup _setGrp, _infoGrp;
    [SerializeField] GridLayoutGroup _charLayout;
    [SerializeField] DisableScrollWithHeight _setScroll, _charScroll;
    [SerializeField] Button _setBtn, _charBtn;
    [SerializeField] RectTransform _setContent, _charContent;
    [SerializeField] TMP_Text _setTitle, _charName, _charDesc, _prevChar, _nextChar;
    [SerializeField] Image _charFullBody, _charPortrait;

    [SerializeField] TMP_Text _relationshipTitle;
    [SerializeField] DisableScrollWithHeight _relationshipScroll;
    [SerializeField] RectTransform _relationshipContent, _relationshipDisplay;
    [SerializeField] Slider _timelineSlider;

    int _entriesPerRow;
    int _currSet;
    int _charIndex;
    CharacterInfo _currCharacter;
    int _lastPeriodA, _lastPeriodB;
    private void Awake()
    {
        //_entriesPerRow = (int)Mathf.Ceil((_charContent.rect.width - _charLayout.padding.left - _charLayout.padding.right) 
        //    / (_charLayout.cellSize.x + _charLayout.spacing.x));
        //Debug.Log(_entriesPerRow);
        _entriesPerRow = 3;
        _lastPeriodA = _lastPeriodB = -1;
    }
    private IEnumerator RebuildSets()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_setContent);
        yield return new WaitForEndOfFrame();
        _setContent.sizeDelta = new Vector2(_setContent.sizeDelta.x, _setBtn.GetComponent<RectTransform>().sizeDelta.y * _setContent.childCount);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _setScroll.ToggleCheck();
    }
    private IEnumerator RebuildEntries()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_charContent);
        yield return new WaitForEndOfFrame();
        int rows = (int)Mathf.Ceil((float)_charContent.childCount / _entriesPerRow);
        _charContent.sizeDelta = new Vector2(_charContent.sizeDelta.x, (_charLayout.cellSize.y + _charLayout.spacing.y) * rows);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _charScroll.ToggleCheck();
    }
    private IEnumerator RebuildRelationships()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_relationshipContent);
        yield return new WaitForEndOfFrame();
        _relationshipContent.sizeDelta = new Vector2(_relationshipContent.sizeDelta.x, _relationshipDisplay.sizeDelta.y * _relationshipContent.childCount);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _relationshipScroll.ToggleCheck();
    }
    public void LoadSets()
    {
        for (int i = _setContent.childCount - 1;  i >= 0; i--)
            Destroy(_setContent.GetChild(i).gameObject);
        for (int i = 0; i < CharacterData.Instance.characterSets.Count; i++)
        {
            var set = Instantiate(_setBtn, _setContent);
            set.GetComponentInChildren<TMP_Text>().text = CharacterData.Instance.characterSets[i].name;
            int index = i;
            set.onClick.AddListener(() => SelectSet(index));
        }
        StartCoroutine(RebuildSets());
        SelectSet(0);
        ToggleInfo(false);
        _timelineSlider.onValueChanged.AddListener(UpdateRelationships);
    }
    public void SelectSet(int index)
    {
        for (int i = _charContent.childCount - 1; i >= 0; i--)
            Destroy(_charContent.GetChild(i).gameObject);
        _currSet = index;
        var set = CharacterData.Instance.characterSets[index];
        for (int i = 0; i < set.characterList.Count; i++)
        {
            var entry = Instantiate(_charBtn, _charContent);
            entry.GetComponentInChildren<TMP_Text>().text = set.characterList[i].characterName;
            Sprite portrait = set.characterList[i].portrait;
            if (portrait != null) {
                entry.transform.Find("CharacterPortrait").GetComponent<Image>().sprite = portrait;
            }
            int ind = i;
            entry.onClick.AddListener(delegate
            {
                SelectCharacter(ind);
                ToggleInfo(true);
            });
        }
        _setTitle.text = set.name;
        StartCoroutine(RebuildEntries());
    }
    public void SelectCharacter(int index)
    {
        _charIndex = index;
        CharacterInfo character = CharacterData.Instance.characterSets[_currSet].characterList[_charIndex];
        _charName.text = character.characterName;
        _charDesc.text = character.description;
        _charFullBody.sprite = character.fullBody != null ? character.fullBody : Global.Instance.placeholderSprite;
        _charPortrait.sprite = character.portrait != null ? character.portrait : Global.Instance.placeholderSprite;
        _prevChar.text = GetPrevCharacter().characterName;
        _nextChar.text = GetNextCharacter().characterName;
        LoadRelationships(character);
    }
    public void LoadRelationships(CharacterInfo character)
    {
        _currCharacter = character;
        _relationshipTitle.text = _currCharacter.characterName + "'s Relationships";
        _lastPeriodA = _lastPeriodB = -1;
        UpdateRelationships(_timelineSlider.value);
    }
    public void UpdateRelationships(float value)
    {
        if (_currCharacter == null) return;
        int point = (int)Mathf.Round(value * 300);
        if (point >= _lastPeriodA && point < _lastPeriodB) return;
        _lastPeriodA = -1;
        _lastPeriodB = 301;
        List<int> periodEnds = new();
        for (int i = _relationshipContent.childCount -1; i >= 0; i--)
            Destroy(_relationshipContent.GetChild(i).gameObject);
        for (int i = 0; i < _currCharacter.relationships.Count; i++)
        {
            var charRelation = _currCharacter.relationships[i];
            if (charRelation.stages.Count == 0) continue;
            var newEntry = Instantiate(_relationshipDisplay, _relationshipContent);
            newEntry.Find("Self").Find("Icon").GetComponent<Image>().sprite = _currCharacter.portrait;
            newEntry.Find("SelfName").GetComponentInChildren<TMP_Text>().text = _currCharacter.characterName;
            if (charRelation.character.portrait != null)
                newEntry.Find("Other").Find("Icon").GetComponent<Image>().sprite = charRelation.character.portrait;
            newEntry.Find("OtherName").GetComponentInChildren<TMP_Text>().text = charRelation.character.characterName;
            newEntry.Find("Scribble").localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            TMP_Text title = newEntry.Find("Title").GetComponent<TMP_Text>();
            TMP_Text comment = newEntry.Find("Comment").GetComponent<TMP_Text>();
            Image relationIcon = newEntry.Find("RelationCircle").Find("Relationship").GetComponent<Image>();
            for (int j = 0; j < charRelation.stages.Count; j++)
            {
                var stage = charRelation.stages[j];
                if (point >= stage.storyProgression)
                {
                    var info = CharacterData.Instance.relationshipInfo[(int)stage.relationship];
                    title.text = info.name;
                    title.color = info.color;
                    if (string.IsNullOrEmpty(stage.comment))
                        comment.text = "\"...\"";
                    else
                        comment.text = "\"" + stage.comment + "\"";
                    relationIcon.sprite = info.sprite;
                    if (stage.storyProgression > _lastPeriodA)
                        _lastPeriodA = stage.storyProgression;
                } else
                {
                    periodEnds.Add(stage.storyProgression);
                    break;
                }
            }
        }
        for (int i = 0; i < periodEnds.Count; i++)
        {
            if (periodEnds[i] < _lastPeriodB && periodEnds[i] > _lastPeriodA)
                _lastPeriodB = periodEnds[i];
        }
        StartCoroutine(RebuildRelationships());
    }
    private int IncrementSetIndex(int curr, int inc)
    {
        int newSet = curr + inc;
        if (newSet < 0) newSet = CharacterData.Instance.characterSets.Count - 1;
        else if (newSet >= CharacterData.Instance.characterSets.Count) newSet = 0;
        return newSet;
    }
    private int[] IncrementCharIndex(int inc)
    {
        int newSet = _currSet;
        int newIndex = _charIndex + inc;
        if (newIndex < 0)
        {
            newSet = IncrementSetIndex(newSet, -1);
            newIndex = CharacterData.Instance.characterSets[newSet].characterList.Count - 1;
        }
        else if (newIndex >= CharacterData.Instance.characterSets[newSet].characterList.Count)
        {
            newSet = IncrementSetIndex(newSet, 1);
            newIndex = 0;
        }
        return new int[] { newIndex, newSet};
    }
    public void ToggleInfo(bool isActive)
    {
        _setGrp.alpha = isActive ? 0 : 1;
        _setGrp.blocksRaycasts = !isActive;
        _infoGrp.alpha = isActive ? 1 : 0;
        _infoGrp.blocksRaycasts = isActive;
    }
    private CharacterInfo GetPrevCharacter()
    {
        int[] charIndex = IncrementCharIndex(-1);
        return CharacterData.Instance.characterSets[charIndex[1]].characterList[charIndex[0]];
    }
    private CharacterInfo GetNextCharacter()
    {
        int[] charIndex = IncrementCharIndex(1);
        return CharacterData.Instance.characterSets[charIndex[1]].characterList[charIndex[0]];
    }
    public void IncrementCharacter(int inc)
    {
        int[] newIndexes = IncrementCharIndex(inc);
        _currSet = newIndexes[1];
        SelectSet(_currSet);
        SelectCharacter(newIndexes[0]);
    }
}
