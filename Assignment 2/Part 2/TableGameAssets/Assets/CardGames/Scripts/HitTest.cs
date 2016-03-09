using UnityEngine;
using System.Collections;

public class HitTest : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
	{
     
		//loop through all registered touches
		for (int i = 0; i < Input.touchCount; ++i) 
		{
			Vector2 touchPosition = Input.GetTouch(i).position;
        	if (Input.GetTouch(i).phase == TouchPhase.Began) 
        	{    
            	RaycastHit hit;
			 	Ray ray = Camera.main.ScreenPointToRay (touchPosition);
				
				if (Physics.Raycast (ray, out hit, 10000))
				{
                    if (hit.transform.gameObject.name == "Card_h13")
                        hit.transform.gameObject.GetComponent<Animation>().Play("FlipY");
                    else
                        hit.transform.gameObject.transform.Rotate(0, 180, 0);	
				}		
        	}
    	}
	
		// for mouse interface
		if (Input.GetMouseButtonDown(0)) 
        {    
            	RaycastHit hit;
			 	Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 10000))
				{
                    if (hit.transform.gameObject.name == "Card_h13")
                        hit.transform.gameObject.GetComponent<Animation>().Play("FlipY");
                    else
					    hit.transform.gameObject.transform.Rotate (0, 180, 0);	
				}
 	
        }
	}
}
