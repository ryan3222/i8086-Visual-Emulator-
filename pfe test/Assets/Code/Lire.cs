using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lire : MonoBehaviour
{

    public AnimationController an;
    public AnimationController anp;
    public void OnButtonClick(){
        an.Lire();
        anp.Lire();
       
        Image buttonImage = this.GetComponent<Image>();
   
        if(an.lireToggle){
               buttonImage.color = Color.red;
             
        }else{
                 buttonImage.color = Color.white;
                  
        }
        Debug.Log(anp.CurrentSequence.Count);
    }
}
