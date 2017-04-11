using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ActionFactory
{
    private static Queue<CraftingRecipe> recipeActions;
    private static Queue<GetTool> getToolActions;
    private static Queue<HarvestResource> harvestActions;
    private static Queue<Explore> exploreActions;

    private static MoveToBase goToBaseAction = null;
    private static BuildBridge buildBridgeAction = null;


    private static Action goalAction = null;

    private static Action defaultAction = null;

    public ActionFactory()
    {
        initRecipies();
        initGetTools();
        initHarvesting();
        initGoToBase();
        initExploring();
        initBuildBridge();
        initWait();
        createGoalAction();
    }

    public Action getGoalAction()
    {
        return goalAction;
    }

    public Action getDefaultAction()
    {
        return defaultAction;
    }

    public Action getNextAction(Type actionType)
    {
        Action nextAction = null;

        if(actionType == typeof(CraftingRecipe))
        {
            if (recipeActions.Count != 0)
            {
                nextAction = recipeActions.Dequeue();
            }
        }
        else if (actionType == typeof(GetTool))
        {
            if (getToolActions.Count != 0)
            {
                nextAction = getToolActions.Dequeue();
            }
        }
        else if (actionType == typeof(HarvestResource))
        {
            if (harvestActions.Count != 0)
            {
                nextAction = harvestActions.Dequeue();
            }
        }
        else if (actionType == typeof(MoveToBase))
        {
            //No need to check if null because null is returned if no such object exists in factory at that moment
            nextAction = goToBaseAction;
        }
        else if (actionType == typeof(BuildBridge))
        {
            nextAction = buildBridgeAction;
        }
        else if (actionType == typeof(Explore))
        {
            if (exploreActions.Count != 0)
            {
                nextAction = exploreActions.Dequeue();
            }
        }
        else
        {
            throw new NotSupportedException("The type of class passed isn't a valid Action or is not an action at all");
        }

        return nextAction;
    }

    private void initExploring()
    {
        Explore explore = new Explore();
        Explore mountainExplore = new Explore();

        exploreActions = new Queue<Explore>();
        //TODO a mountain climbing explore

        exploreActions.Enqueue(explore);
        exploreActions.Enqueue(mountainExplore);

        //Post conditions are that it will have a path to a resource
        //in the code we do NOT change the state based on these conditions
        //this is only for the planner to know the benefits of exploring
        explore.addPostCond(State.hasPathToWood, true);
        explore.addPostCond(State.hasPathToGrass, true);
        explore.addPostCond(State.hasPathToStone, true);
        explore.addPostCond(State.needsBridge, true);

        mountainExplore.addPreCond(State.hasMtnKit, true);
        mountainExplore.addPostCond(State.hasPathToWind, true);

    }

    private void initHarvesting()
    {
        HarvestResource getWood = new HarvestResource(Resource.Wood);
        HarvestResource getIron = new HarvestResource(Resource.Iron);
        HarvestResource getStone = new HarvestResource(Resource.Stone);
        HarvestResource getGrass = new HarvestResource(Resource.TallGrass);
        HarvestResource getWool = new HarvestResource(Resource.Wool);
        HarvestResource getWind = new HarvestResource(Resource.WindBottle);

        harvestActions = new Queue<HarvestResource>();

        harvestActions.Enqueue(getWood);
        harvestActions.Enqueue(getIron);
        harvestActions.Enqueue(getStone);
        harvestActions.Enqueue(getGrass);
        harvestActions.Enqueue(getWool);
        harvestActions.Enqueue(getWind);

        //Get wood
        //Pre: has a path to wood, has an axe
        //Post: 1 wood in inventory
        getWood.addPreCond(State.hasPathToWood, true);
        getWood.addPreCond(State.hasAxe, true);
        getWood.addPostCond(Resource.Wood, 1);

        //Get Iron
        //Pre: has a path to iron, has a pickaxe
        //Post: 1 iron in inventory
        getIron.addPreCond(State.hasPathToIron, true);
        getIron.addPreCond(State.hasPickAxe, true);
        getIron.addPostCond(Resource.Iron, 1);

        //Get stone
        //Pre: has a path to stone
        //Post: 1 stone in inventory
        getStone.addPreCond(State.hasPathToStone, true);
        getStone.addPostCond(Resource.Stone, 1);

        //Get grass
        //Pre: has a path to grass
        //Post: 1 grass in inventory
        getGrass.addPreCond(State.hasPathToGrass, true);
        getGrass.addPostCond(Resource.TallGrass, 1);

        //Get wool
        //Pre: has a path to sheep, has shears
        //Post: 1 wool in inventory
        getWool.addPreCond(State.hasPathToSheep, true);
        //getWool.addPreCond(State.hasShears, true);
        getWool.addPostCond(Resource.Wool, 1);

        //Get wind bottle
        //Pre: has a path to wind bottle
        //Post: 1 wind bottle in inventory
        getWind.addPreCond(State.hasPathToWind, true);
        getWind.addPreCond(State.hasMtnKit, true);
        getWind.addPostCond(Resource.WindBottle, 1);

    }

    private void initGetTools()
    {
        //Init of pre/post conds are done in the getTool constructor, nifty no?
        GetTool getAxe = new GetTool(State.hasAxe, State.axeAtBase);
        GetTool getPickAxe = new GetTool(State.hasPickAxe, State.pickAxeAtBase);
        GetTool getShears = new GetTool(State.hasShears, State.shearsAtBase);
        GetTool getMountianKit = new GetTool(State.hasMtnKit, State.mtnKitAtBase);
        GetTool getBridge = new GetTool(State.hasBridge, State.bridgeAtBase);

        getToolActions = new Queue<GetTool>();

        getToolActions.Enqueue(getAxe);
        getToolActions.Enqueue(getPickAxe);
        getToolActions.Enqueue(getMountianKit);
        getToolActions.Enqueue(getBridge);
        //getToolActions.Enqueue(getShears);

        getAxe.addPreCond(State.hasPickAxe, false);
        getPickAxe.addPreCond(State.hasAxe, false);
    }

    private void initRecipies()
    {


        CraftingRecipe makeHammer = new CraftingRecipe();
        CraftingRecipe makeRope = new CraftingRecipe();
        CraftingRecipe makeMtnClimbKit = new CraftingRecipe();
        CraftingRecipe makeFabric = new CraftingRecipe();
        CraftingRecipe makeBridge = new CraftingRecipe();
        CraftingRecipe makeShears = new CraftingRecipe();

        //init queue
        recipeActions = new Queue<CraftingRecipe>();

        recipeActions.Enqueue(makeHammer);
        recipeActions.Enqueue(makeRope);
        recipeActions.Enqueue(makeMtnClimbKit);
        recipeActions.Enqueue(makeFabric);
        recipeActions.Enqueue(makeBridge);
        //recipeActions.Enqueue(makeShears);


        //Hammer
        //pre: wood, stone, rope
        //post: hammer
        makeHammer.addPreCond(Resource.Wood, 1);
        makeHammer.addPreCond(Resource.Stone, 1);
        makeHammer.addPreCond(Resource.Rope, 1);
        makeHammer.addPostCond(Resource.Hammer, 1);


        //Rope
        //pre: tallgrass 3
        //post: rope
        makeRope.addPreCond(Resource.TallGrass, 3);
        makeRope.addPostCond(Resource.Rope, 1);


        //Mountian Climbing Kit
        //Pre: rope 2, iron 1, hammer
        //Post: hammer, mountain climbing kit
        makeMtnClimbKit.addPreCond(Resource.Rope, 2);
        makeMtnClimbKit.addPreCond(Resource.Iron, 1);
        makeMtnClimbKit.addPostCond(State.mtnKitAtBase, true);
        makeMtnClimbKit.addTool(Resource.Hammer);

        //Fabric
        //Pre: wool 2
        //Post: fabric
        makeFabric.addPreCond(Resource.Wool, 2);
        makeFabric.addPostCond(Resource.Fabric, 1);


        //Bridge
        //Pre: 10 wood, 4 rope
        //Post: 1 bridge
        makeBridge.addPreCond(Resource.Wood, 5);
        makeBridge.addPreCond(Resource.Rope, 2);
        makeBridge.addPostCond(State.bridgeAtBase, true);

        //Shears
        //Pre: 1 wood, 1 iron, 1 hammer
        //Post: 1 hammer
        makeShears.addPreCond(Resource.Wood, 1);
        makeShears.addPreCond(Resource.Iron, 1);
        makeShears.addTool(Resource.Hammer);

    }

    private void initGoToBase()
    {
        goToBaseAction = new MoveToBase();
    }

    private void initWait()
    {
        defaultAction = new Wait();
    }

    private void initBuildBridge()
    {
        buildBridgeAction = new BuildBridge();
    }

    private void createGoalAction()
    {
        CraftingRecipe makeShip = new CraftingRecipe();

        //SHIP! Goal Action
        //Pre: 30 wood, 15 iron, 10 rope, 1 wind, 30 fabric, 1 hammer
        //post: 1 hammer, 1 ship
        makeShip.addPreCond(Resource.Wood, 15);
        makeShip.addPreCond(Resource.Iron, 15);
        makeShip.addPreCond(Resource.Rope, 5);
        makeShip.addPreCond(Resource.Fabric, 10);
        makeShip.addPreCond(Resource.WindBottle, 1);
        makeShip.addPostCond(Resource.Ship, 1);
        makeShip.addTool(Resource.Hammer);

        goalAction = makeShip;
    }


}

