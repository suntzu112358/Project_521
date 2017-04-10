using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GetTool : Action
{
    State toolType;
    State toolIsAtBase;

    public GetTool(State toolType, State toolTypeIsAtBase) : base()
    {
        addPreCond(toolTypeIsAtBase, true);
        addPostCond(toolType, true);

        this.toolType = toolType;
        this.toolIsAtBase = toolTypeIsAtBase;
    }

    public override void moveToActionLoc(Minion minion)
    {
        minion.goToPos(minion.basePosition);
    }

    public override void doAction(Minion minion)
    {
        minion.updateInventories();
        minion.shareKnowledgeWithBase();
        minion.tryGetTool(toolType, toolIsAtBase);
    }
}

