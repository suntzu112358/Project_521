using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe : Action {

    public CraftingRecipe()
        : base()
    {
    }

    public override void moveToActionLoc(Minion minion)
    {
        minion.goToPos(minion.basePosition);
    }

    public override void doAction(Minion minion)
    {
        minion.baseUpdate(false);

        //It might be that some resources have been used since the last time the minion was at the base so it might
        //not be able to make the item anymore even though it thought it could
        if (!isDoableByMinion(minion))
            return;

        foreach(var preCond in preConditions)
        {
            minion.removeItemFromBase(preCond.Key, preCond.Value);
        }
        foreach (var postCond in postConditions)
        {
            minion.addItemToBase(postCond.Key, postCond.Value);
        }
        foreach (var boolPostCond in boolPostConditions)
        {
            minion.agentInfo.setState(boolPostCond.Key, boolPostCond.Value);
            minion.setBaseState(boolPostCond.Key, boolPostCond.Value);
        }
    }

    public override float getCost(Minion minion)
    {
        return 1 + Mathf.Abs(minion.getCurPos().x - minion.basePosition.x) + Mathf.Abs(minion.getCurPos().y - minion.basePosition.y);
    }
}
