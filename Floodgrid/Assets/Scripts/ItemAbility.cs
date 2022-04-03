using System.Collections.Generic;
using UnityEngine;

public class ItemAbility : MonoBehaviour
{
	Player p; Map m; void Awake() {p = Player.i; m = Map.i;}
	public Item.Type useAbility; 
	GameObject tempIndicator; List<GameObject> tempIndicatorList = new List<GameObject>();
	Node tempNode;
	List<Node> affectsNode = new List<Node>();
	[Header("Run")]
	public int runDistance;
	public GameObject runIndicator;
	[Header("Bomb")]
	public GameObject bombIndicator;
	[Header("Freeze Ray")]
	public int freezeRayDuration;
	public GameObject freezeRayIndicator;

    public void UseAbility(Item.Type use)
    {
        if(use == Item.Type.run) {Run();}
        if(use == Item.Type.bomb) {Bomb(Vector2.zero, true);}
        if(use == Item.Type.freezeRay) {FreezeRay(Vector2.zero);}
    }

	public void MoveCasting(Vector2 dir)
	{
		//If using the run ability then begin run
        if(useAbility == Item.Type.run) {Run(true);}
        if(useAbility == Item.Type.bomb) {Bomb(dir);}
        if(useAbility == Item.Type.freezeRay) {FreezeRay(dir);}
	}

	void Run(bool beginRun = false)
	{
		//Get the run distance
		int d = runDistance;
		//If are using run ability and has begin run
		if(useAbility ==  Item.Type.run && beginRun == true)
		{
			//Set player speed back to 1
			p.speed = 1;
			//No longer use any ability
			useAbility = Item.Type.none;
			ClearList();
			//Complete casting of this ability
			CompleteCast(); return;
		}
		ClearList();
		//Get the starting node under player
		Node starting = m.FindNodeAtCoordinates(p.coordinates);
		//For each of run distance in all direction
		for (int r = 1; r < runDistance+1; r++) for (int a = 0; a < 4; a++)
		{
			//Get the node at starting coordinates outward up to distance in 4 direction
			Node n = m.FindNodeAtCoordinates(starting.coord + m.DirectionVector(a, r));
			//If the node exist
			if(n != null)
			{
				//Create run indicator at this node
				GameObject indi = Instantiate(runIndicator, n.position, Quaternion.identity);
				//Add indicator to it temp list
				tempIndicatorList.Add(indi);
				//Make the indicator look forward at current direction
				indi.transform.up = (indi.transform.position + (Vector3)m.DirectionVector(a,1)) - indi.transform.position;
			}
		}
		//Increase player speed by run distance
		p.speed = runDistance;
		//Are now use run ability
		useAbility = Item.Type.run;
	}

	void Bomb(Vector2 moveDirection, bool dropBoom = false)
	{
		//If are using bomb ability
		if(useAbility == Item.Type.bomb)
		{
			//If drop bomb
			if(dropBoom)
			{
				//Drain the temp node
				tempNode.Drain();
				//Scan the node around temp node
				Node[] scanned = m.ScanNode(tempNode.coord);
				//Drain all the available scanned node 
				for (int s = 0; s < scanned.Length; s++) {if(scanned[s] != null) {scanned[s].Drain();}}
				//Movement is no longer lock
				p.lockMovement = false;
				//Destroy the temporary indicator
				Destroy(tempIndicator);
				//Complete casting of this ability
				CompleteCast(); return;
			}
			//Find target node the node at where player moving to at given direction
			Node target = Map.i.FindNodeAtCoordinates(tempNode.coord + moveDirection);
			//If there is target
			if(target != null)
			{
				//Save the temp node as target then set temp indicator at temp node
				tempNode = target; tempIndicator.transform.position = tempNode.position;
			}
			return;
		}
		//Create the temporary object of bomb indicator at player
		tempIndicator = Instantiate(bombIndicator, p.transform.position, Quaternion.identity);
		//Temporary get the node at player
		tempNode = Map.i.FindNodeAtCoordinates(p.coordinates);
		//Now are using the bomb ability
		useAbility = Item.Type.bomb;
		//Movement is now lock
		p.lockMovement = true;
	}

	void FreezeRay(Vector2 fireDirection)
	{
		//Clear affecting node
		ClearList();
		//Get the starting node at player
		Node starting = Map.i.FindNodeAtCoordinates(p.coordinates);
		//If are using freeze ray ability with an fire direction
		if(useAbility == Item.Type.freezeRay && fireDirection != Vector2.zero)
		{
			//Add the first affect node at starting
			affectsNode.Add(starting);
			//For all the node at direction
			for (int d = 0; d < 1000; d++)
			{
				//Find the node at the fire direction of previous node
				Node finded = Map.i.FindNodeAtCoordinates(affectsNode[d].coord + fireDirection);
				//If finded an node
				if(finded != null)
				{
					//Add the finded node to affect
					affectsNode.Add(finded);
					//Freeze the finded node
					finded.Freeze(freezeRayDuration);
				}
				//Stop find more node if there is no more node at fire direction
				else {break;}
			}
			//Movement is no longer lock
			p.lockMovement = false;
			//Clear affecting node
			ClearList();
			//Complete casting of this ability
			CompleteCast(); return;
		}
		//For each all of distance in this direction
		for (int r = 1; r < 100; r++) for (int a = 0; a < 4; a++)
		{
			//Get the node at starting coordinates outward up to distance in 4 direction
			Node n = m.FindNodeAtCoordinates(starting.coord + m.DirectionVector(a, r));
			//If the node exist
			if(n != null)
			{
				//And is flooded
				if(n.flooded)
				{
					//Create run indicator at this node
					GameObject indi = Instantiate(freezeRayIndicator, n.position, Quaternion.identity);
					//Add indicator to it temp list
					tempIndicatorList.Add(indi);
				}
			}
			//Stop create indicator if no more node left
			else {break;}
		}
		//Now using freeze ray ability
		useAbility = Item.Type.freezeRay;
		//Movement is now lock
		p.lockMovement = true;
	}

	void CompleteCast()
	{
		//No longer use any ability
		useAbility = Item.Type.none;
		//Clear player item
		p.ClearItem();
	}

	void ClearList() 
	{
		for (int t = 0; t < tempIndicatorList.Count; t++) {Destroy(tempIndicatorList[t]);}
		affectsNode.Clear(); affectsNode = new List<Node>();
		tempIndicatorList.Clear(); tempIndicatorList = new List<GameObject>();
	}

	public void ClearAbility()
	{
		//Player no longer has ability
		p.equipAbility = Item.Type.none;
		//Hide the item panel
		p.itemPanelDisplay.SetActive(false);
	}
}
