using System;
using System.Collections.Generic;
using UnityEngine;


public class Planner
{
    public Action finalGoal { get; private set; }
    private Node actionTree;
    private List<Action> possibleActions;

    public Planner(Position2D baseLocation)
    {
        initActions(baseLocation);
        actionTree = buildGraph(finalGoal);
    }

    private void initActions(Position2D baseLocation)
    {
        possibleActions = new List<Action>();

        Explore explore = new Explore();
        //Explore exploreMountains; //TODO

        MoveToBase moveToBase = new MoveToBase();

        CraftingRecipe makeHammer = new CraftingRecipe();
        CraftingRecipe makeRope = new CraftingRecipe();
        CraftingRecipe makeMtnClimbKit = new CraftingRecipe();
        CraftingRecipe makeFabric = new CraftingRecipe();
        CraftingRecipe makeBridge = new CraftingRecipe();
        CraftingRecipe makeShip = new CraftingRecipe();
        CraftingRecipe makeWood = new CraftingRecipe();

		HarvestResource getWood = new HarvestResource(Resource.Wood);

        GetTool getAxe = new GetTool(State.hasAxe, State.axeAtBase);

        makeHammer.addPreCond(Resource.Wood, 1);
        makeHammer.addPreCond(Resource.Stone, 1);
        makeHammer.addPreCond(Resource.Rope, 1);
        makeHammer.addTool(Resource.Hammer);

        makeRope.addPreCond(Resource.TallGrass, 3);

        makeMtnClimbKit.addPreCond(Resource.Rope, 2);
        makeMtnClimbKit.addPreCond(Resource.Iron, 1);
        makeMtnClimbKit.addTool(Resource.Hammer);

        makeFabric.addPreCond(Resource.Wool, 2);

        makeBridge.addPreCond(Resource.Wood, 10);
        makeBridge.addPreCond(Resource.Rope, 4);

        makeShip.addPreCond(Resource.Wood, 30);
        makeShip.addPreCond(Resource.Iron, 15);
        makeShip.addPreCond(Resource.Rope, 10);
        makeShip.addPreCond(Resource.WindBottle, 1);
        makeShip.addTool(Resource.Hammer);

        makeWood.addPreCond(Resource.Wood, 5);

        getWood.addPreCond(State.hasPathToWood, true);
        getWood.addPreCond(State.hasAxe, true);
        getWood.addPostCond(Resource.Wood, 1);

        //TODO need one for each for this
        explore.addPostCond(State.hasPathToWood, true);

        possibleActions.Add(moveToBase);
        possibleActions.Add(getAxe);
        possibleActions.Add(explore);
        possibleActions.Add(getWood);

        finalGoal = makeWood;
     
    }

    public Action getNextAction(Minion minion)
    {
        Action nextAction = null;

        List<Action> possibleActions = getPossibleActions(minion);

        Dictionary<Action, int> actionCount = new Dictionary<Action, int>();

        foreach(Action a in possibleActions)
        {
            if (actionCount.ContainsKey(a))
            {
                actionCount[a]++;
            }
            else
            {
                actionCount.Add(a, 1);
            }
        }

        int max = int.MinValue;

        foreach(var action in actionCount)
        {
            if(action.Value > max)
            {
                nextAction = action.Key;
                max = action.Value;
            }
        }

        return nextAction;

    }

