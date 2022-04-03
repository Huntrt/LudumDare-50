using UnityEngine;

public class Node : MonoBehaviour
{
	public bool flooded; int freezeConter; [HideInInspector] bool needToFlood;
	public Vector2 coord, position;
	public Item itemOnNode; int despawnCounter;
	public int neighboursCount; public Node[] neighbours;
	public GameObject nodeObject; public SpriteRenderer nodeRender, borderRender;
	public Map.NodeColor nodeColor;
	Map m;

	//Get the map
	void Awake() {m = Map.i; neighbours = new Node[4];}

	void Start()
	{
		//Get the object this node 
		nodeObject = gameObject;
		//Get the sprite render object of this node
		nodeRender = gameObject.GetComponent<SpriteRenderer>();
		//Get the grid render object on the first child of this node
		borderRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		//Change color to empty color
		ColoringNode(m.settings.emptyNode);
	}

	void OnEnable() 
	{
		//Check border upon map complete generation
		m.completeGenerate += GetNeighbours;
		//Flood and despawn item when player move
		Player.i.onMove += Flood; Player.i.onMove += DespawnItem;
	}

	void GetNeighbours()
	{
		//Get the scanned node
		Node[] scanned = m.ScanNode(coord);
		//Go through all the scanned node 
		for (int s = 0; s < scanned.Length; s++) 
		{
			//If has scan an node
			if(scanned[s] != null) 
			{
				//Save the scanned as neighbour
				neighbours[s] = scanned[s];
				//Increase neighbours count
				neighboursCount++;
			}
		}
	}

	public void Flood(Vector2 dir)
	{
		//If node is freezing
		if(freezeConter > 0) 
		{
			//Decrease counter
			freezeConter--;
			//If freeze counter are over
			if(freezeConter == 0)
			{
				//Is is now back to flood
				flooded = true; needToFlood = true;
				//Change node color to flood color
				ColoringNode(m.settings.floodedNode);
			}
			//Don't do anything if it freeze
			return;
		}
		//If this node it need to flood OR it on the border
		if(needToFlood || (flooded == false && neighboursCount < 4))
		{	
			//If successful flood with rate
			if(m.floodRate >= Random.Range(0f,100f))
			{
				//Is is now flood
				flooded = true;
				//Change color to flood color
				ColoringNode(m.settings.floodedNode);
			}
			//If this node has flooded
			if(flooded)
			{
				//No longer need to flood
				needToFlood = false;
				//Go through all the neighbours node
				for (int n = 0; n < 4; n++)
				{	
					//If there is neighbours then that neighbours haven't flooded
					if(neighbours[n] != null) if(!neighbours[n].flooded)
					{
						//That neighbours is now need to flood
						neighbours[n].needToFlood = true;
					}
				}
			}
		}
	}

	public void Drain() 
	{
		//If this node is flooded
		if(flooded)
		{	
			//No longer flooded
			flooded = false;
			//Reset color back to empty
			ColoringNode(m.settings.emptyNode);
		}
	}

	public void Freeze(int duration)
	{
		//Only freeze not that has been flood
		if(flooded)
		{
			//Set freeze counter as duration
			freezeConter = duration;
			//Change node color to freeze color
			ColoringNode(m.settings.freezeNode);
		}
	}

	public void AddItem(GameObject item)
	{
		//Spawn the represent item at position
		GameObject added = Instantiate(item, position, Quaternion.identity);
		//Reset the despawn counter
		despawnCounter = ItemSpawner.i.despawnTurn;
		//This node now has item
		itemOnNode = added.GetComponent<Item>();
	}

	public void PickItem()
	{
		//The player now equip the item on node
		Player.i.Equiping(itemOnNode);
		//Destroy the item object then this node no longer has item
		Destroy(itemOnNode.gameObject); itemOnNode = null;
	}

	void DespawnItem(Vector2 dir)
	{
		//If there is an item on node
		if(itemOnNode != null)
		{
			//Decrease depawn counter by
			despawnCounter--;
			//Despawn the item object then this node no longer has item
			if(despawnCounter == 0) {Destroy(itemOnNode.gameObject); itemOnNode = null;}
		}
	}

	public void ColoringNode(Map.NodeColor color) 
	{
		nodeRender.color = color.node; borderRender.color = color.border;
		nodeColor = color;
	}

	void OnDisable()
	{
		m.completeGenerate -= GetNeighbours;
		if(Player.i != null) {Player.i.onMove -= Flood; Player.i.onMove -= DespawnItem;}
	}
}