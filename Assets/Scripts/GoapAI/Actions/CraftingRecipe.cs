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
        minion.updateInventories();
        minion.shareKnowledgeWithBase();
        foreach(var preCond in preConditions)
        {
            minion.removeItemFromBase(preCond.Key, preCond.Value);
        }
        foreach (var postCond in postConditions)
        {
            minion.addItemToBase(postCond.Key, postCond.Value);
        }
    }

    //TODO create a factory to auto load and dynamically create the objects which should be stored as actions or something
    //I need to sleep and think about this more
    //auto load from json or something, maybe yaml? something simple
}
