using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Step : MonoBehaviour
{
     public TMP_InputField myInputField;
         public GameObject highlight;

         public SequenceGenerator sq;
  static int i=0;

 public static List<int> visitedLines=new List<int>();
    public void OnButtonClick()
    {

        AnimationController.instNumber++;
        StepBack.popTwice=true;
        string instString = Emulateur.cpu_core.getCurrentInstruction().getOriginalString();
        Debug.Log("Executed : "+ instString);
        i=GetLineOfSubstring(myInputField.text,"x segment");

          i++;
        highlightCurrentInstructin(myInputField.text, instString);
        Emulateur.ExecuteStep();
        sq.clearPreviousValues();
        displayHistorique();
        

    }

    void displayHistorique(){
        foreach (KeyValuePair<string, Stack<Info>> kvp in Emulateur.historiqueRegs)
        {
            string key = kvp.Key;
            Stack<Info> stack = kvp.Value;

            // Display the key
            Debug.Log("Reg: " + key);

            // Iterate through the elements in the stack and display them
            foreach (Info element in stack)
            {
                 Debug.Log("Val precedentes : " + element.value +" AT "+element.instNum);
            }
        }
        foreach (KeyValuePair<int, Stack<Info>> kvp in Emulateur.historiqueRam)
        {
            int key = kvp.Key;
            Stack<Info> stack = kvp.Value;

            // Display the key
            Debug.Log("ram adresse: " + key);

            // Iterate through the elements in the stack and display them
            foreach (Info element in stack)
            {
                 Debug.Log("Val precedentes : " + element.value+" AT "+element.instNum);
            }
        }

        foreach (int item in Emulateur.historiqueFIvert)
        {
            Debug.Log("Historique fil verte perecendete : " +item);
        }
        foreach (int item in Emulateur.historiqueFIbleu)
        {
            Debug.Log("Historique fil bleu perecendete : " +item);
        }

    }


    private void highlightCurrentInstructin(string text,string substring){
        string[] lines = text.Split('\n');
        highlight.GetComponent<highlight>().HighlightLine(getLine(text,substring));
       
    }

    public int getLine(string text, string searchString){
  
                string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(searchString) && ! visitedLines.Contains(i+1))
                {
                    visitedLines.Add(i+1);// pour ne pas revenir 
                    return i + 1;
                }
            }

    return -1; // Indicate that the string was not found in the text.
    }

    public int GetLineOfSubstring(string text, string substring)
{
    int line = 1;
        string[] lines = text.Split('\n');

    for (int i = 0; i < lines.Length; i++)
    {
        if (lines[i].Contains(substring))
        {
            return line;
        }
            

            line++;
    }
        // The substring was not found in the text
        return -1;
}
}
