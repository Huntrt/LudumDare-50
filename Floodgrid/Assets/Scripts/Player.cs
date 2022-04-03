using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{
	public int moved, speed, recover, _breath; int _recover;
	public Vector2 coordinates;
	public bool drowning, lockMovement, invincible;
	public Item.Type equipAbility;
	public delegate void OnMove(Vector2 moveDirection); public event OnMove onMove;
	[SerializeField] ItemAbility abilites;
	[Header("UI")] public GameObject[] breathCounter; public GameObject itemPanelDisplay; 
	[SerializeField] TextMeshProUGUI itemNameDisplay, itemDescriptionDisplay, pointCounter; 
	[SerializeField] GameObject deadDisplay; [SerializeField] TextMeshProUGUI deadPointDisplay;
	//Set this class to singleton
	public static Player i {get{if(_i==null){_i = GameObject.FindObjectOfType<Player>();}return _i;}} static Player _i;

	void Start()
	{
		//Get the breath amount from counter
		_breath = breathCounter.Length; 
	}
	
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
		//Update the point counter
		pointCounter.text = moved.ToString();
		//Calculated breathing
		Breathing();
		//If the player haven't has ability and there is item on destination then pickup item
		if(equipAbility == Item.Type.none && destination.itemOnNode != null) destination.PickItem();
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

	public void Breathing()
	{
		//If is it drowning
		if(drowning)
		{
			//Lost an breath point
			_breath--;
			//Hide an bubble (prevent it from go to -X)
			if(_breath >= 0) {breathCounter[_breath].SetActive(false);}
			//! If out of breath
			if(_breath <= 0)
			{
				//Update the point display in text
				deadPointDisplay.text = moved.ToString();
				//Deactive player
				gameObject.SetActive(false);
				//Enable die menu
				deadDisplay.SetActive(true);
			}
		}
		//If it is not drowning
		else
		{
			//Recovery breath slowly if lose any
			if(_breath < breathCounter.Length) 
			{
				//Increase recover counter
				_recover++; 
				//If has recover enough and breath haven't maxxed
				if(_recover >= recover && _breath < breathCounter.Length) 
				{
					//Increase breath and reset recover counter
					_breath++; _recover -= _recover;
					//Enable bubble
					breathCounter[_breath-1].SetActive(true);
				}
			}
		}
	}

	public void MoveInDirection(Vector2 direction)
	{
		//If movement not being lock
		if(!lockMovement)
		{
			//If this movement has been block by any border
			bool blocked = false;
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
					//If destination is flooded and not freeze
					if(target.flooded && target.freezeConter == 0) 
					{
						//Player are now drowning
						drowning = true;
					} 
					//If destination are empty or is freeze
					if(!target.flooded || target.freezeConter > 0)
					{
						//Player are no longer drown
						drowning = false;
					}
					//The player has move to target while not invincible
					if(!invincible) {Move(target);}
				}
				//! This way has been blocked
				else {blocked = true;}
			}
			//Call on move event only when not get block
			if(!blocked) {onMove.Invoke(direction);}
		}
		//Cast any abilites that use move despite movement lock
		abilites.MoveCasting(direction);
	}
}
