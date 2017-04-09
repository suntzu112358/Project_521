using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	private Dictionary<Resource, int> items;
    private const int limit = 10;
    private int freeSpace = 10;
    private bool isMinionAgent;


    public Inventory(bool isMinionAgent)
    {
        this.isMinionAgent = isMinionAgent;
        items = new Dictionary<Resource, int>();
    }

	public void addItem(Resource res){
        int quantity = 1;
        int temp;

            
        if (isSpace() || !isMinionAgent)
        {
            if (items.ContainsKey(res))
            {
                items.TryGetValue(res, out quantity);
                temp = (int)quantity;
                temp++;
                quantity = temp;
                items.Remove(res);
            }
            items.Add(res, quantity);
            reduceInventorySpace();
        }
       
	}

    public Resource removeItem(Resource res)
    {
        int quantity = 1;
        int temp = 0;
        Resource rtnVal = Resource.Nothing;

        if (isSpace() || !isMinionAgent)
        {
            if (items.ContainsKey(res))
            {
                items.TryGetValue(res, out quantity);
                temp = (int)quantity;
                temp--;
                quantity = temp;
                items.Remove(res);
                rtnVal = res;
            }
            if (temp == 0)
            {
                items.Add(res, quantity);
                increaseInventorySpace();
            }
        }
        

        return rtnVal;
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

    public bool isSpace()
    {
        if(freeSpace > 0)
        {
            return true;
        }

        return false;
    }

    public bool isEmpty()
    {
        if(freeSpace == limit)
        {
            return true;
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

    private void reduceInventorySpace()
    {
        freeSpace--;
        if (freeSpace < 0)
        {
            freeSpace = 0;
        }
        
    }

    private void increaseInventorySpace()
    {
        freeSpace++;
        if (freeSpace > limit)
        {
            freeSpace = limit;
        }

    }
}
