using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	private Dictionary<Resource, int> items;
    private const int limit = 10;
    private int freeSpace = 10;
    private bool isAgent;


    public Inventory(bool isAgent)
    {
        this.isAgent = isAgent;
    }

	public void AddItem(Resource res){
        int quantity = 1;
        if (isSpace() || !isAgent)
        {
            if (items.ContainsKey(res))
            {
                items.TryGetValue(res, out quantity);
                quantity++;
                items.Remove(res);
            }
            items.Add(res, quantity);
            reduceInventorySpace();
        }
	}

    public bool isSpace()
    {
        if(freeSpace > 0)
        {
            return true;
        }

        return false;
    }
    private void reduceInventorySpace()
    {
        freeSpace--;
        if (freeSpace < 0)
        {
            freeSpace = 0;
        }
        
    }

	public bool BuildRecipe(CraftingRecipe r){
		return true;
	}
}
