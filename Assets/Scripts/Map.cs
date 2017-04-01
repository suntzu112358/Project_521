using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map{

	public const int mapSize = 50;
	private MapTile[,] mapGrid;


	public MapTile getTileAt (int x, int y) {
		//TODO: check bounds
		return MapTile[x, y];
	}

	public void changeTileAt(int x, int y, MapTile newTile){
		MapTile[x, y] = newTile;
	}

	private Map(){
		mapGrid = new MapTile[mapSize, mapSize];
	}

	public static Map generateMap(){

	}
		
}
