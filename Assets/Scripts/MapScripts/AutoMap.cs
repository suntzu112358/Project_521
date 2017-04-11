using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMap 
{
	private int mapSize;

	private Map map;
	private NoiseGenerator heightMap;
	private NoiseGenerator rainMap;

	private float[,] heightTable;

    public Position2D basePosition { get; private set; }

	//A partition of the map based on accessibility. 
	//We compute this when validating the terrain but it's also useful once we start placing resources so we'll save it here.
	private List<List<Position2D>> riverSeparatedComponents;
	private List<Position2D> mainComponent;

	// Use this for initialization
	public AutoMap (int mapSize) 
	{
		this.mapSize = mapSize;

	}

	public Map CreateMap ()
	{
        bool success = false;
        while (!success)
        {
            generateTerrain();
            while (!ValidateTerrain())
            {
                generateTerrain();
            }

            success = PlaceResources();
        }

        return map;
	}

	private void generateTerrain()
	{

		map = new Map (mapSize);

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

				float height = heightMap.GetNoise (tileX, tileY) - heightDrop;
				float rain = rainMap.GetNoise (tileX, tileY);

				heightTable [i, j] = height;

				//Set tiles on border of map to water tiles
				if (i == 0 || j == 0 || i == mapSize - 1 || j == mapSize - 1) {
					tile = new MapTile (Resource.Nothing, TileType.Water);
				}
				else if (height <= 0) {
					tile = new MapTile (Resource.Nothing, TileType.Water);
				} 
				else if (height < 0.1f) {
					tile = new MapTile (Resource.Nothing, TileType.Sand);
				} 
				else if (height < 0.4f) 
				{
					if (rain < 0.6f)
						tile = new MapTile (Resource.Nothing, TileType.Plains);
					else
						tile = new MapTile (Resource.Nothing, TileType.Forest);
				}
				else {
					if (rain < 0.6f) {
						tile = new MapTile (Resource.Nothing, TileType.Mountain);
					}
					else{
						tile = new MapTile (Resource.Nothing, TileType.SnowyMountain);
						riverStartCandidates.Add (new Position2D (i, j));
					}
				} 

				map.setTileAt (i,j, tile);
			}
		}

		//If more than one island is generated, delete smaller ones that aren't reachable from main island
		deleteExtraIslands();

		//Pick locations for boulders completely randomly 
		int boulderCount = Random.Range(mapSize*mapSize/100, mapSize*mapSize/50);
		for (int i = 0; i < boulderCount; i++) {
			int x = Random.Range (0, mapSize);
			int y = Random.Range (0, mapSize);

			if (map.getTileTypeAt (x, y) == TileType.Plains || map.getTileTypeAt (x, y) == TileType.Sand) 
			{
				map.setTileAt (x, y, new MapTile(Resource.Nothing, TileType.Boulder));
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
                {
                    break;
                }

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
				map.setTileAt(nextRiv.x, nextRiv.y, new MapTile(Resource.Nothing, TileType.Water));
			}
		}
	}

	private void deleteExtraIslands()
	{
		List<List<Position2D>> componentList = new List<List<Position2D>>();
		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				if (map.getTileTypeAt (i, j) != TileType.Water) 
				{
					Position2D newPos = new Position2D (i, j);
					bool alreadyFound = false;
					foreach (List<Position2D> component in componentList)
					{
						if (component.Contains (newPos)) 
						{
							alreadyFound = true;
							break;
						}
					}

					if (!alreadyFound) 
					{
						componentList.Add (getConnectedComponent (newPos, true, true, true));
					}
				}
			}
		}

		if (componentList.Count > 1) 
		{
			List<Position2D> mainIsland = componentList[0];
			for (int i = 1; i < componentList.Count; i++) 
			{
				if (componentList [i].Count > mainIsland.Count)
					mainIsland = componentList [i];
			}

			componentList.Remove (mainIsland);
			foreach (List<Position2D> extraIsland in componentList) 
			{
				foreach (Position2D pos in extraIsland) 
				{
					heightTable [pos.x, pos.y] = 0;
					map.setTileAt(pos.x, pos.y, new MapTile(Resource.Nothing, TileType.Water));
				}
			}
		}
	}

	private bool ValidateTerrain()
	{
		//First simple test, check that there are at least some small number of each tile type
		int mountainCount = 0;
		int forestCount = 0;
		int plainsCount = 0;
		int sandCount = 0;

		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				switch (map.getTileTypeAt (i, j)) 
				{
				case TileType.Mountain:
					mountainCount++;
					break;
				case TileType.Forest:
					forestCount++;
					break;
				case TileType.Plains:
					plainsCount++;
					break;
				case TileType.Sand:
					sandCount++;
					break;
				}
			}
		}

		int totalSize = mapSize * mapSize;
		if (mountainCount < totalSize / 50 || forestCount < totalSize / 50 || plainsCount < totalSize / 50 || sandCount < totalSize / 50)
			return false;


		//Test if rivers block a large enough portion of the map. (We want our minions to build bridges)
		//To do so we'll split the area based on accessibility. We'll pass the test if we get at least 2 large components
		riverSeparatedComponents = new List<List<Position2D>>();
		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				TileType type = map.getTileTypeAt (i, j);
				if (type != TileType.Water && type != TileType.Mountain && type != TileType.Boulder && type != TileType.SnowyMountain) 
				{
					Position2D newPos = new Position2D (i, j);
					bool alreadyFound = false;
					foreach (List<Position2D> component in riverSeparatedComponents)
					{
						if (component.Contains (newPos)) 
						{
							alreadyFound = true;
							break;
						}
					}

					if (!alreadyFound) 
					{
						riverSeparatedComponents.Add (getConnectedComponent (newPos, false, false, false));
					}
				}
			}
		}

		if (riverSeparatedComponents.Count == 0)
			return false;

		int largeComponents = 0;
		int mediumComponents = 0;
		foreach (List<Position2D> component in riverSeparatedComponents) 
		{
			if (component.Count > totalSize / 10)
				largeComponents++;
			else if (component.Count > totalSize / 50)
				mediumComponents++;
		}

		//We need at least two components, one of which must be large.
		if(largeComponents == 0 || (largeComponents + mediumComponents < 2))
			return false;


		//Finally, check that at least one of each tile containing the resources required to build a bridge 
		//is accessible without crossing a river (forest, plains, Boulder)
		mainComponent = riverSeparatedComponents [0];
		for (int i = 1; i < riverSeparatedComponents.Count; i++) {
			if (mainComponent.Count < riverSeparatedComponents [i].Count) 
			{
				mainComponent = riverSeparatedComponents [i];
			}
		}

		bool hasForest = false;
		bool hasBoulder = false;
		bool hasPlains = false;
		foreach (Position2D p in mainComponent) 
		{
			TileType type = map.getTileTypeAt (p.x, p.y);
			if (type == TileType.Forest) {
				hasForest = true;
			}
			else if (type == TileType.Boulder) {
				hasBoulder = true;
			}
			else if (type == TileType.Plains) {
				hasPlains = true;
			}
		}

		if (hasForest && hasPlains && hasBoulder)
			return true;
		else 
			return false;

	}

	//Find and return all positions accessible from a given starting position
	private List<Position2D> getConnectedComponent(Position2D start, bool crossWater, bool crossMountains, bool crossObstacles)
	{
		List<Position2D> connectedComponent = new List<Position2D>();

		bool[,] visited = new bool[mapSize, mapSize];

		List<Position2D> toVisit = new List<Position2D>();
		toVisit.Add (start);
		connectedComponent.Add (start);
		visited [start.x, start.y] = true;

		while (toVisit.Count > 0) 
		{
			Position2D current = toVisit [0];
			toVisit.RemoveAt (0);

			List<Position2D> neighbors = new List<Position2D> ();
			neighbors.Add(new Position2D(current.x, current.y + 1));
			neighbors.Add(new Position2D(current.x, current.y - 1));
			neighbors.Add(new Position2D(current.x + 1, current.y));
			neighbors.Add(new Position2D(current.x - 1, current.y));

			foreach (Position2D n in neighbors) 
			{
				if (n.y < mapSize && n.y >= 0 && n.x < mapSize && n.x >= 0 && !visited[n.x, n.y])
				{
					TileType type = map.getTileTypeAt (n.x, n.y);

					switch (type) 
					{
					case TileType.Forest:
					case TileType.Plains:
					case TileType.Sand:
					case TileType.Bridge:
						toVisit.Add (n);
						connectedComponent.Add (n);
						visited [n.x, n.y] = true;
						break;
					case TileType.Boulder:
					case TileType.SnowyMountain:
						if (crossObstacles) {
							toVisit.Add (n);
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						} else {
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						}
						break;
					case TileType.Mountain:
						if (crossMountains || crossObstacles) {
							toVisit.Add (n);
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						} else {
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						}
						break;
					case TileType.Water:
						if (crossWater && map.getTileTypeAt (current.x, current.y) != TileType.Water) {
							toVisit.Add (n);
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						} else {
							connectedComponent.Add (n);
							visited [n.x, n.y] = true;
						}
						break;
					}
				}
			}
		}

		return connectedComponent;
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

	private bool PlaceResources()
	{
		//Get a list of all possible positions to place each resource type
		List<Position2D> baseSpots = new List<Position2D>();
		List<Position2D> woodSpots = new List<Position2D>();
		List<Position2D> stoneSpots = new List<Position2D>();
		List<Position2D> grassSpots = new List<Position2D>();


		//For some resources (and the base) we'll want at least one copy to be available from the starting area so generate one of each first
		foreach (Position2D p in mainComponent) 
		{
			TileType type = map.getTileTypeAt(p.x, p.y);
			if (type == TileType.Boulder) {
				stoneSpots.Add (p);
			} else if (type == TileType.Forest) {
				woodSpots.Add (p);
			} else if (type == TileType.Plains) {
				grassSpots.Add (p);
			} else if (type == TileType.Sand) {
				baseSpots.Add (p);
			}
		}

		basePosition = baseSpots[Random.Range(0, baseSpots.Count)];
		Position2D stonePosition = stoneSpots[Random.Range(0, stoneSpots.Count)];
		Position2D woodPosition = woodSpots[Random.Range(0, woodSpots.Count)];
		Position2D grassPosition = grassSpots[Random.Range(0, grassSpots.Count)];

		map.setTileAt (basePosition.x, basePosition.y, new MapTile(Resource.Nothing, TileType.Base));
		map.AddResource (stonePosition.x, stonePosition.y, Resource.Stone);
		map.AddResource (woodPosition.x, woodPosition.y, Resource.Wood);
		map.AddResource (grassPosition.x, grassPosition.y, Resource.TallGrass);

		//Now we can add the rest of the resources to random (valid) locations on the island
		woodSpots.Clear();
		stoneSpots.Clear();
		grassSpots.Clear();
		List<Position2D> ironSpots = new List<Position2D>();
		List<Position2D> bottleSpots = new List<Position2D>();
		List<Position2D> sheepSpots = new List<Position2D>();

		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				TileType type = map.getTileTypeAt(i, j);
				if (type == TileType.Boulder) 
				{
					stoneSpots.Add (new Position2D(i,j));
				} 
				else if (type == TileType.Forest) 
				{
					woodSpots.Add (new Position2D(i,j));
				} 
				else if (type == TileType.Plains) 
				{
					grassSpots.Add (new Position2D(i,j));

					//If the patch is not in the same starting area as the base, we can put sheep on it
					if (!mainComponent.Contains (new Position2D (i, j))) {
						sheepSpots.Add (new Position2D (i, j));
					}
				} 
				else if (type == TileType.Mountain) 
				{
					//If tile is at the base of a mountain it can be an iron spot,
					//If it is towards the inside of the mountain it can contain the magic bottle
					bool isAtEdge = map.getTileTypeAt (i + 1, j) != TileType.Mountain
					|| map.getTileTypeAt (i - 1, j) != TileType.Mountain
					|| map.getTileTypeAt (i, j + 1) != TileType.Mountain
					|| map.getTileTypeAt (i, j - 1) != TileType.Mountain;


					if(isAtEdge)
						ironSpots.Add(new Position2D(i,j));
					else
						bottleSpots.Add(new Position2D(i,j));
				}
			}
		}

		//Add Wood
		int woodCount = Random.Range (2, 6);
		for (int i = 0; i < woodCount; i++) {
			Position2D pos = woodSpots [Random.Range (0, woodSpots.Count)];
			if (map.getResource (pos.x, pos.y) == Resource.Nothing)
				map.AddResource (pos.x, pos.y, Resource.Wood);
		}

		//Add Stones
		int stoneCount = Random.Range (2, 6);
		for (int i = 0; i < stoneCount; i++) {
			Position2D pos = stoneSpots [Random.Range (0, stoneSpots.Count)];
			if (map.getResource (pos.x, pos.y) == Resource.Nothing)
				map.AddResource (pos.x, pos.y, Resource.Stone);
		}

		//Add Grass
		int grassCount = Random.Range (2, 6);
		for (int i = 0; i < grassCount; i++) {
			Position2D pos = grassSpots [Random.Range (0, grassSpots.Count)];
			if (map.getResource (pos.x, pos.y) == Resource.Nothing)
				map.AddResource (pos.x, pos.y, Resource.TallGrass);
		}

		//Add Iron
		int ironCount = Random.Range (2, 6);
		for (int i = 0; i < ironCount; i++) {
			Position2D pos = ironSpots [Random.Range (0, ironSpots.Count)];
			if (map.getResource (pos.x, pos.y) == Resource.Nothing)
				map.AddResource (pos.x, pos.y, Resource.Iron);
		}

        //Add Sheep
        if (sheepSpots.Count == 0)
            return false;

		int sheepCount = Random.Range (2, 6);
		for (int i = 0; i < sheepCount; i++) {
			Position2D pos = sheepSpots [Random.Range (0, sheepSpots.Count)];
			if (map.getResource (pos.x, pos.y) == Resource.Nothing)
				map.AddResource (pos.x, pos.y, Resource.Wool);
		}
			
		//Add Wind Bottle
		Position2D bottlePos = bottleSpots [Random.Range (0, bottleSpots.Count)];
		map.AddResource (bottlePos.x, bottlePos.y, Resource.WindBottle);

        return true;
	}

    public List<Position2D> GetMinionPositions(int minionCount)
    {
        List<Position2D> minionList = new List<Position2D>();

        Position2D upPos = new Position2D(basePosition.x, basePosition.y + 1);
        Position2D downPos = new Position2D(basePosition.x, basePosition.y - 1);

        if (map.getTileTypeAt(upPos.x, upPos.y) != TileType.Water)
        {
            minionList.Add( upPos);
        }
        else
            minionList.Add(downPos);

        return minionList;
    }

}
