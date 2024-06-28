using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class displayRegs : MonoBehaviour
{

    public TMP_InputField myInputField;

    public void OnButtonClick()
    {
      
   
     foreach(KeyValuePair<string,Register> reg in Emulateur.cpu_core.registers.regs)
     {
        Debug.Log(reg.Key+" : "+ reg.Value.Get());
     }


    }
}
