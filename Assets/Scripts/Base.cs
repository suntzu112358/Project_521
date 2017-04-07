using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//NOTE this class will function as a singleton
class Base
{
    Inventory worldState;
    Knowledge worldInfo;

    Base instance = null;

    private Base(Map map)
    {
        worldState = new Inventory(false);
        worldInfo = new Knowledge(map);

    }

    public Base getInstance(Map map)
    {
        if(instance == null)
        {
            return new Base(map);
        }
        return instance;
    }
}

