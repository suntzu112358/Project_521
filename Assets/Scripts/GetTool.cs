using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GetTool : Action
{
    public GetTool(State toolType, State toolTypeIsAtBase) : base()
    {
        addPreCond(toolTypeIsAtBase, true);
        addPostCond(toolType, true);
    }

    public override void doAction(Minion agent)
    {
        //TODO move to base
        throw new NotImplementedException();
    }
}

