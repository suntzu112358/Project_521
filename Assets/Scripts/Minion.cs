using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion {

    private const float speed = 0.1f;
    private int posX;
    private int posY;
    private float tileSize;

    private List<Position2D> currentPath;


	public Knowledge agentInfo { get; private set; }
	private Inventory agentBag;
    private AStar astar;
    
    private bool isMoving = false;

	public Minion ( int posX, int posY, Map map, Base homeBase, float tileSize)
	{
		this.posX = posX;
		this.posY = posY;
		this.agentBag = new Inventory (true);
		this.agentInfo = new Knowledge(map);
        this.tileSize = tileSize;
        currentPath = new List<Position2D>();

        astar = new AStar(map.mapSize, map.mapSize, map);
        agentInfo.discoverTiles(this.posX, this.posY);

        initState();
    }

    public void initState()
    {
        agentInfo.setState(State.hasPathToGrass, false);
        agentInfo.setState(State.hasPathToIron, false);
        agentInfo.setState(State.hasPathToSheep, false);
        agentInfo.setState(State.hasPathToStone, false);
        agentInfo.setState(State.hasPathToWind, false);
        agentInfo.setState(State.hasPathToWood, false);

        agentInfo.setState(State.hasPickAxe, false);
        agentInfo.setState(State.hasAxe, false);

        agentInfo.setState(State.hasSpace, true);

        agentInfo.setState(State.axeAtBase, true);
        agentInfo.setState(State.pickAxeAtBase, true);

    }
    
    //IEnumerator walkToPos(List<Position2D> positions)
    //{
    //    isMoving = true;

    //    for (int i = 0; i < positions.Count; i++)
    //    {
    //        float posChangeY = 0, posChangeX = 0;
    //        posChangeY = positions[i].y * tileSize;
    //        posChangeX = positions[i].x * tileSize;
    //        posX = positions[i].x;
    //        posY = positions[i].y;
         
    //        agentInfo.discoverTiles((int)posX, (int) posY);

    //        if(posX == 0 && posY == 0)
    //        {
    //            posY = 0;
    //        }

    //        yield return new WaitForSeconds(speed);
    //    }
    //    isMoving = false;
    //}




	
	// Update is called once per frame
	public void goToPos (Position2D targetPos) {
        if (!isMoving)
        {
            currentPath = astar.pathFindNewTarget(new Position2D(posX, posY), targetPos, canCrossMountains());
            isMoving = true;
        }
    }

    public void takeStep()
    {
        if (isMoving)
        {
            Position2D nextPos = currentPath[0];
            currentPath.RemoveAt(0);
            posX = nextPos.x;
            posY = nextPos.y;

            agentInfo.discoverTiles(posX, posY);

            if (currentPath.Count == 0)
            {
                isMoving = false;
            }
        }
    }

    //TODO Delete
    //Position2D getFrontierDest()
    //{
    //    Position2D dest = new Position2D(0, 0);
    //    Knowledge.Frontier  front = agentInfo.findNextFrontier(new Position2D(posX, posY));

    //    if (front != null) {
    //        dest = front.pos;
    //    }

    //    return dest;
    //}

    public Position2D getCurPos()
    {
        return new Position2D(posX, posY);
    }

    public bool canCrossMountains()
    {
        return agentBag.hasResource(Resource.MontainKit);
    }


	public bool hasDiscoveredTile(int x, int y)
	{
		return agentInfo.isRevealedTile [x, y];
	}

    public int getItemCount(Resource r)
    {
        return agentBag.getItemCount(r);
    }
}
