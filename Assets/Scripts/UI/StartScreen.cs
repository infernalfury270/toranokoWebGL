using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    [SerializeField] GameObject _start, _unavailable;
    [DllImport("__Internal")]
    private static extern bool IsMobile();
    public bool isMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            return IsMobile();
#endif
        return false;
    }
    private void Awake()
    {
        bool check = isMobile();
        _start.SetActive(!check);
        _unavailable.SetActive(check);
    }
    public void StartGame() => SceneManager.LoadScene("MainScene");
}
