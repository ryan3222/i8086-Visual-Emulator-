using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayMem : MonoBehaviour
{

    public TMP_InputField myInputField;

    public void OnButtonClick()
    {

      
      foreach(MemByte BYTE in Emulateur.cpu_core.memory.GetMemBytes())
      {
        if(BYTE.address>=1000) 
           Debug.Log(BYTE.address+"   :  "+ BYTE.ToString());
      }
   

    }
}
