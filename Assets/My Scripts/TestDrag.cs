using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]

public class TestDrag : MonoBehaviour 
{
	/*
	private Vector3 screenPoint;
	private Vector3 offset;
	private float newPosX, newPosY;
	
	void Start(){
		
	}
	
	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;

	}
	
	void OnMouseUp()
	{
		if (transform.position.x % 0.5f != 0 || transform.position.y % 0.5f != 0){
			Debug.Log("snapping!");
			
			newPosX = 2*transform.position.x;
			newPosX = Mathf.Round(newPosX);
			newPosX /= 2;
			
			newPosY = 2*transform.position.y;
			newPosY = Mathf.Round(newPosY);
			newPosY /= 2;
			
			transform.position = new Vector3(newPosX,newPosY,0f);
		}
	}
	*/
}