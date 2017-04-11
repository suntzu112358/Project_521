using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion {

    private const float speed = 0.1f;
    private int posX;
    private int posY;

    private List<Position2D> currentPath;

	public Knowledge agentInfo { get; private set; }
	private Inventory agentBag;
    private AStar astar;

    private Base homeBase;

    public Position2D basePosition { get; private set; }
    
    public bool isMoving { get; private set; }

	public Minion ( int posX, int posY, Map map, Base homeBase, float tileSize)
	{
        isMoving = false;
		this.posX = posX;
		this.posY = posY;
		this.agentBag = new Inventory (10);
        //TODO: pass baseKnowledge and inventory
		this.agentInfo = new Knowledge(map, homeBase);
        currentPath = new List<Position2D>();

        astar = new AStar(map.mapSize, map.mapSize, map);
        agentInfo.discoverTiles(this.posX, this.posY, canClimbMountains());

        this.homeBase = homeBase;
        basePosition = homeBase.basePosition;

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
        agentInfo.setState(State.hasMtnKit, false);
        agentInfo.setState(State.hasBridge, false);
        agentInfo.setState(State.hasShears, false);

        agentInfo.setState(State.hasSpace, true);

        agentInfo.setState(State.axeAtBase, true);
        agentInfo.setState(State.pickAxeAtBase, true);
        agentInfo.setState(State.shearsAtBase, false);
        agentInfo.setState(State.mtnKitAtBase, false);
        agentInfo.setState(State.bridgeAtBase, false);

        agentInfo.setState(State.needsBridge, false);


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
	public void goToPos (Position2D targetPos)
    {
        if (!isMoving)
        {
            currentPath = astar.pathFindNewTarget(new Position2D(posX, posY), targetPos, canClimbMountains());
            if (currentPath == null)
            {
                throw new System.ArgumentNullException();
            }
            isMoving = true;
        }
    }

    /**
     * @return returns false if minion has no more steps to take
     */
    public bool takeStep()
    {
        if (currentPath.Count == 0)
        {
            isMoving = false;
            return false;
        }

        if (isMoving)
        {
            Position2D nextPos = currentPath[0];
            currentPath.RemoveAt(0);
            posX = nextPos.x;
            posY = nextPos.y;

            agentInfo.discoverTiles(posX, posY, canClimbMountains());
        }

        return true;
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


	public bool hasDiscoveredTile(int x, int y)
	{
		return agentInfo.isRevealedTile [x, y];
	}

    public int getItemCount(Resource r)
    {
        return agentBag.getItemCount(r);
    }


    public void goToClosestResource(Resource resType)
    {
        if (!isMoving)
        {
            currentPath = agentInfo.getPathToClosestResource(resType, getCurPos(), canClimbMountains());
            if (currentPath == null)
            {
                throw new System.ArgumentNullException();
            }
            isMoving = true;
        }
    }

    public void baseUpdate(bool keepTools)
    {
        agentBag.depositInventoryToBase(homeBase.baseItems);
        agentInfo.syncRevealedTiles(homeBase.baseInfo);
        agentInfo.recalculateFrontier(canClimbMountains());
        agentInfo.syncStates(homeBase, keepTools);
    }

    public void harvestResource(Resource res)
    {
        agentBag.addItem(res, 1);
        if(agentBag.freeSpace == 0)
        {
            agentInfo.setState(State.hasSpace, false);
        }
    }

    public void tryGetTool(State hasTool, State isToolAtBase)
    {
        if (homeBase.baseInfo.getStateInfo(isToolAtBase))
        {
            agentInfo.setState(hasTool, true);
            agentInfo.setState(isToolAtBase, false);
            homeBase.baseInfo.setState(hasTool, false);
            homeBase.baseInfo.setState(isToolAtBase, false);
        }
    }

    public void addItemToBase(Resource r, int amount)
    {
        homeBase.baseItems.addItem(r, amount);
    }

    public void removeItemFromBase(Resource r, int amount)
    {
        homeBase.baseItems.removeItem(r, amount);
    }

    public float getClosestResourceDistance(Resource r)
    {
        return agentInfo.getClosestResourceDistance(r, getCurPos());
    }

    public bool canClimbMountains()
    {
        return agentInfo.getStateInfo(State.hasMtnKit);
    }

    public void setBaseState(State s, bool b)
    {
        homeBase.baseInfo.setState(s, b);
    }

    public bool getBaseState(State s)
    {
        return homeBase.baseInfo.getStateInfo(s);
    }

    public void recomputeRiverFrontiers()
    {
        agentInfo.recomputeRiverFrontiers();
    }

    public bool needsBridge()
    {
        return agentInfo.riverFrontiers.Count > 0;
    }
}
