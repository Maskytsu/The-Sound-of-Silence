using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{
    public static void SetAlpha(this Image ui, float alpha)
    {
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, alpha);
    }

    public static void SetAlpha(this RawImage ui, float alpha)
    {
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, alpha);
    }

    public static void SetAlpha(this TextMeshProUGUI ui, float alpha)
    {
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, alpha);
    }
}