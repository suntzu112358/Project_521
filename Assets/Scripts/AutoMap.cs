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

	private float[,] heightTable;

	// Use this for initialization
	void Start () 
	{
		//Assume all tiles are same sized squares
		Bounds tileBounds = water.GetComponent<Renderer> ().bounds;
		map = new Map (mapSize, tileBounds.max.x - tileBounds.min.x);

		heightMap = new NoiseGenerator (1);
		rainMap = new NoiseGenerator (1);
		heightTable = new float[mapSize, mapSize];

		//Candidates for locations to start rivers
		List<Position2D> riverStartCandidates = new List<Position2D>();

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

				heightTable [i, j] = height;

				//Set tiles on border of map to water tiles
				if (i == 0 || j == 0 || i == mapSize - 1 || j == mapSize - 1) {
					tile = new MapTile (Instantiate (water), Resource.Nothing, TileType.Water);
				}
				else if (height <= 0) {
					tile = new MapTile (Instantiate (water), Resource.Nothing, TileType.Water);
				} 
				else if (height < 0.1f) {
					tile = new MapTile (Instantiate (sand), Resource.Nothing, TileType.Sand);
				} 
				else if (height < 0.4f) 
				{
					if (rain < 0.6f)
						tile = new MapTile (Instantiate (plains), Resource.Nothing, TileType.Plains);
					else
						tile = new MapTile (Instantiate (forest), Resource.Nothing, TileType.Forest);
				}
				else {
					tile = new MapTile (Instantiate (mountain), Resource.Nothing, TileType.Mountain);
					if (rain >= 0.6f)
						riverStartCandidates.Add (new Position2D (i, j));
				} 

				map.setTileAt (i,j, tile);
			}
		}


		//Randomly pick river start locations. After a location is picked, eliminate other candidates that are too close to it
		List<Position2D> riverStartLocations = new List<Position2D>();
		int minRiverDistance = mapSize / 8;
		while (riverStartCandidates.Count > 0) 
		{
			int next = Random.Range (0, riverStartCandidates.Count);
			Position2D nextPos = riverStartCandidates [next];

			bool tooClose = false;
			//Check if too close to an already selected river tile
			foreach (Position2D riverPos in riverStartLocations) 
			{
				//Check distance in manhattan distance
				if (Mathf.Abs (nextPos.x - riverPos.x) + Mathf.Abs (nextPos.y - riverPos.y) < minRiverDistance) 
				{
					tooClose = true;
					break;
				}
			}

			if(!tooClose)
				riverStartLocations.Add(nextPos);
			
			riverStartCandidates.RemoveAt(next);
		}

		//Dig rivers
		foreach (Position2D riverPos in riverStartLocations) 
		{
			List<Position2D> river = new List<Position2D>();
			Position2D currentPos = riverPos;
			while (map.getTileTypeAt (currentPos.x, currentPos.y) != TileType.Water) 
			{
				river.Add (currentPos);
				List<Position2D> neighbors = getRiverNeighbors (currentPos, river);

				float minHeight = Mathf.Infinity;

				if (neighbors.Count == 0)
					break;
				
				Position2D minPos = new Position2D();
				foreach (Position2D n in neighbors) 
				{
					float newHeight = heightTable [n.x, n.y];
					if (newHeight < minHeight) {
						minHeight = newHeight;
						minPos = n;
					}
				}
				currentPos = minPos;
			}

			foreach (Position2D nextRiv in river) 
			{
				heightTable[nextRiv.x, nextRiv.y] = 0;
				map.setTileAt(nextRiv.x, nextRiv.y, new MapTile(Instantiate(water), Resource.Nothing, TileType.Water));
			}
		} 
	}

	//Returns a list of possible locations the river could go from a given tile
	private List<Position2D> getRiverNeighbors(Position2D current, List<Position2D> river)
	{
		Position2D upPosition = new Position2D(current.x, current.y + 1);
		Position2D downPosition = new Position2D(current.x, current.y - 1);
		Position2D rightPosition = new Position2D(current.x + 1, current.y);
		Position2D leftPosition = new Position2D(current.x - 1, current.y);

		List<Position2D> neighbors = new List<Position2D> ();

		if (upPosition.y < mapSize && !river.Contains (upPosition))
			neighbors.Add (upPosition);
		if (downPosition.y >=0 && !river.Contains (downPosition))
			neighbors.Add (downPosition);
		if (rightPosition.x < mapSize && !river.Contains (rightPosition))
			neighbors.Add (rightPosition);
		if (leftPosition.x >= 0 && !river.Contains (leftPosition))
			neighbors.Add (leftPosition);
		
		return neighbors;
	}
}
