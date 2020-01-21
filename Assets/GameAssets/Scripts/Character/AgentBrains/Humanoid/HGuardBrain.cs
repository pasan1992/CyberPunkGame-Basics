using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HGuardBrain : BasicBrain
{
    protected ICharacterBehaviorState m_currentState;
    protected ICharacterBehaviorState m_combatStage;
    protected ICharacterBehaviorState m_iteractionStage;
    public HGuardBrain(NavMeshAgent agent,HumanoidMovingAgent movingAgent,WaypointRutine waypointRutine):base(agent,movingAgent)
    {
        m_combatStage = new CoverPointBasedCombatStage(movingAgent,agent,GameEnums.MovmentBehaviorType.FREE);
        m_iteractionStage = new IteractionStage(movingAgent,agent,waypointRutine.m_wayPoints.ToArray());
    }
    public override void alertDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAgentDestroy()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAllClear()
    {
        throw new System.NotImplementedException();
    }

    public override void onDamaged()
    {
        throw new System.NotImplementedException();
    }

    public override void onEnemyDetection(ICyberAgent opponent)
    {
        throw new System.NotImplementedException();
    }

    public override void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
