using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fill :Ifill
{
    // Start is called before the first frame update
    
    void Start()
    {
         init();


    }
 

    
    void Update()
    {
      if(activerAnimation){
        animation();//towards right
      }
     // Debug.Log("fill is "+activerAnimation);
     
        
       
    }
     

      void animation(){ //animations towards left or right
         spriteRenderer.enabled=true;
         transform.position=Vector2.MoveTowards(transform.position,Destination,speed*Time.deltaTime);
          if(transform.position==Destination){//end of animation
            //Debug.Log("end");
            transform.position=initPos;
            hideChild();
            activerAnimation=false;  }
       
     }

     void hideChild(){
        spriteRenderer.enabled=false;
     }
     void init(){
       Bounds bounds = transform.parent.GetComponent<Renderer>().bounds;
          initPos=transform.position;
             // Get the end of the object along the x-axis
            end = bounds.max.x;
            start=bounds.min.x;
             spriteRenderer = GetComponent<SpriteRenderer>();
             hideChild();
            endOfBus=new Vector3(end, transform.position.y);
            startOfBus=new Vector3(start,transform.position.y);
            Destination=endOfBus;
            if(this.gameObject.name=="fill2"){
              Destination=startOfBus;
            }

     }
     
 
}
