using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{
	public int moved;
	public int speed;
	public Vector2 coordinates;
	public bool drowning;
	public Item.Type ability;
	public event Action onMove;
	[Header("UI")] [SerializeField] GameObject itemPanelDisplay; 
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
		if(Input.GetKeyDown(KeybinderSystem.i.Up)) {MoveInDirection(Vector2.up * speed);}
		if(Input.GetKeyDown(KeybinderSystem.i.Down)) {MoveInDirection(Vector2.down * speed);}
		if(Input.GetKeyDown(KeybinderSystem.i.Left)) {MoveInDirection(Vector2.left * speed);}
		if(Input.GetKeyDown(KeybinderSystem.i.Right)) {MoveInDirection(Vector2.right * speed);}
	}

	public void Move(Node destination)
	{
		//Player has move one time
		moved++;
		//If the player haven't has ability and there is item on destination then pick it up
		if(ability == Item.Type.none && destination.itemOnNode != null) {destination.PickItem();}
	}

	public void Equiping(Item pick)
	{
		//Get the pickup item's aiblity
		ability = pick.type;
		//@ Update the item name and description in display
		itemNameDisplay.text = pick.type.ToString(); 
		itemDescriptionDisplay.text = pick.Description;
		//Show the item panel
		itemPanelDisplay.SetActive(true);
	}

	public void UseItem()
	{
		print(ability.ToString());
		//No longer has ability
		ability = Item.Type.none;
		//Hide the item panel
		itemPanelDisplay.SetActive(false);
	}

	public void MoveInDirection(Vector2 direction)
	{
		//Find target node the node at where player moving to at given direction
		Node target = Map.i.FindNodeAtCoordinates(coordinates + direction);
		//If there is destination
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
			//Call on move event
			onMove?.Invoke();
		}
		//The player has move to target
		Move(target);
	}
}
