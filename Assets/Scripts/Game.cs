using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour 
{
	public int mapSize;
	public int minionCount;
	private float tileSize;

	public Transform waterPrefab;
	public Transform forestPrefab;
	public Transform plainsPrefab;
	public Transform mountainPrefab;
	public Transform sandPrefab;
	public Transform boulderPrefab;
	public Transform basePrefab;
	public Transform snowyMountainPrefab;

	private AutoMap mapGenerator;
	private Map map;
	private Base homeBase;
	//private Minion[] minions;

	private Transform[,] tileSprites;

	private const int terrainDepth = 1;
	private const int resourceDepth = 0;
	private const int minionDepth = -1;

	// Use this for initialization
	void Start () 
	{
		mapGenerator = new AutoMap (mapSize);
		map = mapGenerator.CreateMap ();
		//minions = new Minion[minionCount];

		Bounds tileBounds = waterPrefab.GetComponent<Renderer> ().bounds;
		tileSize = tileBounds.max.x - tileBounds.min.x;
		tileSprites = new Transform[mapSize, mapSize];

		//Display Map
		for (int i = 0; i < mapSize; i++) 
		{
			for (int j = 0; j < mapSize; j++) 
			{
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
				Resource r = map.getResource(i,j);
				switch(r){
				case Resource.Wood:
				case Resource.Iron:
				case Resource.TallGrass:
				case Resource.WindBottle:
				case Resource.Wool:
				case Resource.Stone:
					Destroy(tileSprites [i, j].gameObject);
					break;
				default:
					break;
				}
			}
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
