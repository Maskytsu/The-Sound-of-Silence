using UnityEngine;
using UnityEngine.UI;

public class Blackout : MonoBehaviour
{
    public RawImage Image;

    public void SetAlphaToZero()
    {
        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0);
    }
}