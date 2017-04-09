using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HarvestResource : Action
{
    
    public HarvestResource() : base() {
        addPreCond(State.hasSpace, true);
    }

    public override void doAction(Minion agent)
    {
        throw new NotImplementedException();
    }
}

