using UnityEngine;
using System;

public class ItemSpawner : MonoBehaviour
{
    public float spawnRate, spawnFrequent, spawnAmount;
	[SerializeField] SpawnData[] spawns; 
	//List to spawn an item using it rarity
	[Serializable] class SpawnData {public GameObject item; public float rarity;} 
	Map m; Player p;

	void Start()
	{
		m = Map.i; p = Player.i;
		//Spawn when player move
		p.onMove += Spawn;
	}

	void Spawn()
	{
		//Don't spawn item if player move not enough frequent or it first move
		if(((p.moved % spawnFrequent) != 0) || p.moved == 0) {return;}
		//Amount has spawn in this session
		int spawned = 0;
		//Go through all the nodes in map
		for (int node = 0; node < m.nodes.Count; node++)
		{
			//Suffle an random node in nodes list to spawn item at
			int n = UnityEngine.Random.Range(node, m.nodes.Count);
			//Don't countine to spawn item if has spawned all the amount has
			if(spawned >= spawnAmount) {break;}
			//If successful spawned with rate and the node that spawn on haven't has item
			if(spawnRate >= UnityEngine.Random.Range(0f,100f) && m.nodes[n].itemOnNode == null)
			{
				//Get the total rarity of all the spawn item
				float total = 0; for (int s = 0; s < spawns.Length; s++) {total += spawns[s].rarity;}
				//Get an random number in total
				float chance = UnityEngine.Random.Range(0, total);
				//Go through all the item to spawn
				for (int i = 0; i < spawns.Length; i++)
				{
					//If the chance successful decrease with spawn rarity to zero
					if(chance - spawns[i].rarity <= 0)
					{
						//Add the spawn item to node
						m.nodes[n].AddItem(spawns[i].item);
						//Has spawned an node
						spawned++;
						//Exit the spawn loop
						break;
					}
					//If the chance fail to decrease with this spawn rarity to zero
					else
					{
						//Decrease chance with failed rarity
						chance -= spawns[i].rarity;
					}
				}
			}
		}
	}

	void OnDisable() {if(p != null) p.onMove -= Spawn;}
}
