using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//NOTE this class will function as a singleton
public class Base
{
    Inventory baseItems;
    Knowledge baseInfo;

    public Base(Map map)
    {
        baseItems = new Inventory(false);
        baseInfo = new Knowledge(map);

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

