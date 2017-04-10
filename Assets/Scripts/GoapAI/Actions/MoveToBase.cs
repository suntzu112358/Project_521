using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MoveToBase : Action
{

    public MoveToBase() : base()
    {
        addPreCond(State.hasSpace, false);
        addPostCond(State.hasSpace, true);
        addPostCond(State.hasAxe, false);
        addPostCond(State.hasPickAxe, false);
    }

    public override void moveToActionLoc(Minion minion)
    {
        minion.goToPos(minion.basePosition);
    }

    public override void doAction(Minion minion)
    {
        minion.baseUpdate(false);
    }

    public override float getCost(Minion minion)
    {
        return 1 + Mathf.Abs(minion.getCurPos().x - minion.basePosition.x) + Mathf.Abs(minion.getCurPos().y - minion.basePosition.y);
    }
}

