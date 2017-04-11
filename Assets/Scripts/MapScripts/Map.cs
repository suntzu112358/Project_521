using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map{

	public int mapSize { get; private set;}
	private MapTile[,] mapGrid;

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
		if (x < 0 || x >= mapSize)
		{
			return Resource.Nothing;
		}
		if (y < 0 || y >= mapSize)
		{
			return Resource.Nothing;
		}
        return mapGrid[x, y].getResource();
    }

	public void AddResource(int x, int y, Resource r)
	{
		mapGrid [x, y].addResource (r);

	}

	public void RemoveAllResources()
	{
		for(int i=0; i<mapSize; i++)
		{
			for(int j=0; j<mapSize; j++)
			{
                mapGrid[i, j].removeResource();
            }
		}
	}

    public void removeResource(int x, int y)
    {
        mapGrid[x, y].removeResource();
    }

    public void setTileAt(int x, int y, MapTile newTile)
	{
		mapGrid[x, y] = newTile;
	}

	public Map(int mapSize)
	{
		this.mapSize = mapSize;
		mapGrid = new MapTile[mapSize, mapSize];
    }

    public bool isPassable(TileType type, bool canCrossMountians)
    {
        if (type == TileType.Boulder)
        {
            return false;
        }
        else if (type == TileType.Forest)
        {
            return true;
        }
        else if (type == TileType.Water)
        {
            return false;
        }
        else if (type == TileType.Sand)
        {
            return true;
        }
        else if (type == TileType.Plains)
        {
            return true;
        }
        else if (type == TileType.Bridge)
        {
            return true;
        }
        else if (type == TileType.Mountain)
        {
            return canCrossMountians;
        }
        else if (type == TileType.Base)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
