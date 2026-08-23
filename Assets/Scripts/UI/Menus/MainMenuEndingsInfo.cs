using UnityEngine;

public class MainMenuEndingsInfo : MonoBehaviour
{
    [SerializeField] private GameObject AllEndingsFinished;
    [SerializeField] private GameObject Ending1Info;
    [SerializeField] private GameObject Ending2Info;
    [SerializeField] private GameObject Ending3Info;
    [SerializeField] private GameObject Ending4Info;
    [SerializeField] private GameObject Ending5Info;

    private void Start()
    {
        var allEndingsReached = EndingsSaveManager.AllEndingsReached;
        AllEndingsFinished.SetActive(allEndingsReached);
        Ending1Info.SetActive(EndingsSaveManager.Ending1Reached && !allEndingsReached);
        Ending2Info.SetActive(EndingsSaveManager.Ending2Reached && !allEndingsReached);
        Ending3Info.SetActive(EndingsSaveManager.Ending3Reached && !allEndingsReached);
        Ending4Info.SetActive(EndingsSaveManager.Ending4Reached && !allEndingsReached);
        Ending5Info.SetActive(EndingsSaveManager.Ending5Reached && !allEndingsReached);
    }
}
