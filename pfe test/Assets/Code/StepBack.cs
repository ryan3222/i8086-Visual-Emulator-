using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBack : MonoBehaviour
{
    public AnimationController an;

    public static bool popTwice;
    
     public void OnButtonClick()
    {
      
         AnimationController.instNumber--;
         if(AnimationController.instNumber<0){
            AnimationController.instNumber=0;
            an.resetALL();
         }else{
        
              an.stepBack();
              popTwice=false;
         }
         

        

    }
}
