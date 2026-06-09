using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExploreManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _exitPromptGrp;
    [SerializeField] CanvasGroup _interactablesGrp;
    [SerializeField] Animator _transition;
    [SerializeField] AudioSource _exploreEnterSFX;
    private static readonly float _fadeTime = 0.3f;
    private int _counter;
    private void Start()
    {
        _exitPromptGrp.alpha = 0;
        _interactablesGrp.alpha = 1;
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
    private void LoadLocation()
    {

    }
}
