using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
public class syntaxHighlter : MonoBehaviour
{  [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI outputField;
    bool isInserted=false;
    bool isRemoved=true;
    string highlightedText;
    private int currentCaretPosition;
    private int sizeBeforeChange;
    private int postionBefore;
    private void Start()
    {

                string unhighlightedText = Regex.Replace(inputField.text, @"<.*?>", string.Empty);
            string t=inputField.text;

            const string pattern = @"\b(mov|mul|add|div|or|and|not|sub|push|pop|ret|call|inc|dec|dw|db|assume|segment|ends|PROC|ENDP)\b";
            const string replacement = "<color=#0456CD>$1</color>";
            const string pattern2=@"\b(ax|bx|cx|dx|cs|ss|es|ds|al|ah|bh|cl|ch|dl|dh)\b";
            const string replacement2 = "<color=#CD3B04>$1</color>";


            int textSizeBeforeChange = inputField.text.Length;

            string highlightedText = Regex.Replace(unhighlightedText, pattern, replacement);
         //   inputField.SetTextWithoutNotify(highlightedText);
            string highlightedText2 = Regex.Replace(highlightedText, pattern2, replacement2);
            inputField.SetTextWithoutNotify(highlightedText2);

           inputField.onValueChanged.AddListener(OnInputValueChanged);


    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Return))
    {
        // Perform the desired action when Enter is pressed
        // Call your function or execute the desired code here
       Debug.Log("ENTER IS PRESSED");
                int caretPosition = inputField.stringPosition;
            string textBeforeCaret = inputField.text.Substring(0, caretPosition);
            int lineStartPosition = textBeforeCaret.LastIndexOf('\n') + 1;
            bool isCaretAtLineStart = (caretPosition == lineStartPosition);

            if (!isCaretAtLineStart)
            {
                // Move the caret down to the start of the next line
                int nextLineStartPosition = inputField.text.IndexOf('\n', caretPosition) + 1;
                inputField.stringPosition = nextLineStartPosition;
               // inputField.selectionAnchorPosition = nextLineStartPosition;
            }
    }

    if(Input.GetKeyDown(KeyCode.Backspace)){
        Debug.Log("DELETE");
        isRemoved=true;
    }
}

private int GetLineIndexFromPosition(TMPro.TMP_InputField inputField, int caretPosition)
{
    string text = inputField.text;
    int lineIndex = 0;

    for (int i = 0; i < caretPosition; i++)
    {
        if (text[i] == '\n')
        {
            lineIndex++;
        }
    }

    return lineIndex;
}

// Custom method to get the end position of a line
private int GetLineEndPosition(string text, int lineIndex)
{
    int lineCount = 0;
    int position = 0;

    while (lineCount < lineIndex && position < text.Length)
    {
        if (text[position] == '\n')
        {
            lineCount++;
        }

        position++;
    }

    while (position < text.Length && text[position] != '\n')
    {
        position++;
    }

    return position;
}
 private void OnInputValueChanged(string text)
    {
        // Remove existing color tags from the text
        sizeBeforeChange=text.Length;
        postionBefore=inputField.stringPosition;
        string unhighlightedText = Regex.Replace(text, @"<.*?>", string.Empty);
       

       
        // Apply keyword highlighting
        highlightedText = HighlightKeywords(unhighlightedText);

        // Apply identifier highlighting
       highlightedText = HighlightIdentifiers(highlightedText);
        //string pattern = "</color></color>";
     // highlightedText = Regex.Replace(highlightedText, pattern, "</color>");
        // Update the input field text without triggering value changed event
         // Check if a newline character was inserted
            bool isNewlineInserted = text.EndsWith("\n");

    

         inputField.SetTextWithoutNotify(highlightedText);


        // Adjust the caret position based on modifications
        AdjustCaretPosition();
       // int nextLineStartPosition = inputField.stringPosition + 1;
         //inputField.stringPosition = nextLineStartPosition;
         //inputField.selectionAnchorPosition = nextLineStartPosition;
       
    }

    private string HighlightKeywords(string text)
    {

       
        const string pattern = @"(?<!</color>)\b(mov|mul|add|div|or|and|not|sub|push|pop|ret|call|inc|dec|dw|db|assume|segment|ends|PROC|ENDP)\b(?!<\/color>)";
        const string replacement = "<color=#0456CD>$1</color>";
        
        return Regex.Replace(text, pattern, replacement, RegexOptions.IgnoreCase);
        
    }

    private string HighlightIdentifiers(string text)
    {
       
        const string pattern = @"(?<!</color>)\b(ax|bx|cx|dx|cs|ss|es|ds|al|ah|bh|cl|ch|dl|dh)\b(?!<\/color>)";

        const string replacement =  "<color=#CD3B04>$1</color>";
        return Regex.Replace(text, pattern, replacement, RegexOptions.IgnoreCase);
    }

  private void AdjustCaretPosition()
{
    int textLengthDiff = inputField.text.Length - sizeBeforeChange;
     
       // isRemoved=true;
     //  isInserted=false;
    if (textLengthDiff < 0)
    {
        // Adjusting for text removal
        Debug.Log("REMOVAL"+inputField.text.Length+"  " +sizeBeforeChange+"DIIF = "+textLengthDiff);
        inputField.stringPosition -= -textLengthDiff;
        if(isRemoved){
            inputField.stringPosition +=8;
            isRemoved=false;
            Debug.Log("TW");
        }
        
        
    }
    else
    { 
        
        if(textLengthDiff>0){
        // Adjusting for text insertion
        Debug.Log("INSERTION");
         inputField.stringPosition += textLengthDiff;

        
         //inputField.stringPosition -=8;
          
        }
        //isRemoved=false;
       // isInserted=true;

    }

 
     int characterIndex = inputField.caretPosition - 1;
     char character = inputField.text[characterIndex];
     if(character=='>'){
        int currentPosition = inputField.stringPosition;
        int nextLineStartPosition = inputField.text.IndexOf('\n', currentPosition) + 1;
        inputField.stringPosition = nextLineStartPosition;
     }
    isRemoved=false;
}

}
