using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map{

	public int mapSize { get; private set;}
	private float tileSize;
	private MapTile[,] mapGrid;

	private const int terrainDepth = 1;
	private const int resourceDepth = 0;
    private const int minionDepth = -1;

    public TileType getTileTypeAt(int x, int y)
    {
        if (x < 0 || x >= mapSize)
        {
            return TileType.Water;
        }
        if (y < 0 || y >= mapSize)
        {
            return TileType.Water;
        }
        return mapGrid[x, y].getTileType();
    }

    public Resource getResource(int x, int y)
    {
        return mapGrid[x, y].getResource();
    }

    public void setTileAt(int x, int y, MapTile newTile)
	{
		if (mapGrid [x, y] != null) {
			mapGrid [x, y].Destroy ();
		}

		mapGrid[x, y] = newTile;
		newTile.setPosition (new Vector3 (x*tileSize, y*tileSize, terrainDepth));
	}

	public Map(int mapSize, float tileSize)
	{
		this.mapSize = mapSize;
		this.tileSize = tileSize;
		mapGrid = new MapTile[mapSize, mapSize];
	}
		

}
