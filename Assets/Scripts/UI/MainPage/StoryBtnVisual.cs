using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoryBtnVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly Vector3 _scale = new Vector3(1.025f, 1.025f, 1.025f);
    private static readonly float _scaleTime = 0.15f;
    private static readonly float _maxTilt = 2.5f;
    private static readonly float _tiltSpeed = 3.5f;
    [SerializeField] bool _isCharBtn = false;
    [SerializeField] RectTransform _frame;
    private bool _isEntered;
    private Quaternion GetSelectRotation()
    {
        return Quaternion.Euler(0, 0, Mathf.Sin(Time.time * _tiltSpeed) * _maxTilt);
    }
    private IEnumerator OnEnter()
    {
        _isEntered = true;
        float t = 0;
        Vector3 ogSize = _frame.localScale;
        Vector3 newSize = _scale;
        Quaternion ogRot = _frame.rotation;
        while (t <= _scaleTime)
        {
            if (!_isEntered) yield break;
            t += Time.deltaTime;
            float a = t / _scaleTime;
            _frame.localScale = Vector3.Lerp(ogSize, _scale, a);
            if (!_isCharBtn)
                _frame.rotation = Quaternion.Lerp(ogRot, GetSelectRotation(), a);
            yield return new WaitForEndOfFrame();
        }
        if (!_isCharBtn)
            StartCoroutine(Tilt());
    }
    private IEnumerator OnExit()
    {
        _isEntered = false;
        float t = 0;
        Vector3 ogSize = _frame.localScale;
        Quaternion ogRot = _frame.rotation;
        while (t <= _scaleTime)
        {
            if (_isEntered) yield break;
            t += Time.deltaTime;
            float a = t / _scaleTime;
            _frame.localScale = Vector3.Lerp(ogSize, Vector3.one, a);
            if (!_isCharBtn)
                _frame.rotation = Quaternion.Lerp(ogRot, Quaternion.identity, a);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator Tilt()
    {
        while (_isEntered)
        {
            _frame.rotation = GetSelectRotation();
            yield return new WaitForEndOfFrame();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(OnEnter());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(OnExit());
    }
}
