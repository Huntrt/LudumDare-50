using UnityEngine;
using System;

public class Player : MonoBehaviour
{
	public float speed;
	public Vector2 coordinates;
	public event Action onMove;
	//Set this class to singleton
	public static Player i {get{if(_i==null){_i = GameObject.FindObjectOfType<Player>();}return _i;}} static Player _i;

	void Update()
	{
		Moving();
	}

	void Moving()
	{
		//@ Move the player base on direction input multiple with speed
		if(Input.GetKeyDown (KeyCode.UpArrow)) {Move(Vector2.up * speed);}
		if(Input.GetKeyDown (KeyCode.DownArrow)) {Move(Vector2.down * speed);}
		if(Input.GetKeyDown (KeyCode.LeftArrow)) {Move(Vector2.left * speed);}
		if(Input.GetKeyDown (KeyCode.RightArrow)) {Move(Vector2.right * speed);}
	}

	public void Move(Vector2 direction)
	{
		//Find destination node the node at where player moving to
		NodeData destination = Map.i.FindNodeAtCoordinates(coordinates + direction);
		//If there is destination
		if(destination != null)
		{
			//Set coordinates to destination coordinates
			coordinates = destination.coord;
			//Move to destination position
			transform.position = destination.position;
			//Call on move event
			onMove?.Invoke();
		}
	}
}
