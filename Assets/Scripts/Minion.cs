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
    public bool canCrossMountains = false;

	private Knowledge knowledge;
	private Inventory bag;

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

        Vector3 startPos = gameObject.transform.position;
        for (int i = 0; i < positions.Count; i++)
        {
            float posChangeY = 0, posChangeX = 0;
            posChangeY = positions[i].y * tileSize;
            posChangeX = positions[i].x * tileSize;
            gameObject.transform.position = new Vector3(posChangeX, posChangeY, 0);
            yield return new WaitForSeconds(1f);
        }

    }

    // Use this for initialization
    void Start () {
        AStar astar = new AStar(GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().mapSize, GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().mapSize, GameObject.FindGameObjectWithTag("Map").GetComponent<ManualMap>().map, this);
        List<Position2D> list;
        list = astar.pathFindNewTarget(new Position2D((int)posX, (int)posY), new Position2D(10,10));

        StartCoroutine(goToPos(list));
     
    }
	
	// Update is called once per frame
	void Update () {

    }
}
