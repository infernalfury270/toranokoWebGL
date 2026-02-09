using UnityEngine;
using UnityEngine.UI;

public class TextureScroller : MonoBehaviour
{
    [SerializeField] float _scrollX, _scrollY;
    [SerializeField] float _speedMult = 1.0f;
    [SerializeField] Image _renderer;
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        _renderer.material.mainTextureOffset += new Vector2(_scrollX * Time.deltaTime * _speedMult, _scrollY * Time.deltaTime * _speedMult);
    }
}
