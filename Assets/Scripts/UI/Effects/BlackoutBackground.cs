using UnityEngine;
using UnityEngine.UI;

public class BlackoutBackground : MonoBehaviour
{
    public RawImage Image;

    public void StartAlphaFromZero()
    {
        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0);
    }
}