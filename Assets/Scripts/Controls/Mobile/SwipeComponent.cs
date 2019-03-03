using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeComponent : MonoBehaviour 
{
	private bool tap,swipeLeft,swipeRight,swipeUp,swipeDown;
	private bool isDraggin;
	private Vector2 startTouch,swipeDelta;
	public delegate void OnTapOnObject(Transform tabObject);
	public LayerMask touchLayer;
	OnTapOnObject  getTapObject; 

	private void Update()
	{
		tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

		#region  Standalone Inputs
			if(Input.GetMouseButtonDown(0))
			{
				tap = true;
				isDraggin =true;
				startTouch = Input.mousePosition;
				
			}
			else if (Input.GetMouseButtonUp(0))
			{
				findTouchObject();
				Reset();
			}
		#endregion

		#region  Mobile Inputs
			if(Input.touches.Length >0)
			{
				if(Input.touches[0].phase == TouchPhase.Began)
				{
					tap =true;
					isDraggin = true;
					startTouch =Input.touches[0].position;
					
				}
				else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
				{
					findTouchObject();
					Reset();
				}
			}
		#endregion 

		// Calculate the distance
		if(isDraggin)
		{
			if(Input.touches.Length > 0)
			{
				swipeDelta = Input.touches[0].position - startTouch;
			}
			else if (Input.GetMouseButton(0))
			{
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
			}
		}

		// Dead zone Cross 
		if(swipeDelta.magnitude > 50)
		{
			float x = swipeDelta.x;
			float y= swipeDelta.y;

			if(Mathf.Abs(x) > Mathf.Abs(y))
			{
				// Left or right
				if(x <0)
				{
					swipeLeft = true;
				}
				else
				{
					swipeRight = true;
				}
			}
			else
			{
				// Up or down
				if(y > 0)
				{
					swipeUp = true;
				}
				else
				{
					swipeDown = true;
				}
			}
		
			Reset();
		}

	}

	private void Reset()
	{
		startTouch = swipeDelta = Vector2.zero;
		isDraggin = false;
	}
	public Vector2 SwipeDelta{get{return swipeDelta;}}
	public bool SwipeLeft {get{return swipeLeft;}}
	public bool SwipeRight {get{return swipeRight;}}
	public bool SwipeUp {get{return swipeUp;}}
	public bool SwipeDown {get{return swipeDown;}}

	public Vector2 TapPositon {get{return startTouch;}}
	public bool isTap()
	{
		return tap;
	}

	public bool isDragginOn()
	{
		return isDraggin;
	}


	private void findTouchObject()
	{
		if( swipeDelta.magnitude < 125)
		{
			RaycastHit hit;
		 	Ray ray = Camera.main.ScreenPointToRay(startTouch);
			if(Physics.Raycast(ray,out hit,100.0f,touchLayer))
			{
				getTapObject(hit.transform);
			}		
		}
	}

	public void setGetTapObjectFunction(OnTapOnObject getTapObject)
	{
		this.getTapObject += getTapObject;
	}

}
