using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedup : MonoBehaviour
{
   public void OnButtonClick(){

    if(Time.timeScale==100f)
       Time.timeScale=1;
    else
        Time.timeScale=100f;


   
   }
}
