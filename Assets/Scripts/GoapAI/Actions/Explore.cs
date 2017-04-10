using System;
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
        minion.goToPos(nextPos);
    }

    public override void doAction(Minion minion)
    {
        //In case of exploration, no further action needed when arriving to destination
    }

    public Position2D findNextFrontier(Minion minion)
    {
        Knowledge.Frontier newFrontier;
        Map map = minion.agentInfo.map;
        List<Knowledge.Frontier> frontiers = minion.agentInfo.frontiers;
        Position2D curPos = minion.getCurPos();

        int minIndex = 0;
        int minDiff = map.mapSize;
        int maxProbIndex = 0;
        float maxProb = 0;
        int temp;

        if (frontiers.Count == 0)
        {

        }
        for (int i = 0; i < frontiers.Count; i++)
        {

            temp = Mathf.Abs(frontiers[i].pos.x - curPos.x) + Mathf.Abs(frontiers[i].pos.y - curPos.y);
            if (temp < minDiff)
            {
                minIndex = i;
                minDiff = temp;
            }
            if (maxProb < frontiers[i].probability)
            {
                maxProb = frontiers[i].probability;
                maxProbIndex = i;
            }
        }

        //  newKnowledge.Frontier = frontiers[minIndex];
        //newKnowledge.Frontier = frontiers[maxProbIndex];
        //Note this is one way of making it random, not sure if it's useful

        if (minIndex == maxProbIndex)
        {
            newFrontier = frontiers[maxProbIndex];
        }
        else if (UnityEngine.Random.Range(0f, 1f) < maxProb)
        {
            newFrontier = frontiers[maxProbIndex];
        }
        else
        {
            newFrontier = frontiers[minIndex];
        }


        return newFrontier.pos;
    }

    public override float getCost(Minion minion)
    {
        Position2D nextPos = findNextFrontier(minion);
        return Mathf.Abs(minion.getCurPos().x - nextPos.x) + Mathf.Abs(minion.getCurPos().y - nextPos.y);
    }
}

