using UnityEngine;
using System;

public class Player : MonoBehaviour
{
	public int speed;
	public Vector2 coordinates;
	public bool drowning;
	public event Action onMove;
	//Set this class to singleton
	public static Player i {get{if(_i==null){_i = GameObject.FindObjectOfType<Player>();}return _i;}} static Player _i;

	void Update()
	{
		//Only move if paused
		if(!PlayManager.i.pause) {Moving();}
	}

	void Moving()
	{
		//@ Move the player base on direction input multiple with speed
		if(Input.GetKeyDown (KeybinderSystem.i.Up)) {Move(Vector2.up * speed);}
		if(Input.GetKeyDown (KeybinderSystem.i.Down)) {Move(Vector2.down * speed);}
		if(Input.GetKeyDown (KeybinderSystem.i.Left)) {Move(Vector2.left * speed);}
		if(Input.GetKeyDown (KeybinderSystem.i.Right)) {Move(Vector2.right * speed);}
	}

	public void Move(Vector2 direction)
	{
		//Find destination node the node at where player moving to
		Node destination = Map.i.FindNodeAtCoordinates(coordinates + direction);
		//If there is destination
		if(destination != null)
		{
			//Set coordinates to destination coordinates
			coordinates = destination.coord;
			//Move to destination position
			transform.position = destination.position;
			//If destination is flooded 
			if(destination.flooded == true) 
			{
				//Player are now drowning
				drowning = true;
			} 
			//If destination are empty 
			else 
			{
				//Player are no longer drown
				drowning = false;
			}
			//Call on move event
			onMove?.Invoke();
		}
	}
}
