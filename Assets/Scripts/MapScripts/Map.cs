using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map{

	public int mapSize { get; private set;}
	private MapTile[,] mapGrid;

    //Store list of resources so they can quickly be accessed without searching the map
    private Dictionary<Resource, List<Position2D>> resourcePositions;

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
        removeResource(x, y);

		mapGrid [x, y].addResource (r);

        if (!resourcePositions.ContainsKey(r))
        {
            resourcePositions.Add(r, new List<Position2D>());
        }

        resourcePositions[r].Add(new Position2D(x, y));

	}

	public void RemoveAllResources()
	{
		for(int i=0; i<mapSize; i++)
		{
			for(int j=0; j<mapSize; j++)
			{
                removeResource(i, j);
			}
		}
	}

    public void removeResource(int x, int y)
    {
        Resource prev = mapGrid[x, y].getResource();
        if (prev != Resource.Nothing)
        {
            mapGrid[x, y].removeResource();
            resourcePositions[prev].Remove(new Position2D(x,y));
        }
    }

    public void setTileAt(int x, int y, MapTile newTile)
	{
		mapGrid[x, y] = newTile;
	}

	public Map(int mapSize)
	{
		this.mapSize = mapSize;
		mapGrid = new MapTile[mapSize, mapSize];
        resourcePositions = new Dictionary<Resource, List<Position2D>>();
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

    public List<Position2D> getResourcePositions(Resource r)
    {
        if (resourcePositions.ContainsKey(r))
        {
            return resourcePositions[r];
        }
        else
        {
            return new List<Position2D>();
        }
    }
}
