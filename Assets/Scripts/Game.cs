using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour 
{
	public int mapSize;
	private int minionCount = 1;
    private float timeStep = 0.1f;
	private float tileSize;

	public bool showFullMap = false;

    private bool gameOver = false;

	public Transform waterPrefab;
	public Transform forestPrefab;
	public Transform plainsPrefab;
	public Transform mountainPrefab;
	public Transform sandPrefab;
	public Transform boulderPrefab;
	public Transform basePrefab;
	public Transform snowyMountainPrefab;
	public Transform undiscoveredPrefab;

	public Transform grassPrefab;
	public Transform stonePrefab;
	public Transform ironPrefab;
	public Transform sheepPrefab;
	public Transform woodPrefab;
	public Transform housePrefab;

    public Transform minionPrefab;

	private AutoMap mapGenerator;
	private Map map;
	private Base homeBase;
	private Minion[] minions;

	private Transform[,] tileSprites;
	private Transform[,] undiscoveredSprites;
	private bool[,] isDiscoveredTile;
    private Transform[] minionSprites;


	private const int terrainDepth = 1;
	private const int resourceDepth = 0;
	private const int minionDepth = -1;

    private Explore explore;

    // Use this for initialization
    void Start () 
	{
		mapGenerator = new AutoMap (mapSize);
		map = mapGenerator.CreateMap ();

		minions = new Minion[minionCount];
        minionSprites = new Transform[minionCount];
        List<Position2D> minionPosList = mapGenerator.GetMinionPositions(minionCount);
        for(int i = 0; i < minionCount; i++)
        {
            minions[i] = new Minion(minionPosList[i].x, minionPosList[i].y, map, tileSize);
            minionSprites[i] = Instantiate(minionPrefab);
            minionSprites[i].position = new Vector3(tileSize * minions[i].getCurPos().x, tileSize * minions[i].getCurPos().y, minionDepth);
        }

		Bounds tileBounds = waterPrefab.GetComponent<Renderer> ().bounds;
		tileSize = tileBounds.max.x - tileBounds.min.x;
		tileSprites = new Transform[mapSize, mapSize];
		undiscoveredSprites = new Transform[mapSize, mapSize];
		isDiscoveredTile = new bool[mapSize, mapSize];

		//Display Map
		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				if (!showFullMap) {
					isDiscoveredTile [i, j] = false;
					undiscoveredSprites [i, j] = Instantiate (undiscoveredPrefab);
					undiscoveredSprites [i, j].position = new Vector3 (tileSize * i, tileSize * j, minionDepth);
				}

				//Display Terrain
				TileType type = map.getTileTypeAt (i, j);

				switch(type){
				case TileType.Base:
					tileSprites [i, j] = Instantiate(basePrefab);
					break;
				case TileType.Boulder:
					tileSprites [i, j] = Instantiate (boulderPrefab);
					break;
				case TileType.Forest:
					tileSprites [i, j] = Instantiate (forestPrefab);
					break;
				case TileType.Mountain:
					tileSprites [i, j] = Instantiate (mountainPrefab);
					break;
				case TileType.SnowyMountain:
					tileSprites [i, j] = Instantiate (snowyMountainPrefab);
					break;
				case TileType.Plains:
					tileSprites [i, j] = Instantiate (plainsPrefab);
					break;
				case TileType.Sand:
					tileSprites [i, j] = Instantiate (sandPrefab);
					break;
				case TileType.Water:
					tileSprites [i, j] = Instantiate (waterPrefab);
					break;
				}

				tileSprites [i, j].position = new Vector3 (tileSize * i, tileSize * j, terrainDepth);


				//Display Resource
				Transform resourceInstance;
				Resource r = map.getResource(i,j);
				switch(r){
				case Resource.Wood:
					resourceInstance = Instantiate (woodPrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				case Resource.Iron:
					resourceInstance = Instantiate (ironPrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				case Resource.TallGrass:
					resourceInstance = Instantiate (grassPrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				case Resource.WindBottle:
					resourceInstance = Instantiate (housePrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				case Resource.Wool:
					resourceInstance = Instantiate (sheepPrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				case Resource.Stone:
					resourceInstance = Instantiate (stonePrefab);
					resourceInstance.position = new Vector3 (tileSize * i, tileSize * j, resourceDepth);
					break;
				default:
					break;
				}


			}
		}




		//TODO: this is really bad, change it
        explore = new Explore(null, null, null, null);




        StartCoroutine(GameLoop());
    }
	
	// Update is called once per frame
	void Update () 
	{
		
	}


    IEnumerator GameLoop()
    {
        while (!gameOver)
        {
            for (int i = 0; i < minionCount; i++)
            {
                minions[i].takeStep();
				minionSprites[i].position = new Vector3(tileSize * minions[i].getCurPos().x, tileSize * minions[i].getCurPos().y, minionDepth);
                explore.doAction(minions[i]);
            }

			UpdateMap ();

            yield return new WaitForSeconds(timeStep);
        }
    }

	private void UpdateMap()
	{
		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
				if (!showFullMap) {
					if (!isDiscoveredTile [i, j]) {
						//If the tile hasn't been revealed on the screen, check if a minion has found it
						foreach (Minion m in minions) {
							if (m.hasDiscoveredTile (i, j)) {
								isDiscoveredTile [i, j] = true;
								break;
							}
						}

						//Check if tile was discovered in this round. If so, reveal it on the map
						if (isDiscoveredTile [i, j]) {
							Destroy (undiscoveredSprites [i, j].gameObject);
						}
					}
				}
			}
		}
	}
}
