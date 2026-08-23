using System.Collections.Generic;

public static class EndingsSaveManager
{
    public static bool Ending1Reached;
    public static bool Ending2Reached;
    public static bool Ending3Reached;
    public static bool Ending4Reached;
    public static bool Ending5Reached;

    public static bool AllEndingsReached => Ending1Reached && Ending2Reached && Ending3Reached && Ending4Reached && Ending5Reached;

    private static List<BoolSaveData> _endingsSaveData = new()
        {
            new ("Ending1Reached", () => Ending1Reached, value => Ending1Reached = value),
            new ("Ending2Reached", () => Ending2Reached, value => Ending2Reached = value),
            new ("Ending3Reached", () => Ending3Reached, value => Ending3Reached = value),
            new ("Ending4Reached", () => Ending4Reached, value => Ending4Reached = value),
            new ("Ending5Reached", () => Ending5Reached, value => Ending5Reached = value),
        };

    public static void SaveEndings()
    {
        foreach (var saveDataElement in _endingsSaveData)
        {
            saveDataElement.SaveValue();
        }
    }

    public static void LoadEndings()
    {
        foreach (var saveDataElement in _endingsSaveData)
        {
            saveDataElement.LoadValue();
        }
    }
}