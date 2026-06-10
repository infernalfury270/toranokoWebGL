using Coffee.UIExtensions;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Sprite[] _sprites;
    [SerializeField] float _minLoadingTime = 3.0f;
    [SerializeField] float _maxLoadingTime = 5.0f;
    private static readonly float _fadeTime = 0.35f;
    [SerializeField] Animator _animator;
    [SerializeField] Image _loadingBarAmt;
    [SerializeField] CanvasGroup _loadingBarGrp;
    [SerializeField] Image _randomSprite;
    [SerializeField] UIParticle[] _spawnParticles;
    [SerializeField] UIParticle _loveParticle;
    [SerializeField] AudioSource _meow;
    public UnityEvent OnLoadFinished;
    public void PlaySpawnParticle(int index)
    {
        if (index < 0 || index >= _spawnParticles.Length) return;
        var particle = _spawnParticles[index].transform;
        particle.GetChild(0).GetComponent<ParticleSystem>().Emit(1);
        particle.GetChild(1).GetComponent<ParticleSystem>().Emit(3);
        particle.GetChild(2).GetComponent<ParticleSystem>().Emit(6);
        particle.GetComponent<AudioSource>().Play();
    }
    public void PlayLoveParticles()
    {
        _loveParticle.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        _loveParticle.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        _loveParticle.GetComponent<AudioSource>().Play();
    }
    public void StopLoveParticles()
    {
        _loveParticle.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        _loveParticle.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
        _loveParticle.GetComponent<AudioSource>().Stop();
    }
    public void Meow() => _meow.Play();

    private void Start()
    {
        StartCoroutine(LoadingSequence());
    }
    private IEnumerator LoadingSequence()
    {
        _loadingBarAmt.fillAmount = 0;
        _loadingBarGrp.alpha = 0;
        float loadTime = Random.Range(_minLoadingTime, _maxLoadingTime);
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            yield return new WaitForSeconds(1.0f);
            _animator.SetBool("LoadingComplete", false);
            _animator.Play("ToradoraLovePt1");
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("ToradoraLovePt2"))
            {
                yield return new WaitForEndOfFrame();
            }
            while (_loadingBarGrp.alpha < 1)
            {
                float alphaStep = Time.deltaTime / _fadeTime;
                _loadingBarGrp.alpha += alphaStep;
                yield return new WaitForEndOfFrame();
            }
        } else
        {
            _randomSprite.gameObject.SetActive(true);
            _randomSprite.sprite = _sprites[Random.Range(0, _sprites.Length)];
            _loadingBarGrp.alpha = 1.0f;
        }
        while (_loadingBarAmt.fillAmount < 1.0f)
        {
            float alphaStep = Time.deltaTime / loadTime;
            _loadingBarAmt.fillAmount += alphaStep;
            yield return new WaitForEndOfFrame();
        }
        _animator.SetBool("LoadingComplete", true);
        while (_loadingBarGrp.alpha > 0)
        {
            float alphaStep = Time.deltaTime / _fadeTime;
            _loadingBarGrp.alpha -= alphaStep;
            yield return new WaitForEndOfFrame();
        }
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            _animator.Play("FootstepCover");
        }
        OnLoadFinished?.Invoke();
    }
}
