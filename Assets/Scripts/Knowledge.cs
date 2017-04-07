using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge {
	//TODO have list of resource points and base location
	private Map map;
	public bool[,] isRevealedTile;
    //private bool[,] isFrontier;
    private List<Frontier> frontiers;
    public bool canCrossMountains = false;

    public Frontier findNextFrontier(Position2D curPos)
    {
        Frontier newFrontier;
        int minIndex = 0;
        int minDiff = map.mapSize;
        int maxProbIndex = 0;
        float maxProb = 0;
        if(frontiers.Count == 0)
        {
            Debug.Log(frontiers);
        }
        for(int i = 0; i < frontiers.Count; i++)
        {
			if(Mathf.Abs(frontiers[i].pos.x - curPos.x) + Mathf.Abs(frontiers[i].pos.y - curPos.y) < minDiff)
            {
                minIndex = i;
                minDiff = frontiers[i].pos.x - curPos.x + frontiers[i].pos.y - curPos.y;
            }
            if(maxProb < frontiers[i].probability)
            {
                maxProb = frontiers[i].probability;
                maxProbIndex = i;
            }
        }

       //  newFrontier = frontiers[minIndex];
        //newFrontier = frontiers[maxProbIndex];
        //Note this is one way of making it random, not sure if it's useful
   
        if (minIndex == maxProbIndex)
        {
            newFrontier = frontiers[maxProbIndex];
        }
        else if (Random.Range(0f, 1f) < maxProb)
        {
            newFrontier = frontiers[maxProbIndex];
        }
        else
        {
            newFrontier = frontiers[minIndex];
        }
     
   


        return newFrontier;
    }

    public class Frontier
    {
        public Position2D pos { get; set; }
        public float probability { get; private set; }

        public Frontier (Position2D pos, int mapSize, bool north, bool east)
        {
            this.pos = pos;
            this.probability = calculateProb(mapSize, north, east);
        }

        private float calculateProb(int mapSize, bool north, bool east)
        {
            float prob = 0;
            float difX, difY;

            if (east)
            {
                difX = Mathf.Abs(mapSize - pos.x) ;
            }else
            {
                difX = pos.x;
            }

            if (north)
            {
                difY = Mathf.Abs(mapSize - pos.y);
            }else
            {
                difY = pos.y;
            }

            prob = (difX * difY) / (mapSize * mapSize);

            

            return prob;
        }

    }

	public Knowledge(Map map)
	{
		this.map = map;
		isRevealedTile = new bool[map.mapSize, map.mapSize];

        frontiers = new List<Frontier>();

        for (int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                isRevealedTile[i, j] = false;
            }
        }
	}

    private void removeFrontier(int x, int y)
    {
        Position2D temp = new Position2D(x, y);
        for(int i = 0; i < frontiers.Count; i++)
        {
            if(frontiers[i].pos == temp)
            {
                frontiers.RemoveAt(i);
                return;
            }
        }
    }

	private bool isFrontier(int x, int y)
	{
		List<Position2D> neighbors = new List<Position2D> ();
		neighbors.Add (new Position2D (x+1, y));
		neighbors.Add (new Position2D (x-1, y));
		neighbors.Add (new Position2D (x, y+1));
		neighbors.Add (new Position2D (x, y-1));

		foreach (Position2D p in neighbors) 
		{
			if (p.x >= 0 && p.x < map.mapSize && p.y >= 0 && p.y < map.mapSize) 
			{
				if (!isRevealedTile [p.x, p.y])
					return true;
			}
		}
		return false;
	}

    public void addFrontier(Frontier f)
    {
		if (isFrontier (f.pos.x, f.pos.y)) {
			if (f.probability > 0 && map.isPassable (map.getTileTypeAt (f.pos.x, f.pos.y), canCrossMountains)) {
				frontiers.Add (f);
			}  
		}
    }

	public void discoverTiles(int x, int y){
		if (isRevealedTile[x, y] && !isFrontier(x,y))
        {
            removeFrontier(x, y);
        }

        isRevealedTile[x, y] = true;
        

		List<Position2D> visibleNeighbors = new List<Position2D> ();
		visibleNeighbors.Add (new Position2D (x+1, y));
		visibleNeighbors.Add (new Position2D (x-1, y));
		visibleNeighbors.Add (new Position2D (x, y+1));
		visibleNeighbors.Add (new Position2D (x, y-1));
		visibleNeighbors.Add (new Position2D (x+2, y));
		visibleNeighbors.Add (new Position2D (x-2, y));
		visibleNeighbors.Add (new Position2D (x, y+2));
		visibleNeighbors.Add (new Position2D (x, y-2));
		visibleNeighbors.Add (new Position2D (x+1, y+1));
		visibleNeighbors.Add (new Position2D (x+1, y-1));
		visibleNeighbors.Add (new Position2D (x-1, y+1));
		visibleNeighbors.Add (new Position2D (x-1, y-1));

		foreach (Position2D p in visibleNeighbors) 
		{
			if (p.x >= 0 && p.x < map.mapSize && p.y >= 0 && p.y < map.mapSize) 
			{
				if (!isRevealedTile[p.x, p.y])
				{
					isRevealedTile[p.x, p.y] = true;

					bool north = p.y > y;
					bool east = p.x > x;
					addFrontier(new Frontier(new Position2D(p.x, p.y), map.mapSize, north, east));
				}
				else if (!isFrontier(p.x, p.y))
				{
					removeFrontier(p.x, p.y);
				}
			}	
		}
    }

	//GetTileInfo
}
