using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BuildBridge : Action
{
    public BuildBridge() 
        : base()
    {
        addPreCond(State.hasBridge, true);
        addPreCond(State.needsBridge, true);
        addPostCond(State.hasBridge, false);
        addPostCond(State.hasPathToSheep, true);
        addPostCond(State.hasPathToIron, true);
    }

    public override void moveToActionLoc(Minion minion)
    {
        Position2D targetPos = getTargetLocation(minion);
        if (targetPos.x == -1 && targetPos.y == -1)
        {
            minion.agentInfo.riverFrontiers.Clear();
            minion.agentInfo.setState(State.needsBridge, false);
        }
        else
        {
            minion.goToPos(targetPos);
        }
    }

    public override void doAction(Minion minion)
    {
        bool success = placeBridgeTile(minion);
        minion.recomputeRiverFrontiers();
        minion.agentInfo.setState(State.needsBridge, minion.needsBridge());
        if (success)
        {
            minion.agentInfo.setState(State.hasBridge, false);
            minion.setBaseState(State.needsBridge, minion.needsBridge());
        }
    }

    public override float getCost(Minion minion)
    {
        Position2D targetPos = getTargetLocation(minion);
        return 1 + Mathf.Abs(minion.getCurPos().x - targetPos.x) + Mathf.Abs(minion.getCurPos().y - targetPos.y);
    }

    private Position2D getTargetLocation(Minion minion)
    {
        Map map = minion.agentInfo.map;
        AStar pathfinder = new AStar(map.mapSize, map.mapSize, map);

        Position2D nextFrontier = findNextRiverFrontier(minion);
        if(nextFrontier.x == -1 && nextFrontier.y == -1)
        {
            return new Position2D(-1, -1);
        }

        Position2D targetPos;
        
        if (map.getTileTypeAt(nextFrontier.x + 1, nextFrontier.y) == TileType.Water)
        {
            targetPos = new Position2D(nextFrontier.x + 2, nextFrontier.y);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) != null)
            {
                return targetPos;
            }
        }
        else if (map.getTileTypeAt(nextFrontier.x - 1, nextFrontier.y) == TileType.Water)
        {
            targetPos = new Position2D(nextFrontier.x - 2, nextFrontier.y);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) != null)
            {
                return targetPos;
            }
        }
        else if (map.getTileTypeAt(nextFrontier.x, nextFrontier.y + 1) == TileType.Water)
        {
            targetPos = new Position2D(nextFrontier.x, nextFrontier.y + 2);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) != null)
            {
                return targetPos;
            }
        }
        else if (map.getTileTypeAt(nextFrontier.x, nextFrontier.y - 1) == TileType.Water)
        {
            targetPos = new Position2D(nextFrontier.x, nextFrontier.y - 2);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) != null)
            {
                return targetPos;
            }
        }

        //This won't ever happen
        return new Position2D(-1,-1);
    }

    private Position2D findNextRiverFrontier(Minion minion)
    {
        Map map = minion.agentInfo.map;
        List<Position2D> riverFrontiers = minion.agentInfo.riverFrontiers;
        Position2D curPos = minion.getCurPos();

        Position2D nextFrontier = new Position2D(-1, -1);

        Dictionary<Position2D, float> frontierValue = new Dictionary<Position2D, float>();
        foreach (Position2D pos in riverFrontiers)
        {
            if (!frontierValue.ContainsKey(pos))
            {
                float score = 0;
                
                List<Position2D> visibleNeighbors = new List<Position2D>();
                visibleNeighbors.Add(new Position2D(pos.x + 1, pos.y));
                visibleNeighbors.Add(new Position2D(pos.x - 1, pos.y));
                visibleNeighbors.Add(new Position2D(pos.x, pos.y + 1));
                visibleNeighbors.Add(new Position2D(pos.x, pos.y - 1));
                visibleNeighbors.Add(new Position2D(pos.x + 2, pos.y));
                visibleNeighbors.Add(new Position2D(pos.x - 2, pos.y));
                visibleNeighbors.Add(new Position2D(pos.x, pos.y + 2));
                visibleNeighbors.Add(new Position2D(pos.x, pos.y - 2));
                visibleNeighbors.Add(new Position2D(pos.x + 1, pos.y + 1));
                visibleNeighbors.Add(new Position2D(pos.x + 1, pos.y - 1));
                visibleNeighbors.Add(new Position2D(pos.x - 1, pos.y + 1));
                visibleNeighbors.Add(new Position2D(pos.x - 1, pos.y - 1));

                foreach (Position2D n in visibleNeighbors)
                {
                    if (n.x >= 0 && n.x < map.mapSize && n.y >= 0 && n.y < map.mapSize)
                    {
                        if (!minion.hasDiscoveredTile(n.x, n.y))
                        {
                            score++;
                        }
                    }
                }

                frontierValue.Add(pos, score);
            }
        }

        //Divide value of frontier by cost required to complete it
        List<Position2D> keys = new List<Position2D>();
        foreach (var a in frontierValue)
            keys.Add(a.Key);

        foreach (Position2D pos in keys)
        {
            float frontiersCost = 1 + Mathf.Abs(minion.getCurPos().x - pos.x) + Mathf.Abs(minion.getCurPos().y - pos.y);
            frontierValue[pos] /= frontiersCost;
        }

        //Normalize scores so that they add up to 1
        float total = 0;
        foreach (Position2D pos in keys)
            total += frontierValue[pos];

        if (total == 0)
        {
            return new Position2D(-1, -1);
        }

        foreach (Position2D pos in keys)
            frontierValue[pos] /= total;

        //Randomly select frontier while favoring spaces with more undiscovered tiles
        //and penalizing frontiers that require moving far away from the minion's current position
        float rand = Random.Range(0f, 1f);

        foreach (var f in frontierValue)
        {
            if (rand < f.Value)
            {
                nextFrontier = f.Key;
                break;
            }
            rand -= f.Value;
        }

        if (nextFrontier == new Position2D(-1, -1))
        {
            throw new System.InvalidOperationException();
        }

        return nextFrontier;
    }

    private bool placeBridgeTile(Minion minion)
    {
        Position2D targetPos;
        Map map = minion.agentInfo.map;
        AStar pathfinder = new AStar(map.mapSize, map.mapSize, map);
        Position2D pos = minion.getCurPos();
        MapTile bridgeTile = new MapTile(Resource.Nothing, TileType.Bridge);

        if (map.getTileTypeAt(pos.x + 1, pos.y) == TileType.Water)
        {
            targetPos = new Position2D(pos.x + 2, pos.y);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), new Position2D(pos.x + 2, pos.y), minion.canClimbMountains()) == null)
            {
                map.setTileAt(pos.x + 1, pos.y, bridgeTile);
                return true;
            }
        }
        else if (map.getTileTypeAt(pos.x - 1, pos.y) == TileType.Water)
        {
            targetPos = new Position2D(pos.x - 2, pos.y);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) == null)
            {
                map.setTileAt(pos.x - 1, pos.y, bridgeTile);
                return true;
            }
        }
        else if (map.getTileTypeAt(pos.x, pos.y + 1) == TileType.Water)
        {
            targetPos = new Position2D(pos.x, pos.y + 2);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) == null)
            {
                map.setTileAt(pos.x, pos.y + 1, bridgeTile);
                return true;
            }
        }
        else if (map.getTileTypeAt(pos.x, pos.y - 1) == TileType.Water)
        {
            targetPos = new Position2D(pos.x, pos.y - 2);
            if (pathfinder.pathFindNewTarget(minion.getCurPos(), targetPos, minion.canClimbMountains()) == null)
            {
                map.setTileAt(pos.x, pos.y - 1, bridgeTile);
                return true;
            }
        }

        return false;
    }
}

