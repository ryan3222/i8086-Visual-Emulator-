using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relire : MonoBehaviour
{

    public AnimationController an;
    public AnimationController anp;
    public void OnButtonClick(){
        an.ReLire();
        if(an.firstFetchSequence.Count==0)
             anp.ReLire();


        Debug.Log(anp.CurrentSequence.Count);
    }
}