using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slow : MonoBehaviour
{
   public void OnButtonClick(){

    if(Time.timeScale==0.5f)
       Time.timeScale=1;
    else
        Time.timeScale=0.5f;


   
   }
}
