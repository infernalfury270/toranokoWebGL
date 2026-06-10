using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExploreManager : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] CanvasGroup _uiGrp;
    [SerializeField] CanvasGroup _exitPromptGrp;
    [SerializeField] CanvasGroup _interactablesGrp;
    [SerializeField] CanvasGroup _dialogueGrp;
    [SerializeField] GameObject _morningBG, _nightBG;
    [SerializeField] CanvasGroup _morningTrans, _nightTrans;
    [SerializeField] GameObject _moveBtn, _interactBtn;
    [SerializeField] LocationData _startLocationMorning, _startLocationNight;
    [SerializeField] Animator _transition;
    [SerializeField] AudioSource _exploreEnterSFX;
    [SerializeField] Image _locationBG, _cutsceneBG;
    [SerializeField] AudioSource _bgmSource, _stepsSource;
    [SerializeField] AudioClip _morningBGM, _nightBGM, _midnightBGM;
    [SerializeField] TMP_Text _speakerName, _dialogueText;
    [SerializeField] GameObject _confirmDialogue;
    [SerializeField] AudioSource[] _dialogueAudio;

    CanvasGroup _locationTransition;
    private DateTime dateTime;
    private static readonly float _fadeTime = 0.3f;
    private static readonly float _textGap = 0.025f;
    private int _counter;
    private RectTransform _btnRT;
    private RectTransform _cfmRT;
    private bool _isTransitioning;
    private bool _isCutsceneRunning;

    private Vector2 _ogBob;
    private float _rot, _jitter, _jitterSpeed;

    private void Start()
    {
        _cfmRT = _confirmDialogue.GetComponent<RectTransform>();
        _ogBob = _cfmRT.anchoredPosition;
        _rot = 45.0f;
        _jitter = 7.5f;
        _jitterSpeed = 1.5f;
        CutsceneData.OnInvokeCutscene += RunCutscene;
        _btnRT = _interactablesGrp.GetComponent<RectTransform>();
        _isTransitioning = false;
        _exitPromptGrp.alpha = 0;
        _interactablesGrp.alpha = 1;
        dateTime = DateTime.Now;

        var tod = dateTime.TimeOfDay;
        if (tod.Hours >= 6 && tod.Hours < 22)
        {
            _morningBG.SetActive(true);
            _nightBG.SetActive(false);
            _morningTrans.gameObject.SetActive(true);
            _nightTrans.gameObject.SetActive(false);
            _locationTransition = _morningTrans;
            SetLocation(_startLocationMorning);
        } else
        {
            _morningBG.SetActive(false);
            _nightBG.SetActive(true);
            _morningTrans.gameObject.SetActive(false);
            _nightTrans.gameObject.SetActive(true);
            _locationTransition = _nightTrans;
            SetLocation(_startLocationNight);
        }
    }
    public void PlayBGM()
    {
        TimeSpan tod = dateTime.TimeOfDay;
        if (tod.Hours >= 6 && tod.Hours < 19)
            _bgmSource.clip = _morningBGM;
        else if (tod.Hours >= 19 && tod.Hours < 22)
            _bgmSource.clip = _nightBGM;
        else
            _bgmSource.clip = _midnightBGM;
        _bgmSource.Play();
    }
    private void Update()
    {
        if (InputSystem.actions["ExploreUIToggle"].WasPressedThisFrame())
            if (_uiGrp.alpha == 0)
                _uiGrp.alpha = 1;
            else
                _uiGrp.alpha = 0;
        _cfmRT.Rotate(0,0, _rot * Time.deltaTime);
        _cfmRT.anchoredPosition = _ogBob + new Vector2(0,_jitter * Mathf.Sin(Time.time * _jitterSpeed));
    }
    public void ToMainScene() => StartCoroutine(ToMain());
    public void ToggleExitPrompt(bool isOpen) => StartCoroutine(TogglePromptCrt(isOpen));
    private IEnumerator ToMain()
    {
        _transition.Play("FootstepRecover");
        _exploreEnterSFX.Play();
        yield return new WaitForEndOfFrame();
        while (_transition.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("MainScene");
    }
    private IEnumerator TogglePromptCrt(bool isOpen)
    {
        if (_isTransitioning || _isCutsceneRunning) yield break;
        _counter++;
        int id = _counter;
        while (id == _counter && (isOpen ? (_exitPromptGrp.alpha < 1) : (_exitPromptGrp.alpha > 0)))
        {
            float inc = (Time.deltaTime / _fadeTime) * (isOpen ? 1 : -1);
            _exitPromptGrp.alpha += inc;
            _interactablesGrp.alpha -= inc;
            yield return new WaitForEndOfFrame();
        }
    }
    private void GoToLocation(LocationData location) => StartCoroutine(LocationCoroutine(location));
    private void SetLocation(LocationData location)
    {
        var info = location.GetCurrentInfo(dateTime);
        _locationBG.sprite = info.background;
        for (int i = _btnRT.childCount - 1; i >= 1; i--)
        {
            Destroy(_btnRT.GetChild(i).gameObject);
        }
        for (int i = 0; i < info.travelPts.Count; i++) { 
            var travelData = info.travelPts[i];
            var newBtn = Instantiate(_moveBtn, _btnRT);
            newBtn.GetComponent<RectTransform>().anchoredPosition = travelData.pos;
            newBtn.GetComponentInChildren<TMP_Text>().text = travelData.destination.locationName;
            newBtn.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (!_isTransitioning && !_isCutsceneRunning){
                    _exploreEnterSFX.Play();
                    GoToLocation(travelData.destination);
                }
            });
        }
        for (int i = 0; i < info.interactables.Count; i++)
        {
            var interactData = info.interactables[i];
            var newBtn = Instantiate(_interactBtn, _btnRT);
            newBtn.GetComponent<RectTransform>().anchoredPosition = interactData.pos;
            newBtn.GetComponentInChildren<TMP_Text>().text = interactData.name;
            newBtn.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (!_isTransitioning && !_isCutsceneRunning)
                    interactData.OnInteract?.Invoke();
            });
        }
    }
    private IEnumerator LocationCoroutine(LocationData location)
    {
        _stepsSource.Play();
        _isTransitioning = true;
        _locationTransition.GetComponentInChildren<Animator>().Play("IconAnimation", 0, 0.0f);
        while (_locationTransition.alpha < 1)
        {
            _locationTransition.alpha += Time.deltaTime / _fadeTime;
            yield return new WaitForEndOfFrame();
        }
        SetLocation(location);
        yield return new WaitForSeconds(2.0f);
        while (_locationTransition.alpha > 0)
        {
            _locationTransition.alpha -= Time.deltaTime / _fadeTime;
            yield return new WaitForEndOfFrame();
        }
        _isTransitioning = false;
    }
    public void RunCutscene(CutsceneData cutscene)
    {
        if (_isTransitioning || _isCutsceneRunning || cutscene.sequence.Count < 1) return;
        StartCoroutine(CutsceneCoroutine(cutscene));
    }
    private IEnumerator CutsceneCoroutine(CutsceneData cutscene)
    {
        _isCutsceneRunning = true;
        _confirmDialogue.SetActive(false);
        _speakerName.text = cutscene.sequence[0].speakerName;
        _dialogueText.text = string.Empty;

        bool canSkip = false;
        bool canContinue = false;

        while (_dialogueGrp.alpha < 1 && _interactablesGrp.alpha > 0)
        {
            float t = Time.deltaTime / _fadeTime;
            _dialogueGrp.alpha += t;
            _interactablesGrp.alpha -= t;
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < cutscene.sequence.Count; i++)
        {
            _speakerName.text = cutscene.sequence[i].speakerName;
            var info = cutscene.sequence[i];
            if (info.clearBackground)
            {
                StartCoroutine(FadeCutsceneBG(null));
            } else if (info.background != null)
            {
                StartCoroutine(FadeCutsceneBG(info.background));
            }
            for (int line = 0; line < info.dialogue.Length; line++)
            {
                canSkip = canContinue  = false;
                _confirmDialogue.SetActive(false);
                _dialogueText.text = string.Empty;
                var str = info.dialogue[line];
                for (int c = 0; c < str.Length; c++)
                {
                    if (!InputSystem.actions["DialogueProgress"].IsPressed())
                    {
                        canSkip = true;
                    }
                    if (canSkip && InputSystem.actions["DialogueProgress"].IsPressed())
                    {
                        _dialogueText.text = str;
                        break;
                    }
                    for (int a = 0; a < _dialogueAudio.Length; a++)
                    {
                        if (!_dialogueAudio[a].isPlaying)
                        {
                            _dialogueAudio[a].Play();
                            break;
                        }
                    }
                    _dialogueText.text += str[c];
                    yield return new WaitForSeconds(_textGap);
                }
                _confirmDialogue.SetActive(true);
                while (gameObject != null)
                {
                    if (!InputSystem.actions["DialogueProgress"].IsPressed())
                    {
                        canContinue = true;
                    }
                    if (canContinue && InputSystem.actions["DialogueProgress"].IsPressed())
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        FadeCutsceneBG(null);
        while (_dialogueGrp.alpha > 0 && _interactablesGrp.alpha < 1)
        {
            float t = Time.deltaTime / _fadeTime;
            _dialogueGrp.alpha -= t;
            _interactablesGrp.alpha += t;
            yield return new WaitForEndOfFrame();
        }
        _isCutsceneRunning = false;
    }
    private IEnumerator FadeCutsceneBG(Sprite bg)
    {
        var grp = _cutsceneBG.GetComponent<CanvasGroup>();
        if (bg == null)
        {
            while (grp.alpha > 0)
            {
                grp.alpha -= Time.deltaTime / _fadeTime;
                yield return new WaitForEndOfFrame();
            }
        } else
        {
            _cutsceneBG.sprite = bg;
            while (grp.alpha < 1)
            {
                grp.alpha += Time.deltaTime / _fadeTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    private void OnDestroy()
    {
        CutsceneData.OnInvokeCutscene -= RunCutscene;
    }
}
