using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class toggleMC : MonoBehaviour
{
    


    public void OnButtonClick(){
         Image buttonImage = this.GetComponent<Image>();
        Emulateur.cpu_core.seq.an.toggleCodeMachine=!Emulateur.cpu_core.seq.an.toggleCodeMachine;
        if(Emulateur.cpu_core.seq.an.toggleCodeMachine==true){
               buttonImage.color = Color.red;
        }else{
                 buttonImage.color = Color.white;
        }
        Emulateur.cpu_core.seq.an.updateRamVals();
    } 
}
