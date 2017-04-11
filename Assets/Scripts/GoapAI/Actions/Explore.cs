using System.Collections.Generic;
using UnityEngine;


public class Explore : Action
{

    public Explore()
        : base()
    {

    }

    public override void moveToActionLoc(Minion minion)
    {
        Position2D nextPos = findNextFrontier(minion);
        if (nextPos == new Position2D(-1, -1))
        {
            minion.agentInfo.recalculateFrontier(minion.canClimbMountains());
        }
        else
        {
            minion.goToPos(nextPos);
        }
    }

    public override void doAction(Minion minion)
    {
        //In case of exploration, no further action needed when arriving to destination
    }

    public Position2D findNextFrontier(Minion minion)
    {
        Map map = minion.agentInfo.map;
        List<Position2D> frontiers = minion.agentInfo.frontiers;
        Position2D curPos = minion.getCurPos();

        Position2D nextFrontier = new Position2D(-1, -1);

        Dictionary<Position2D, float> frontierValue = new Dictionary<Position2D, float>();
        foreach (Position2D pos in frontiers)
        {
            if (!frontierValue.ContainsKey(pos) && map.isPassable(map.getTileTypeAt(pos.x, pos.y), minion.canClimbMountains()))
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

        if(total == 0)
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

        if (nextFrontier == new Position2D(-1,-1))
        {
            new Position2D(-1, -1);
        }
        
        return nextFrontier;
    }

    public override float getCost(Minion minion)
    {
        Position2D nextPos = findNextFrontier(minion);
        return Mathf.Abs(minion.getCurPos().x - nextPos.x) + Mathf.Abs(minion.getCurPos().y - nextPos.y);
    }
}

