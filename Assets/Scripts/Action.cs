using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class Action 
{
    public Dictionary<State, bool> boolPreConditions;
    public Dictionary<State, bool> boolPostConditions;
    public Dictionary<Resource, int> preConditions;
    public Dictionary<Resource, int> postConditions;

    public int cost; // TODO need a subscribe function herererere


    public Action()
    {
        this.preConditions = new Dictionary<Resource, int>();
        this.postConditions = new Dictionary<Resource, int>();
        this.boolPreConditions = new Dictionary<State, bool>();
        this.boolPostConditions = new Dictionary<State, bool>();
    }

    public void addPreCond(Resource r, int i)
    {
        preConditions.Add(r, i);
    }
    public void addPostCond(Resource r, int i)
    {
        postConditions.Add(r, i);
    }
    public void addPreCond(State s, bool b)
    {
        boolPreConditions.Add(s, b);
    }
    public void addPostCond(State s, bool b)
    {
        boolPostConditions.Add(s, b);
    }

    public void addTool(Resource r)
    {
        addPreCond(r, 1);
        addPostCond(r, 1);
    }


    public Action(Dictionary<Resource, int> preConditions, Dictionary<Resource, int> postConditions, Dictionary<State, bool> boolPreConditions, Dictionary<State, bool> boolPostConditions)
    {
        this.preConditions = preConditions;
        this.postConditions = postConditions;
        this.boolPreConditions = boolPreConditions;
        this.boolPostConditions = boolPostConditions;
    }

    public bool isDoableByMinion(Minion minion)
    {
        foreach(var preCond in preConditions)
        {
            if(preCond.Value > minion.getItemCount(preCond.Key))
            {
                return false;
            }
        }

        foreach (var preCondBool in boolPreConditions)
        {
            if (preCondBool.Value != minion.agentInfo.getStateInfo(preCondBool.Key))
            {
                return false;
            }
        }
        
        return true;
    }

    public abstract void moveToActionLoc(Minion minion);
    public abstract void doAction(Minion minion);
}

