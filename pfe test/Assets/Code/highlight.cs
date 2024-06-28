using TMPro;
using UnityEngine;

public class highlight : MonoBehaviour
{


    public void HighlightLine(int lineNumber)
{
    TMP_InputField textArea = this.GetComponent<TMP_InputField>();

    string[] lines = textArea.text.Split('\n');
    if (lineNumber < 1 || lineNumber > lines.Length)
    {
        Debug.LogError("Invalid line number: " + lineNumber);
        return;
    }

    // Unhighlight previously highlighted lines
    for (int i = 0; i < lines.Length; i++)
    {
        lines[i] = RemoveHighlight(lines[i]);
    }

    // Highlight the specified line
    string highlightedLine = "<mark=red>" + lines[lineNumber - 1] + "</mark>";
    lines[lineNumber - 1] = highlightedLine;

    textArea.text = string.Join("\n", lines);
}

public static string RemoveHighlight(string line)
{
    // Remove the highlight tags from the line
    return line.Replace("<mark=red>", "").Replace("</mark>", "");
}
}