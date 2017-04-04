using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    private Map map;
    private int gridWidth;
    private int gridHeight;

    Position2D startPosition;
    Position2D targetPosition;

    Minion minion;

    //space-time array being searched for a path
    private AStarNode[,] AStarGrid;

    private class AStarNode
	{

		//Distance from start of path
		private float g;
		//Heuristic distance to end of path
		private float h;
		//Sum of g and h
		private float f;



        //Previous node in shortest path from start to this node
        public Position2D parent { get; set; }

		public float G 
		{
			get { return g;}
			set { g = value; f = h + g;}
		}

		public float H 
		{
			get { return h;}
			set { h = value; f = h + g;}
		}

		public float F { get{ return f; } }

		public AStarNode(){
			Reset();
		}

		public void Reset(){
			G = Mathf.Infinity;
			H = Mathf.Infinity;
		}
	}


    private SortedList<float, Position2D> nextAStar;
    //We want our sortedList to allow duplicate keys so we'll write a custom comparator that never returns equality
    //Note that with such a comparator we can't access elements by key but this is fine for our use since
    //we only ever want to remove the element with the smallest key (at index 0)
    public class DuplicateKeyComparer : IComparer<float>
	{
		public int Compare(float a, float b)
		{
			int result = a.CompareTo (b);

			if (result == 0)
				result = 1;

			return result;
		}
	}


	public AStar(int gridWidth, int gridHeight, Map map, Minion minion)
	{

		this.gridWidth = gridWidth;
		this.gridHeight = gridHeight;
        this.map = map;
        this.minion = minion;

		AStarGrid = new AStarNode[gridWidth, gridHeight];
		for (int i = 0; i < gridWidth; i++)
			for (int j = 0; j < gridHeight; j++)
				AStarGrid [i, j] = new AStarNode ();

		nextAStar = new SortedList<float, Position2D>(new DuplicateKeyComparer());
	}

	//Starts pathfinding towards a new target
	public List<Position2D> pathFindNewTarget(Position2D current, Position2D target)
	{
		this.startPosition = current;
		this.targetPosition = target;

		//Initialize grid for A* search
		nextAStar.Clear();

		for (int i = 0; i < AStarGrid.GetLength (0); i++)
			for (int j = 0; j < AStarGrid.GetLength (1); j++)
				AStarGrid [i, j].Reset ();
		
			
		AStarGrid [startPosition.x, startPosition.y].G = 0;
		AStarGrid [startPosition.x, startPosition.y].H = GetHeuristic (startPosition);

		nextAStar.Add(AStarGrid [startPosition.x, startPosition.y].F, startPosition);

		return AStarSearch ();
	}

	private List<Position2D> AStarSearch()
	{
		bool foundTarget = false;
		while (nextAStar.Count > 0 && !foundTarget) 
		{
			Position2D current = nextAStar.Values[0];
			AStarNode currentNode = AStarGrid [current.x, current.y];
			nextAStar.RemoveAt (0);

			List<Position2D> neighbors = GetNeighbors(current);
			foreach (Position2D n in neighbors)
			{
				if (IsValidMove(n))
				{
					//TODO: for now movecost is just 1
					//If you want to allow diagonal movement cost should be 1 for straight line, Sqrt(2) for diagonals
					//Also, maybe different tiles have different costs, Mountains might have move cost 2 for example (harder to climb a mountain than to walk straight)
					float moveCost = 1;

					AStarNode nextNode = AStarGrid [n.x, n.y];

					if (n == targetPosition) {
						foundTarget = true;
						nextNode.parent = current;
						break;
					}

					if (float.IsInfinity (nextNode.H))
						nextNode.H = GetHeuristic(n);

					if (float.IsInfinity (nextNode.G)) 
					{
						nextNode.G = currentNode.G + moveCost;
						nextNode.parent = current;
						nextAStar.Add (nextNode.F, n);
					}
					else if (nextNode.G > currentNode.G + moveCost) 
					{
						//If the node already has a value, we need to update its children too
						UpdateG (n, current, currentNode.G + moveCost);
					}
				}
			}
		}

		if (!foundTarget) {
			//TODO: In A3, this could not happen. Now however, it will be possible for a given position not to be reachable from your current position
			//You'll need to handle this
			return null;
		}
			
		List<Position2D> path = new List<Position2D> ();
		//Trace path backwards from the end
		Position2D currentPosition = new Position2D (targetPosition.x, targetPosition.y);
		while (currentPosition != new Position2D (startPosition.x, startPosition.y)) 
		{
			path.Insert (0, currentPosition);
			currentPosition = AStarGrid [currentPosition.x, currentPosition.y].parent;
		}

		return path;
	}

	private void UpdateG(Position2D toChange, Position2D newParent, float newValue)
	{
		AStarGrid[toChange.x, toChange.y].G = newValue;
		AStarGrid [toChange.x, toChange.y].parent = new Position2D(newParent.x, newParent.y);

		//Value of G has changed so need to remove from list of nodes to look at
		if (nextAStar.ContainsValue (toChange)) 
		{
			int index = nextAStar.IndexOfValue(toChange);
			nextAStar.RemoveAt (index);
			nextAStar.Add (AStarGrid[toChange.x, toChange.y].F, toChange);
		}

		//Now that we've made changes to the node, propagate changes to its neighbors
		List<Position2D> neighbors = GetNeighbors (toChange);

		foreach (Position2D n in neighbors) 
		{
			//TODO: Same comment as above
			//If you do something complicated, might want to put it in a function rather than write it twice, to avoid confusion
			float moveCost = 1;

			Position2D neighbor = new Position2D (n.x, n.y);
			AStarNode neighborNode = AStarGrid [neighbor.x, neighbor.y];
			if (!float.IsInfinity(neighborNode.G) && neighborNode.G > newValue + moveCost) {
				UpdateG (neighbor, toChange, newValue + moveCost);
			}
		}
			
	}


	//is euclidean 
	public float GetHeuristic(Position2D pos){
        float value = Mathf.Sqrt(Mathf.Pow(targetPosition.x - pos.x, 2) + Mathf.Pow(targetPosition.y - pos.y, 2));
		return value;
	}

    public bool isPassable(TileType type)
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
            if(minion.canCrossMountains)
                return true;
            return false;
        }
        else
        {
            return false;
        }
    }

	//TODO: return list of neighbors of given position, but only if they are not obstructed
	public List<Position2D> GetNeighbors(Position2D pos){
        TileType type = map.getTileTypeAt(pos.x + 1, pos.y);
		List<Position2D> neighbours = new List<Position2D>();
        int x = pos.x, y = pos.y;

        x++;
        type = map.getTileTypeAt(x, y);
        if (isPassable(type))
        {
            neighbours.Add(new Position2D(x, y));
        }

        x -= 2;
        type = map.getTileTypeAt(x, y);
        if (isPassable(type))
        {
            neighbours.Add(new Position2D(x, y));
        }

        x = pos.x;
        y++;
        type = map.getTileTypeAt(x, y);
        if (isPassable(type))
        {
            neighbours.Add(new Position2D(x, y));
        }

        y -= 2;
        type = map.getTileTypeAt(x, y);
        if (isPassable(type))
        {
            neighbours.Add(new Position2D(x, y));
        }



        return neighbours;
	}

	//TODO: return true if position is not blocked
	public bool IsValidMove(Position2D pos){
        if(isPassable(map.getTileTypeAt(pos.x, pos.y)))
		    return true;
        return false;
	}
}
