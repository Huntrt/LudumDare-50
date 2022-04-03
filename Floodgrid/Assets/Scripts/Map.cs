using System.Collections.Generic;
using UnityEngine;
using System;

public class Map : MonoBehaviour
{
	public Settings settings;
	public GameObject floorPrefab; GameObject group;
	public float floodRate;
	public Vector2 mapSize, nodeScale;
	public List<Node> nodes = new List<Node>();
	public event Action completeGenerate;

#region Classes
	[Serializable] public class Settings 
	{
		public NodeColor emptyNode, floodedNode, freezeNode;
	}
	[Serializable] public class NodeColor {public Color node, border;}
#endregion
	//Set this class singleton
	public static Map i {get{if(_i==null){_i = GameObject.FindObjectOfType<Map>();}return _i;}} static Map _i;

	void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		//Renew the node list
		nodes.Clear(); nodes = new List<Node>(); 
		//Renew the group then name it
		Destroy(group); group = new GameObject(); group.name = "Floor";
		//Create the first new node
		NewNode(0,0);
		//Reduce the map size by half
		Vector2 trueSize = new Vector2(mapSize.x/2, mapSize.y/2);
		//Go through all the X axis of true size
		for (int x = 0; x < trueSize.x; x++)
		{
			//Go through all the Y axis true size
			for (int y = 0; y < trueSize.y; y++)
			{	
				//Create an new node at right X and upper Y    [→ ↑]
				NewNode(x,y); 
				//Create an new node at right X and downward Y [→ ↓]
				NewNode(x,-y);
				//Create an new node at left X and upper Y     [← ↑]
				NewNode(-x,y);
				//Create an new node at left X and downward Y  [← ↓]
				NewNode(-x,-y);
			}
		}
		//Invoke complete generation
		completeGenerate?.Invoke();
	}

	void NewNode(float x, float y)
	{
		//Stop if there already an node at x and y given
		if(FindNodeAtCoordinates(new Vector2(x,y)) != null) {return;}
		//Spawned the floor at x and y
		GameObject spawned = Instantiate(floorPrefab, Vector2.zero, Quaternion.identity);
		//Set the floor scale
		spawned.transform.localScale = nodeScale;
		//Group the spawned floot
		spawned.transform.SetParent(group.transform);
		//Get the node data of floor spawned
		Node data = spawned.GetComponent<Node>();
		//Set data coordinates at x and y given
		data.coord.x = x; data.coord.y = y;
		//Set data position at x and y given increase with scale
		data.position.x = x * nodeScale.x; data.position.y = y * nodeScale.y;
		//Set the spawned floor position at it node data position
		spawned.transform.position = data.position;
		//Add node into list
		nodes.Add(data);
	}

	public Node FindNodeAtCoordinates(Vector2 coord)
	{
		//Go through all the nodes in list then return the nodes at given coordinates if has one
		for (int n = 0; n < nodes.Count; n++) {if(nodes[n].coord == coord) {return nodes[n];}} return null;
	}

	public Node FindNodeAtPosition(Vector2 pos)
	{
		//Go through all the nodes in list then return the nodes at given position if has one
		for (int n = 0; n < nodes.Count; n++) {if(nodes[n].position == pos) {return nodes[n];}} return null;
	}

	public Node[] ScanNode(Vector2 coord)
	{
		//Array for all 4 node to scan
		Node[] scan = new Node[4];
		// Go through all 4 direction of coordinates
		for (int n = 0; n < scan.Length; n++)
		{
			//Find node at each direction
			scan[n] = FindNodeAtCoordinates(coord + DirectionVector(n,1));
		}
		//Return the scan result
		return scan;
	}

	public Vector2 DirectionVector(int index, int amount)
	{
		//@ Return vector depend on index given from 0-3 increase with amount
		if(index == 0) {return new Vector2(0,0 + amount);}
		if(index == 1) {return new Vector2(0,0 - amount);}
		if(index == 2) {return new Vector2(0 - amount,0);}
		if(index == 3) {return new Vector2(0 + amount,0);}
		//Return zero vector if index given are not 0-3
		return Vector2.zero;
	}
}
