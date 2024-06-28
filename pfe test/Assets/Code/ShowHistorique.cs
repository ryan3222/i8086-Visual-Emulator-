using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHistorique : MonoBehaviour
{
    public void OnButtonClick(){
         Debug.Log("CURRENT INST NUM " + AnimationController.instNumber);
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
}
