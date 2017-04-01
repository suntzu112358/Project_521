using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge {
	//TODO have list of resource points and base location
	private Map map;
	private bool[,] isRevealedTile;

	public Knowledge(Map map){
		this.map = map;
		isRevealedTile = new bool[map.mapSize][map.mapSize];

		for (int i = 0; i < map.mapSize; i++)
			for (int j = 0; j < map.mapSize; j++)
				isRevealedTile [i, j] = false;
	}

	public void discoverTile(int x, int y){
		isRevealedTile [x, y] = true;
	}

	//GetTileInfo
}
