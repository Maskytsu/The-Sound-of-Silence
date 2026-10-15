using System.Text.RegularExpressions;
using UnityEngine;

public static class ReadingTimeCalculator
{
    // Typical values:
    //  12-13 CPS - children / slow, comfortable pace
    //  15 CPS    - comfortable adult pace (common European guidelines)
    //  17 CPS    - Netflix children's content
    //  20 CPS    - Netflix adult content (upper limit)

    private const float CharsPerSecond = 15f;
    private const float MinDuration = 2f;
    private const float PausePerEllipsis = 0.3f;
    private const float PausePerSentence = 0.15f;

    private static readonly Regex RichTextTags = new Regex(@"<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);
    private static readonly Regex Ellipsis = new Regex(@"\.{3}|…", RegexOptions.Compiled);
    private static readonly Regex SentenceEnd = new Regex(@"[.!?]+(?=\s|$)", RegexOptions.Compiled);

    public static float GetReadingTime(this string text)
    {
        if (string.IsNullOrWhiteSpace(text) || CharsPerSecond <= 0f)
        {
            return 0f;
        }

        string noTags = RichTextTags.Replace(text, string.Empty);
        string clean = Whitespace.Replace(noTags, " ").Trim();
        int ellipsisCount = Ellipsis.Matches(clean).Count;
        string withoutEllipsis = Ellipsis.Replace(clean, " ");

        float duration = clean.Length / CharsPerSecond;
        duration += ellipsisCount * PausePerEllipsis;
        duration += SentenceEnd.Matches(withoutEllipsis).Count * PausePerSentence;

        return Mathf.Max(duration, MinDuration);
    }
}