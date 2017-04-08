//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ManualMap : MonoBehaviour 
//{
//	public Transform water;
//	public Transform forest;
//	public Transform plains;
//    public Transform minion;
//    public Transform emptyTile;
//    private Knowledge k;

//	public int mapSize = 7;

//    public Map map { get; private set; }

    

//	// Use this for initialization
//	void Start () 
//	{
//		//Assume all tiles are same sized squares
//		Bounds tileBounds = water.GetComponent<Renderer> ().bounds;
//		map = new Map (mapSize);

//		for (int i = 0; i < mapSize; i++) 
//		{
//			for (int j = 0; j < mapSize; j++) 
//			{
//				MapTile plainTile = new MapTile (Resource.Nothing, TileType.Plains);
//				map.setTileAt (i,j, plainTile);
//			}
//		}

//		for (int j = 0; j < mapSize; j++) 
//		{
//			MapTile waterTile = new MapTile (Resource.Nothing, TileType.Water);
//            if (j != mapSize/2)
//            {
//                map.setTileAt(mapSize/2, j, waterTile);
//            }
//		}

//        List<Minion> minions = new List<Minion>();
//        Transform newMinion = Instantiate(minion);
//        Minion minionScript = newMinion.GetComponent<Minion>();
//        minionScript.initMinion(newMinion, 1, 1, new Knowledge(map), tileBounds.max.x - tileBounds.min.x);
//        minions.Add(minionScript);

//		k = minionScript.agentInfo;


//	}
	
//	// Update is called once per frame
//	void FixedUpdate () {
//        for (int i = 0; i < mapSize; i++)
//        {
//            for (int j = 0; j < mapSize; j++)
//            {
//                if (k.isRevealedTile[i, j])
//                {
//                    MapTile newTile;
//                    if (i == mapSize / 2 && j != mapSize/2)
//                    {
//                        newTile = new MapTile(Resource.Nothing, TileType.Water);
//                    }
//                    else
//                    {
//                        newTile = new MapTile(Resource.Nothing, TileType.Plains);
                      
//                    }

//                    map.setTileAt(i, j, newTile);
//                }
//            }
//        }
//    }
//}
