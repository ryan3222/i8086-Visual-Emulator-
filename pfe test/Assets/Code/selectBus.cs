using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectBus : bus
{
       private Color newColor=Color.red;
    private Color originalColor;
    private Renderer renderer;


       void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        originalColor = renderer.material.color;
        
    }

    void Update()
    {  
        renderer = GetComponent<SpriteRenderer>();
      
        isAnimated=renderer.material.color!=originalColor;
    }
    public override void animate(int time){
          isAnimated=true;
          SetColorForTime(time);
    }



    public void select(){
     
          renderer.material.color = newColor;       
        
    }

    public void deselect(){
        
        
           renderer.material.color = originalColor;
        
    }

     public void SetColorForTime(float time)
    {
        StartCoroutine(SetColorCoroutine(time));
    }
     public void SetColorForTime()
    {
           StartCoroutine(SetColorCoroutine(1));
    }

    IEnumerator SetColorCoroutine(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            renderer.material.color = Color.Lerp(originalColor, newColor, 1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        renderer.material.color = originalColor;
    }



}  

