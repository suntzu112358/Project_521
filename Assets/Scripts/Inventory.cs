using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	private Dictionary<Resource, int> items;
    public int freeSpace { get; private set; }
    private bool isMinionAgent;


    public Inventory(int inventoryCapacity)
    {
        freeSpace = inventoryCapacity;
        items = new Dictionary<Resource, int>();
    }

    public bool addItem(Resource res, int amount)
    {
        if (freeSpace >= amount)
        {
            if (items.ContainsKey(res))
            {
                items[res] += amount;
            }
            else {
               items.Add(res, amount);
            }
            freeSpace -= amount;
            return true;
        }
        else
            return false;
    }

    public bool removeItem(Resource res, int amount)
    {
        if (items.ContainsKey(res))
        {
            if (items[res] >= amount)
            {
                items[res] -= amount;
                freeSpace += amount;
                return true;
            }
        }

        return false;
    }

    //TOSO num of resources
    public bool hasResource(Resource res)
    {
        int quantity;

        if (items.ContainsKey(res))
        {
            items.TryGetValue(res, out quantity);
            if(quantity > 0)
            {
                return true;
            }
        }

        return false;
    }

    public int getItemCount(Resource r)
    {
        if (items.ContainsKey(r))
        {
            return items[r];
        }
        else
            return 0;
    }


    public void depositInventoryToBase(Inventory baseItems)
    {
        List<Resource> keys = new List<Resource>();


        foreach (var item in items)
        {
            keys.Add(item.Key);
        }

        foreach (Resource key in keys)
        {
            baseItems.addItem(key, items[key]);
            bool removeSuccess = this.removeItem(key, items[key]);

            //Shouldn't happen
            if (!removeSuccess)
                throw new System.Exception();
        }
    }
}
