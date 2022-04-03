using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{
	public int moved;
	public int speed;
	public Vector2 coordinates;
	public bool drowning, lockMovement;
	public Item.Type equipAbility;
	public delegate void OnMove(Vector2 moveDirection); public event OnMove onMove;
	[SerializeField] ItemAbility abilites;
	[Header("UI")] public GameObject itemPanelDisplay; 
	[SerializeField] TextMeshProUGUI itemNameDisplay, itemDescriptionDisplay; 
	//Set this class to singleton
	public static Player i {get{if(_i==null){_i = GameObject.FindObjectOfType<Player>();}return _i;}} static Player _i;

	void Update()
	{
		/// Don't ran anything if game pauses
		if(PlayManager.i.pause) {return;}
		if(Input.GetKeyDown(KeybinderSystem.i.Use)) {UseItem();}
		MovingInput();
	}

	void MovingInput()
	{
		//@ Move the player base on direction input multiple with speed
		if(Input.GetKeyDown(KeybinderSystem.i.Up)) {MoveInDirection(Vector2.up);}
		if(Input.GetKeyDown(KeybinderSystem.i.Down)) {MoveInDirection(Vector2.down);}
		if(Input.GetKeyDown(KeybinderSystem.i.Left)) {MoveInDirection(Vector2.left);}
		if(Input.GetKeyDown(KeybinderSystem.i.Right)) {MoveInDirection(Vector2.right);}
	}

	public void Move(Node destination)
	{
		//Player has move one more time
		moved++;
		//If the player haven't has ability and there is item on destination then pick it up
		if(equipAbility == Item.Type.none && destination.itemOnNode != null) {destination.PickItem();}
	}

	public void Equiping(Item pick)
	{
		//Get the pickup item's aiblity
		equipAbility = pick.type;
		//@ Update the item name and description in display
		itemNameDisplay.text = pick.type.ToString(); 
		itemDescriptionDisplay.text = pick.Description;
		//Show the item panel
		itemPanelDisplay.SetActive(true);
	}

	public void UseItem()
	{
		//If currently equip an ability
		if(equipAbility != Item.Type.none) 
		{
			//Use the equip ability
			abilites.UseAbility(equipAbility);
		}
	}

	public void ClearItem()
	{
		//No longer equip any ability
		equipAbility = Item.Type.none;
		//Hide the item panel
		itemPanelDisplay.SetActive(false);
	}

	public void MoveInDirection(Vector2 direction)
	{
		//If movement not being lock
		if(!lockMovement)
		{
			//Move multiple time depend on speed
			for (int s = 0; s < speed; s++)
			{
				//Don't move if movement is lock
				if(lockMovement) {break;}
				//Find target node the node at where player moving to at given direction
				Node target = Map.i.FindNodeAtCoordinates(coordinates + direction);
				//If there is target
				if(target != null)
				{
					//Set coordinates to destination coordinates
					coordinates = target.coord;
					//Move to destination position
					transform.position = target.position;
					//If destination is flooded 
					if(target.flooded == true) 
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
					//The player has move to target
					Move(target);
				}
			}
			//Call on move event only 
			onMove.Invoke(direction);
		}
		//Cast any abilites that use move despite movement lock
		abilites.MoveCasting(direction);
	}
}
