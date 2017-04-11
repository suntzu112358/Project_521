using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Wait : Action
{
    public override void doAction(Minion minion)
    {
        minion.agentInfo.setState(State.needsBridge, minion.getBaseState(State.needsBridge));
        minion.agentInfo.setState(State.needsBridge, minion.getBaseState(State.pickAxeAtBase));
        minion.agentInfo.setState(State.needsBridge, minion.getBaseState(State.axeAtBase));
    }

    public override float getCost(Minion minion)
    {
        return Mathf.Infinity;
    }

    public override void moveToActionLoc(Minion minion)
    {
        minion.goToPos(minion.basePosition);
    }
}

