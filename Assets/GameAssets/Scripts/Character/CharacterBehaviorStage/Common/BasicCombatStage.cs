using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicCombatStage : BasicMovmentStage
{
    protected ICyberAgent opponent;
    public BasicCombatStage(ICyberAgent selfAgent,ICyberAgent target,NavMeshAgent navMeshAgent):base(selfAgent,navMeshAgent)
    {
    }
    public override void setTargets(ICyberAgent target)
    {
        if(opponent == null || target != this.opponent)
        {
            this.opponent = target; 
        }
    }

    public override void stopStageBehavior()
    {
        base.stopStageBehavior();
    }

    public override void updateStage()
    {
        base.updateStage();
    }

    protected override void stepUpdate()
    {
    }
}
