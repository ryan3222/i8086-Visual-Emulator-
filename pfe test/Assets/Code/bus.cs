using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bus : MonoBehaviour
{

    Ifill fillScript;
    Ifill fill2Script;
    public bool isAnimated=false;
    
    void Start()
    {
        fillScript=transform.GetChild(0).GetComponent<Ifill>();//for starting the animation in direction 0
        fill2Script=transform.GetChild(1).GetComponent<Ifill>(); // for starting the animation in direction 1
        fillScript.SetGlobalScale(new Vector3(0.187265769f,0.0890545771f,1));
        fill2Script.SetGlobalScale(new Vector3(0.187265769f,0.0890545771f,1));
        
    }
    void Update(){
       // Debug.Log(fillScript.activerAnimation + " fff" + fill2Script.activerAnimation);
        isAnimated=fillScript.activerAnimation || fill2Script.activerAnimation;
    }
    

    public virtual void animate(int direction){

       
        if(direction==0){
            fillScript.activerAnimation=true;
            isAnimated=true;

        }else if(direction==1){
            fill2Script.activerAnimation=true;
            isAnimated=true;

        }

    }
    
}
