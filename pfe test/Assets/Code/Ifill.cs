using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ifill :MonoBehaviour
{

    public bool activerAnimation=false;
    protected float end;
    protected float  start;
    protected Vector3 initPos;
    protected Vector3 endOfBus;
    protected Vector3 startOfBus;
    protected Vector3 Destination;
    protected  float speed=5;
    protected SpriteRenderer spriteRenderer;
      void animation(){}
      void hideChild(){}
      void init(){}
     public void SetGlobalScale (Vector3 globalScale)
   {
     transform.localScale = Vector3.one;
     transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
   }
  }