    private List<Action> getPossibleActions(Minion minion)
    {
        List<Action> actions = new List<Action>();

        //check if action is completeable by current state
        List<Node> toVisit = new List<Node>();
        toVisit.Add(actionTree);

        while (toVisit.Count > 0)
        {
            Node current = toVisit[0];
            toVisit.RemoveAt(0);

            if (current.action.isDoableByMinion(minion))
            {
                actions.Add(current.action);

            }
            else if (!isImpossibleAction(current, minion))
            {
                List<Node> usefulChildActions = new List<Node>();

                foreach (var preCond in current.action.preConditions)
                {
                    if (preCond.Value > minion.getItemCount(preCond.Key))
                    {
                        foreach (Node child in current.children)
                        {
                            if (child.action.postConditions.ContainsKey(preCond.Key))
                            {
                                if (child.action.postConditions[preCond.Key] > 0)
                                {
                                    usefulChildActions.Add(child);
                                }
                            }
                        }
                    }
                }

                foreach (var preCondBool in current.action.boolPreConditions)
                {
                    if (preCondBool.Value != minion.agentInfo.getStateInfo(preCondBool.Key))
                    {
                        foreach (Node child in current.children)
                        {
                            if (child.action.boolPostConditions.ContainsKey(preCondBool.Key))
                            {
                                if (child.action.boolPostConditions[preCondBool.Key] == preCondBool.Value)
                                {
                                    usefulChildActions.Add(child);
                                }
                            }
                        }
                    }
                }

                foreach (Node child in usefulChildActions)
                {
                    toVisit.Add(child);
                }
            }
        }

        return actions;

    }

    private Node buildGraph(Action goalAction)
    {
        Node rootNode = new Node(null, goalAction, 1, Mathf.Infinity);

        buildSubGraph(rootNode);

        return rootNode;
    }

    private void buildSubGraph(Node currentActionNode)
    {
        foreach (Action action in possibleActions)
        {
            int actionValue = getNumOfSatisfiedPreConds(currentActionNode.action, action);
            if (actionValue > 0)
            {
                Node childNode = new Node(currentActionNode, action, 1, actionValue);
                currentActionNode.addChild(childNode);
                buildSubGraph(childNode);
            }
        }
        
    }


    private int getNumOfSatisfiedPreConds(Action goal, Action subGoal)
    {
        Dictionary<Resource, int> subGoalPostRes = subGoal.postConditions;
        Dictionary<State, bool> subGoalPostState = subGoal.boolPostConditions;
        Dictionary<Resource, int> goalPreRes = goal.preConditions;
        Dictionary<State, bool> goalPreState = goal.boolPreConditions;


        int matches = 0;
        int tempVal;
        bool tempBool;

        foreach (var childR in subGoalPostRes)
        {
            if (goalPreRes.ContainsKey(childR.Key))
            {
                goalPreRes.TryGetValue(childR.Key, out tempVal);
                if (childR.Value > 0 && tempVal > 0)
                {
                    matches++;
                }
            }
        }

        foreach (var childS in subGoalPostState)
        {
            if (goalPreState.ContainsKey(childS.Key))
            {
                goalPreState.TryGetValue(childS.Key, out tempBool);
                if ( childS.Value == tempBool)
                {
                    matches++;
                }

            }
  
        }

        return matches;
    }

    private bool isImpossibleAction(Node node, Minion minion)
    {
        
        foreach (var preCond in node.action.preConditions)
        {
            if (preCond.Value > minion.getItemCount(preCond.Key))
            {
                bool satisfiable = false;
                foreach (Node child in node.children)
                {
                    if (child.action.postConditions.ContainsKey(preCond.Key))
                    {
                        if(child.action.postConditions[preCond.Key] > 0)
                        {
                            satisfiable = true;
                            break;
                        }
                    }
                }

                if (!satisfiable)
                    return true;
            }
        }

        foreach (var preCondBool in node.action.boolPreConditions)
        {
            if (preCondBool.Value != minion.agentInfo.getStateInfo(preCondBool.Key))
            {
                bool satisfiable = false;
                foreach (Node child in node.children)
                {
                    if (child.action.boolPostConditions.ContainsKey(preCondBool.Key))
                    {
                        if (child.action.boolPostConditions[preCondBool.Key] == preCondBool.Value)
                        {
                            satisfiable = true;
                            break;
                        }
                    }
                }

                if (!satisfiable)
                    return true;
            }
        }

        return false;
    }

    private class Node
    {
        public Node parent;
        public List<Node> children;
        public float cost;
        public float valueOfAction;
        public Action action;

        public Node(Node parent, Action action, float cost, float valueOfAction)
        {
            this.parent = parent;
            this.cost = cost;
            this.action = action;
            this.valueOfAction = valueOfAction;
            children = new List<Node>();
        }

        public void addChild(Node child)
        {
            children.Add(child);
        }
    }
}

