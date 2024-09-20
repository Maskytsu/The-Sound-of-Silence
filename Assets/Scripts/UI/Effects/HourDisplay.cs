using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HourDisplay : MonoBehaviour
{
    public string HourText;

    [SerializeField] private TextMeshProUGUI _hourTMP;
    [SerializeField] private RawImage _background;

    private IEnumerator Start()
    {
        PlayerManager.Instance.PlayerInputProvider.TurnOffPlayerMap();
        _hourTMP.text = HourText;
        yield return new WaitForSeconds(0);

        PlayerManager.Instance.PlayerInputProvider.TurnOnPlayerMap();
    }
}
