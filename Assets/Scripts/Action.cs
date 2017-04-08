using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class Action 
{
    Dictionary<State, bool> boolPreConditions;
    Dictionary<State, bool> boolPostConditions;
    Dictionary<Resource, int> preConditions;
    Dictionary<Resource, int> postConditions;

    public int cost; // TODO need a subscribe function herererere

    public Action(Dictionary<Resource, int> preConditions, Dictionary<Resource, int> postConditions)
    {
        this.preConditions = preConditions;
        this.postConditions = postConditions;
        this.boolPreConditions = new Dictionary<State, bool>();
        this.boolPostConditions = new Dictionary<State, bool>();
    }

    public Action(Dictionary<Resource, int> preConditions, Dictionary<Resource, int> postConditions, Dictionary<State, bool> boolPreConditions, Dictionary<State, bool> boolPostConditions)
    {
        this.preConditions = preConditions;
        this.postConditions = postConditions;
        this.boolPreConditions = boolPreConditions;
        this.boolPostConditions = boolPostConditions;
    }


    public abstract void doAction(Minion agent);
}

