using UnityEngine;

public class SaveCurrentEnding : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    private int EndingNumber = 1;

    private void Awake()
    {
        switch (EndingNumber)
        {
            case 1:
                EndingsSaveManager.Ending1Reached = true;
                break;
            case 2:
                EndingsSaveManager.Ending2Reached = true;
                break;
            case 3:
                EndingsSaveManager.Ending3Reached = true;
                break;
            case 4:
                EndingsSaveManager.Ending4Reached = true;
                break;
            case 5:
                EndingsSaveManager.Ending5Reached = true;
                break;
            default:
                Debug.LogError("NO ENDING WITH THAT NUMBER!");
                break;
        }

        EndingsSaveManager.SaveEndings();
    }
}
