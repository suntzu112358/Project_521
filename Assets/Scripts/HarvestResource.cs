using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HarvestResource : Action
{
    Resource resourceType;

    public HarvestResource(Resource res) : base() {
        addPreCond(State.hasSpace, true);
        resourceType = res;
    }

    public override void moveToActionLoc(Minion minion)
    {
        minion.goToClosestResource(resourceType);
    }

    public override void doAction(Minion minion)
    {
        minion.harvestResource(resourceType);
    }
}

