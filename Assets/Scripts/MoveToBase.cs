using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MoveToBase : Action
{
    public MoveToBase() : base()
    {
        addPreCond(State.hasSpace, false);
        addPostCond(State.hasSpace, true);
        addPostCond(State.hasAxe, false);
        addPostCond(State.hasPickAxe, false);
    }

    public override void doAction(Minion agent)
    {
        throw new NotImplementedException();
    }
}

