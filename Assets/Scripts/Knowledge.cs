using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge {
	//TODO have list of resource points and base location
	private Map map;
	public bool[,] isRevealedTile;
    private bool[,] isFrontier;
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
            if(frontiers[i].pos.x - curPos.x + frontiers[i].pos.y - curPos.y < minDiff)
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

	public Knowledge(Map map){
		this.map = map;
		isRevealedTile = new bool[map.mapSize, map.mapSize];
        isFrontier = new bool[map.mapSize, map.mapSize];

        frontiers = new List<Frontier>();

        for (int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                isRevealedTile[i, j] = false;
                isFrontier[i, j] = false;
            }
        }
	}

    public void removeFrontier(int x, int y)
    {
        isFrontier[x , y] = false;
        Position2D temp = new Position2D(x, y) ;
        for(int i = 0; i < frontiers.Count; i++)
        {
            
            if(frontiers[i].pos == temp)
            {
                frontiers.Remove(frontiers[i]);
                return;
            }
        }
    }

    public void addFrontier(Frontier f)
    {
        if (f.probability > 0 && map.isPassable(map.getTileTypeAt(f.pos.x, f.pos.y), canCrossMountains))
        {
            frontiers.Add(f);
            isFrontier[f.pos.x, f.pos.y] = true;
        }   
    }

	public void discoverTiles(int x, int y){
        if (isRevealedTile[x, y])
        {
            removeFrontier(x, y);
        }else
        {
            isRevealedTile[x, y] = true;
        }

        for(int i = 1; i < 3; i++)
        {
            if (i == 2)
            {
                if (x + i < map.mapSize)
                {
                    if (!isRevealedTile[x + i, y])
                    {
                        isRevealedTile[x + i, y] = true;
                        addFrontier(new Frontier(new Position2D(x + i, y), map.mapSize, true, true));
                    }else if (isFrontier[x + i, y])
                    {
                        //removeFrontier(x + i, y);
                    }
                   

                }
                if (y + i < map.mapSize)
                {
                    if (!isRevealedTile[x, y + i])
                    {
                        isRevealedTile[x, y + i] = true;
                        addFrontier(new Frontier(new Position2D(x, y + i), map.mapSize, true, true));
                    }
                    else if (isFrontier[x , y + i])
                    {
                        //removeFrontier(x , y+1);
                    }

                }
                if (x - i >= 0)
                {
                    if (!isRevealedTile[x - i, y])
                    {
                        isRevealedTile[x - i, y] = true;
                        addFrontier(new Frontier(new Position2D(x - i, y), map.mapSize, true, false));
                    }
                    else if (isFrontier[x - i, y])
                    {
                        //removeFrontier(x - i, y);
                    }

                }
                if (y - i >= 0)
                {
                    if (!isRevealedTile[x, y - i])
                    {
                        isRevealedTile[x, y - i] = true;
                        addFrontier(new Frontier(new Position2D(x, y - i), map.mapSize, false, true));
                    }
                    else if (isFrontier[x, y - i])
                    {
                        //removeFrontier(x, y - 1);
                    }
                }
            }
            else
            {
                if (x + i < map.mapSize)
                {
                    isRevealedTile[x + i, y] = true;
                    if (isFrontier[x + i, y])
                    {
//removeFrontier(x + i, y);
                    }
                }
                if (y + i < map.mapSize)
                {
                    isRevealedTile[x, y + i] = true;
                    if (isFrontier[x, y + i])
                    {
                        //removeFrontier(x, y + 1);
                    }
                }
                if (x - i >= 0)
                {
                    isRevealedTile[x - i, y] = true;
                    if (isFrontier[x - i, y])
                    {
                        //removeFrontier(x - i, y);
                    }
                }
                if (y - i >= 0)
                {
                    isRevealedTile[x, y - i] = true;
                    if (isFrontier[x, y - i])
                    {
                        //removeFrontier(x, y - 1);
                    }
                }
            }
        }
        if(x + 1 < map.mapSize)
        {
            if(y + 1 < map.mapSize)
            {
                if (!isRevealedTile[x + 1, y + 1])
                {
                    isRevealedTile[x + 1, y + 1] = true;
                    addFrontier(new Frontier(new Position2D(x + 1, y + 1), map.mapSize, true, true));
                }
                else if (isFrontier[x + 1, y + 1])
                {
                    //removeFrontier(x + 1, y + 1);
                }
            }
            if (y - 1 >= 0)
            {
                if (!isRevealedTile[x + 1, y - 1])
                {
                    isRevealedTile[x + 1, y - 1] = true;
                    addFrontier(new Frontier(new Position2D(x + 1, y - 1), map.mapSize, false, true));
                }
                else if (isFrontier[x + 1, y - 1])
                {
                    //removeFrontier(x + 1, y - 1);
                }
            }
        }
        if (x - 1 >= 0)
        {
            if (y + 1 < map.mapSize)
            {
                if (!isRevealedTile[x - 1, y + 1])
                {
                    isRevealedTile[x - 1, y + 1] = true;
                    addFrontier(new Frontier(new Position2D(x - 1, y + 1), map.mapSize, true, false));
                }
                else if (isFrontier[x - 1, y + 1])
                {
                    //removeFrontier(x - 1, y + 1);
                }
            }
            if (y - 1 >= 0)
            {
                if (!isRevealedTile[x - 1, y - 1])
                {
                    isRevealedTile[x - 1, y - 1] = true;
                    addFrontier(new Frontier(new Position2D(x - 1, y - 1), map.mapSize, false, false));
                }
                else if (isFrontier[x - 1, y - 1])
                {
                    //removeFrontier(x - 1, y - 1);
                }
            }
        }

    }

	//GetTileInfo
}
