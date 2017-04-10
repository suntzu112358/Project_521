using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


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
        minion.baseUpdate(true);
        minion.tryGetTool(toolType, toolIsAtBase);
    }

    public override float getCost(Minion minion)
    {
        return 1 + Mathf.Abs(minion.getCurPos().x - minion.basePosition.x) + Mathf.Abs(minion.getCurPos().y - minion.basePosition.y);
    }
}

