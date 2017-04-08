using System;
using System.Collections.Generic;
using UnityEngine;


public class Explore : Action
{

    public Explore(Dictionary<Resource, int> preConditions, Dictionary<Resource, int> postConditions, Dictionary<State, bool> boolPreConditions, Dictionary<State, bool> boolPostConditions)
        : base(preConditions, postConditions, boolPreConditions, boolPostConditions)
    {

    }

    public override void doAction(Minion agent)
    {

        Position2D nextPos = findNextFrontier(agent);
        agent.goToPos(nextPos);
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
}

