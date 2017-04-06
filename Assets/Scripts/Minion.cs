using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour {

    private Transform spriteInstance;
    private const float speed = 0.1f;
    private bool isInitialized = false;
    private float posX;
    private float posY;
    private float tileSize;


	public Knowledge knowledge { get; private set; }
	private Inventory bag;
    private AStar astar;
    
    private bool isMoving = false;

	public void initMinion (Transform spriteInstance, int posX, int posY, Knowledge knowledge, float tileSize)
	{
        this.spriteInstance = spriteInstance;
		this.posX = posX;
		this.posY = posY;
		this.bag = new Inventory ();
		this.knowledge = knowledge;
		this.isInitialized = true;
        this.tileSize = tileSize;
        spriteInstance.position = new Vector3(posX * tileSize, posY * tileSize, -1);
	}

    IEnumerator goToPos(List<Position2D> positions)
    {
        isMoving = true;
        Vector3 startPos = gameObject.transform.position;
        for (int i = 0; i < positions.Count; i++)
        {
            float posChangeY = 0, posChangeX = 0;
            posChangeY = positions[i].y * tileSize;
            posChangeX = positions[i].x * tileSize;
            posX = positions[i].x;
            posY = positions[i].y;
            gameObject.transform.position = new Vector3(posChangeX, posChangeY, 0);
            knowledge.discoverTiles((int)posX, (int) posY);

            if(posX == 0 && posY == 0)
            {
                posY = 0;
            }

            yield return new WaitForSeconds(speed);
        }
        isMoving = false;
    }

    // Use this for initialization
    void Start () {
        astar = new AStar(GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().mapSize, GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().mapSize, GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().map, this);

        knowledge.discoverTiles((int)posX, (int) posY);

        
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            List<Position2D> list;
            list = astar.pathFindNewTarget(new Position2D((int)posX, (int)posY), getFrontierDest());
            if (list != null)
            {
                //TODO will just stop pathfinding if this happens, so fix it
                StartCoroutine(goToPos(list));
            }
        }
    }

    Position2D getFrontierDest()
    {
        Position2D dest = new Position2D(0, 0);
        Knowledge.Frontier  front = knowledge.findNextFrontier(new Position2D((int)posX, (int)posY));

        if (front != null) {
            dest = front.pos;
        }

        return dest;
    }
}
