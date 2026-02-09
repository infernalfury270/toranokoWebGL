using UnityEngine;

[CreateAssetMenu(fileName = "Global", menuName = "Global Values")]
public class Global : SingletonScriptableObject<Global>
{
    public Sprite placeholderSprite;
}
