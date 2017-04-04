using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMap : MonoBehaviour 
{
	public Transform water;
	public Transform forest;
	public Transform plains;
	public Transform mountain;
	public Transform sand;

	public int mapSize;

	private Map map;
	private NoiseGenerator heightMap;
	private NoiseGenerator rainMap;

	// Use this for initialization
	void Start () 
	{
		//Assume all tiles are same sized squares
		Bounds tileBounds = water.GetComponent<Renderer> ().bounds;
		map = new Map (mapSize, tileBounds.max.x - tileBounds.min.x);

		heightMap = new NoiseGenerator (1);
		rainMap = new NoiseGenerator (1);

		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				float tileX = ((float) i) / mapSize;
				float tileY = ((float) j) / mapSize;

				MapTile tile;

				//As we get closer to the edge we'll subtract this heightdrop so that the terrain will be island shaped
				float mapMid = mapSize / 2f;
				float heightDrop = (Mathf.Abs(mapMid-i) + Mathf.Abs(mapMid-j)) / mapSize;
				//heightDropFactor = Mathf.Sqrt (heightDropFactor);

				float height = heightMap.GetNoise (tileX, tileY) - heightDrop;
				float rain = rainMap.GetNoise (tileX, tileY);

				//Set tiles on border of map to water tiles
				if (i == 0 || j == 0 || i == mapSize - 1 || j == mapSize - 1) {
					tile = new MapTile (Instantiate (water), Resource.Nothing, TileType.Water);
				}
				else if (height < 0) {
					tile = new MapTile (Instantiate (water), Resource.Nothing, TileType.Water);
				} 
				else if (height < 0.1f) {
					tile = new MapTile (Instantiate (sand), Resource.Nothing, TileType.Sand);
				} 
				else if (height < 0.4f) {
					tile = new MapTile (Instantiate (plains), Resource.Nothing, TileType.Plains);
				}
				else {
					tile = new MapTile (Instantiate (mountain), Resource.Nothing, TileType.Mountain);
				} 

					
				

				map.setTileAt (i,j, tile);
			}
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
