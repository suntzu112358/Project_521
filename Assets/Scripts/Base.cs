using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//NOTE this class will function as a singleton
public class Base
{
    public Inventory baseItems { get; private set; }
    public Knowledge baseInfo { get; private set; }

    public Position2D basePosition { get; private set; }

    public Base(Map map, Position2D basePosition)
    {
        baseItems = new Inventory(int.MaxValue);
        baseInfo = new Knowledge(map);

        this.basePosition = basePosition;

        initState();
    }

    public void initState()
    {
        baseInfo.setState(State.hasPathToGrass, false);
        baseInfo.setState(State.hasPathToIron, false);
        baseInfo.setState(State.hasPathToSheep, false);
        baseInfo.setState(State.hasPathToStone, false);
        baseInfo.setState(State.hasPathToWind, false);
        baseInfo.setState(State.hasPathToWood, false);

        baseInfo.setState(State.hasPickAxe, true);
        baseInfo.setState(State.hasAxe, true);

        baseInfo.setState(State.hasSpace, true);

        baseInfo.setState(State.axeAtBase, true);
        baseInfo.setState(State.pickAxeAtBase, true);

    }

}

