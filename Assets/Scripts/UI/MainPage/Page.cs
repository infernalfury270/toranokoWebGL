using UnityEngine;
using UnityEngine.Events;

public class Page : MonoBehaviour, IAssignable
{
    public GameObject obj;
    public RectTransform rT;
    public CanvasGroup canvasGrp;
    public UnityEvent OnPreload;
    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public void Assign()
    {
        obj = gameObject;
        rT = GetComponent<RectTransform>();
        canvasGrp = GetComponent<CanvasGroup>();
    }
}
