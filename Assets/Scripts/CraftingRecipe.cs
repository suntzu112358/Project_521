using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe {

	private Dictionary<Resource, int> inputs;
	private Dictionary<Resource, int> outputs; //output tools as an output as well

    //TODO create a factory to auto load and dynamically create the objects which should be stored as actions or something
    //I need to sleep and think about this more
    //auto load from json or something, maybe yaml? something simple
}
