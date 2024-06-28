using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayST : MonoBehaviour
{

    public TMP_InputField myInputField;

    public void OnButtonClick()
    {

      if(Emulateur.cpu_core.ST==null){
        Debug.Log("null");
      }
      foreach(KeyValuePair<string,Symbol> s in Emulateur.cpu_core.ST.symbols)
     {
        Debug.Log(s.ToString());
     }

    }
}
