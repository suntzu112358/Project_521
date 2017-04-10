using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge {
	//TODO have list of resource points and base location
	public Map map { get; private set; }
	public bool[,] isRevealedTile;

    Dictionary<State, bool> curAgentState;

    public List<Frontier> frontiers { get; private set; }
    public List<Frontier> riverFrontiers { get; private set; }
    public bool canCrossMountains = false; //use a reference NOT a primitive type TODO or rather a function calls

    private Inventory lastBaseInventory;

    private AStar pathFinder;
    

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

	public Knowledge(Map map, Base homeBase)
	{
		this.map = map;
		isRevealedTile = new bool[map.mapSize, map.mapSize];

        frontiers = new List<Frontier>();
        riverFrontiers = new List<Frontier>();

        for (int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                isRevealedTile[i, j] = false;
            }
        }


        this.curAgentState = new Dictionary<State, bool>();
        pathFinder = new AStar(map.mapSize, map.mapSize, map);
        lastBaseInventory = homeBase.copyBaseInventory();

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

    public void addFrontier(Frontier f, bool blockedByRiver)
    {
		if (isFrontier (f.pos.x, f.pos.y)) {
			if (f.probability > 0 && map.isPassable (map.getTileTypeAt (f.pos.x, f.pos.y), canCrossMountains)) {
                if (!blockedByRiver)
                    frontiers.Add(f);
                else
                    riverFrontiers.Add(f);
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

                    bool blockedByRiver = false;
                    //Check if tile is accessible or blocked by river
                    if (Mathf.Abs(p.x - x) > 1 || Mathf.Abs(p.y - y) > 1)
                    { 
                        int midTileX = (p.x + x) / 2;
                        int midTileY = (p.y + y) / 2;

                        if(map.getTileTypeAt(midTileX, midTileY) == TileType.Water)
                        {
                            //Might be blocked by a river, do A* to see if it is accessible
                            List<Position2D> pathAttempt = pathFinder.pathFindNewTarget(new Position2D(x, y), new Position2D(p.x, p.y), canCrossMountains);
                            if (pathAttempt == null)
                                blockedByRiver = true;
                        }
                    }

                    if (!blockedByRiver)
                    {
                        if(map.getResource(p.x, p.y) != Resource.Nothing)
                        {
                            switch(map.getResource(p.x, p.y))
                            {
                                case Resource.Wood:
                                    setState(State.hasPathToWood, true);
                                    break;
                                case Resource.Iron:
                                    setState(State.hasPathToIron, true);
                                    break;
                                case Resource.TallGrass:
                                    setState(State.hasPathToGrass, true);
                                    break;
                                case Resource.WindBottle:
                                    setState(State.hasPathToWind, true);
                                    break;
                                case Resource.Wool:
                                    setState(State.hasPathToSheep, true);
                                    break;
                                case Resource.Stone:
                                    setState(State.hasPathToStone, true);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    bool north = p.y > y;
					bool east = p.x > x;
					addFrontier(new Frontier(new Position2D(p.x, p.y), map.mapSize, north, east), blockedByRiver);
				}
				else if (!isFrontier(p.x, p.y))
				{
					removeFrontier(p.x, p.y);
				}
			}	
		}
    }

    public bool getStateInfo(State state)
    {
        bool val = false ;

        if (curAgentState.ContainsKey(state))
        {
            curAgentState.TryGetValue(state, out val);
        }

        return val;
    }

    public void setState(State state, bool val)
    {
        if (curAgentState.ContainsKey(state))
        {
            curAgentState[state] = val;
        }
        else
        {
            curAgentState.Add(state, val);
        }
    }

    public float getClosestResourceDistance(Resource r, Position2D curPos)
    {
        List<Position2D> resourcePositions =  map.getResourcePositions(r);
        float min = Mathf.Infinity;
        foreach(Position2D p in resourcePositions)
        {
            if (min > Mathf.Abs(p.x - curPos.x) + Mathf.Abs(p.y - curPos.y))
            {
                min = Mathf.Abs(p.x - curPos.x) + Mathf.Abs(p.y - curPos.y);
            }
        }

        return min;
    }

    public List<Position2D> getPathToClosestResource(Resource resType, Position2D curPos)
    {
        List<Position2D> resPositions = new List<Position2D>();
        for(int i=0; i<map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                if(map.getResource(i,j) == resType && isRevealedTile[i,j])
                {
                    if (map.isPassable(map.getTileTypeAt(i, j), canCrossMountains))
                    {
                        resPositions.Add(new Position2D(i, j));
                    }
                    else if (map.isPassable(map.getTileTypeAt(i+1, j), canCrossMountains))
                    {
                        resPositions.Add(new Position2D(i+1, j));
                    }
                    else if (map.isPassable(map.getTileTypeAt(i, j+1), canCrossMountains))
                    {
                        resPositions.Add(new Position2D(i, j+1));
                    }
                    else if (map.isPassable(map.getTileTypeAt(i-1, j), canCrossMountains))
                    {
                        resPositions.Add(new Position2D(i-1, j));
                    }
                    else if (map.isPassable(map.getTileTypeAt(i, j-1), canCrossMountains))
                    {
                        resPositions.Add(new Position2D(i, j-1));
                    }
                }
            }
        }

        int minLength = int.MaxValue;
        List<Position2D> shortestPath = new List<Position2D>();

        foreach(Position2D pos in resPositions)
        {
            List<Position2D> pathToRes = pathFinder.pathFindNewTarget(curPos, pos, canCrossMountains);
            if(pathToRes != null && pathToRes.Count < minLength)
            {
                shortestPath = pathToRes;
                minLength = pathToRes.Count;
            }
        }

        return shortestPath;
    }

    public void syncRevealedTiles(Knowledge k2)
    {
        for(int i = 0; i < map.mapSize; i++)
        {
            for(int j = 0; j < map.mapSize; j++)
            {
                if(this.isRevealedTile[i,j] || k2.isRevealedTile[i, j])
                {
                    this.isRevealedTile[i, j] = true;
                    k2.isRevealedTile[i, j] = true;
                }
            }
        }
    }

    public void syncStates(Base homeBase, bool keepTools)
    {
        Knowledge baseKnowledge = homeBase.baseInfo;

        if (!keepTools)
        {
            //Drop off tools
            if (this.getStateInfo(State.hasAxe))
            {
                baseKnowledge.setState(State.hasAxe, true);
                baseKnowledge.setState(State.axeAtBase, true);
            }
            if (this.getStateInfo(State.hasPickAxe))
            {
                baseKnowledge.setState(State.hasPickAxe, true);
                baseKnowledge.setState(State.pickAxeAtBase, true);
            }

            this.setState(State.hasAxe, false);
            this.setState(State.hasPickAxe, false);
        }

        this.setState(State.axeAtBase, baseKnowledge.getStateInfo(State.axeAtBase));
        this.setState(State.pickAxeAtBase, baseKnowledge.getStateInfo(State.pickAxeAtBase));

        //Update if new resources have been found
        if(this.getStateInfo(State.hasPathToGrass) || baseKnowledge.getStateInfo(State.hasPathToGrass))
        {
            this.setState(State.hasPathToGrass,true);
            baseKnowledge.setState(State.hasPathToGrass, true);
        }
        if (this.getStateInfo(State.hasPathToIron) || baseKnowledge.getStateInfo(State.hasPathToIron))
        {
            this.setState(State.hasPathToIron, true);
            baseKnowledge.setState(State.hasPathToIron, true);
        }
        if (this.getStateInfo(State.hasPathToSheep) || baseKnowledge.getStateInfo(State.hasPathToSheep))
        {
            this.setState(State.hasPathToSheep, true);
            baseKnowledge.setState(State.hasPathToSheep, true);
        }
        if (this.getStateInfo(State.hasPathToStone) || baseKnowledge.getStateInfo(State.hasPathToStone))
        {
            this.setState(State.hasPathToStone, true);
            baseKnowledge.setState(State.hasPathToStone, true);
        }
        if (this.getStateInfo(State.hasPathToWind) || baseKnowledge.getStateInfo(State.hasPathToWind))
        {
            this.setState(State.hasPathToWind, true);
            baseKnowledge.setState(State.hasPathToWind, true);
        }
        if (this.getStateInfo(State.hasPathToWood) || baseKnowledge.getStateInfo(State.hasPathToWood))
        {
            this.setState(State.hasPathToWood, true);
            baseKnowledge.setState(State.hasPathToWood, true);
        }

        this.setState(State.hasSpace, true);

        lastBaseInventory = homeBase.copyBaseInventory();
    }

    public int getItemsAtBase(Resource r)
    {
        return lastBaseInventory.getItemCount(r);
    }
}
