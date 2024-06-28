using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avancer : MonoBehaviour
{

     public AnimationController an;
    public void OnButtonClick(){
        an.Advance();
    }
}
