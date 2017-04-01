using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMap : MonoBehaviour 
{
	public Transform water;
	public Transform forest;
	public Transform plains;


	private const int mapSize = 30;

	private Map map;

	// Use this for initialization
	void Start () 
	{
		//Assume all tiles are same sized squares
		Bounds tileBounds = water.GetComponent<Renderer> ().bounds;
		map = new Map (mapSize, tileBounds.max.x - tileBounds.min.x);

		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				MapTile plainTile = new MapTile (Instantiate (plains), Resource.Nothing, TileType.Plains);
				map.setTileAt (i,j, plainTile);
			}
		}

		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				MapTile plainTile = new MapTile (Instantiate (plains), Resource.Nothing, TileType.Plains);
				map.setTileAt (i,j, plainTile);
			}
		}

		for (int j = 0; j < mapSize; j++) 
		{
			MapTile waterTile = new MapTile (Instantiate (water), Resource.Nothing, TileType.Water);
			map.setTileAt (15,j, waterTile);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
