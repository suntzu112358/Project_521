using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe : Action {

    protected CraftingRecipe(Dictionary<Resource, int> preConditions, Dictionary<Resource, int>  postConditions)
        : base(preConditions, postConditions)
    {
        
    }


    public override void doAction(Minion agent)
    {
        throw new NotImplementedException();
    }

    //TODO create a factory to auto load and dynamically create the objects which should be stored as actions or something
    //I need to sleep and think about this more
    //auto load from json or something, maybe yaml? something simple
}
