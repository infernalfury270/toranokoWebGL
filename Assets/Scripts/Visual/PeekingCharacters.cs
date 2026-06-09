using System.Collections;
using UnityEngine;

public class PeekingCharacters : MonoBehaviour
{
    [SerializeField] float _range = 200.0f;
    [SerializeField] RectTransform[] _peekers;
    private static readonly float _moveTime = 0.5f;
    private static readonly float _minWatchTime = 5.0f;
    private static readonly float _maxWatchTime = 10.0f;
    private static readonly float _minCooldown = 5.0f;
    private static readonly float _maxCooldown = 30.0f;
    private bool[] _active;
    private float _spawnCooldown;
    private void Awake()
    {
        _active = new bool[_peekers.Length];
        _spawnCooldown = _maxCooldown;
    }
    private void Update()
    {
        if (_spawnCooldown <= 0.0f)
        {
            _spawnCooldown = Random.Range(_minCooldown, _maxCooldown);
            int index = -1;
            int checks = 0;
            while (index < 0 && checks < 200)
            {
                checks++;
                int toMove = Random.Range(0, _peekers.Length);
                if (!_active[toMove])
                {
                    index = toMove;
                }
            }
            if (index != -1)
            {
                _active[index] = true;
                StartCoroutine(PeekOut(index));
            }
        }
        else
            _spawnCooldown -= Time.deltaTime;
    }
    private IEnumerator PeekOut(int index)
    {
        float t = 0;
        RectTransform peeker = _peekers[index];
        var p1 = new Vector2(Random.Range(-_range, _range), -peeker.sizeDelta.y);
        var p2 = p1 + new Vector2(0, peeker.sizeDelta.y);
        peeker.anchoredPosition = p1;
        while (t <= _moveTime)
        {
            t += Time.deltaTime;
            float a = t / _moveTime;
            peeker.anchoredPosition = Vector2.Lerp(p1, p2, a);
            yield return new WaitForEndOfFrame();
        }
        t = 0;
        yield return new WaitForSeconds(Random.Range(_minWatchTime, _maxWatchTime));
        while (t <= _moveTime)
        {
            t += Time.deltaTime;
            float a = t / _moveTime;
            peeker.anchoredPosition = Vector2.Lerp(p2, p1, a);
            yield return new WaitForEndOfFrame();
        }
        _active[index] = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 25, 1));
    }
}
