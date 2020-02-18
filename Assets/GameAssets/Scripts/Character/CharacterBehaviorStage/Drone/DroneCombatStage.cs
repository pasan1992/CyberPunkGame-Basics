using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneCombatStage : BasicMovmentCombatStage
{
    ICyberAgent m_opponent;
    public enum DRONE_COMBAT_STAGES { Moving,Fire,DecidingToMove,NONE}
    private DRONE_COMBAT_STAGES m_currentCombatStage = DRONE_COMBAT_STAGES.DecidingToMove;
    private DRONE_COMBAT_STAGES m_loggingState = DRONE_COMBAT_STAGES.NONE;
    private Vector3 m_movePoint;
    private Vector3 m_randomTargetOffset;

    private float m_timeInFirePosition;

    #region Initialize

    public DroneCombatStage(ICyberAgent selfAgent, NavMeshAgent navMeshAgent, ICyberAgent opponent) : base(selfAgent, navMeshAgent)
    {
        m_opponent = opponent;
        setStepIntervalSize(0.5f);
        navMeshAgent.updateRotation = false;
        m_enableRun = true;
        m_currentMovmentType = GameEnums.MovmentBehaviorType.NEAR_POINT;
    }

    #endregion

    #region Update

    public override void updateStage()
    {
        // This is needed for calling step update.
        base.updateStage();

        m_selfAgent.setTargetPoint(m_opponent.getTopPosition()+ m_randomTargetOffset);
    }

    protected override void updateNearPointPositonMovment()
    {
        logState();
        findTargetLocationToFire();

        switch (m_currentCombatStage)
        {
            case DRONE_COMBAT_STAGES.DecidingToMove:
                calculateMovePoint();
                m_currentCombatStage = DRONE_COMBAT_STAGES.Moving;
                break;
            case DRONE_COMBAT_STAGES.Fire:
     
                m_timeInFirePosition += Time.deltaTime;

                if(m_timeInFirePosition >0.1f)
                {
                    m_timeInFirePosition = 0;
                    m_currentCombatStage = DRONE_COMBAT_STAGES.DecidingToMove;
                }

                if (Vector3.Distance( m_selfAgent.getCurrentPosition(), m_opponent.getCurrentPosition() ) > 10)
                {
                    m_currentCombatStage = DRONE_COMBAT_STAGES.DecidingToMove;
                }
                m_selfAgent.weaponFireForAI();

                break;
            case DRONE_COMBAT_STAGES.Moving:

                if(m_navMeshAgent.remainingDistance < 8)
                {
                    m_selfAgent.weaponFireForAI();
                }



                if (m_navMeshAgent.remainingDistance < 5 + Random.value*2)
                {
                    //m_navMeshAgent.isStopped = true;
                    m_currentCombatStage = DRONE_COMBAT_STAGES.Fire;
                }

                if (Vector3.Distance(m_selfAgent.getCurrentPosition(), m_opponent.getCurrentPosition()) > 10)
                {
                    m_currentCombatStage = DRONE_COMBAT_STAGES.DecidingToMove;
                }

                break;
        }
    }

    private void logState()
    {
        if(!m_loggingState.Equals(m_currentCombatStage))
        {
            m_loggingState = m_currentCombatStage;
        }
    }

    #endregion

    #region Getters and Setters

    public override void setTargets(ICyberAgent target)
    {
        m_opponent = target;
    }

    #endregion

    #region Events
    #endregion

    #region Utility
    public void calculateMovePoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle*(Random.value*5);
        m_movePoint = m_opponent.getCurrentPosition() + new Vector3(randomPoint.x, 0, randomPoint.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(m_movePoint, out hit, 10, 1);

        Vector3 finalPosition = m_movePoint;
        m_navMeshAgent.SetDestination(finalPosition);
        m_navMeshAgent.isStopped = false;
    }

    public void findTargetLocationToFire()
    {

        if (Random.value > m_selfAgent.getSkill())
        {
            m_randomTargetOffset = Random.insideUnitSphere * 1;
        }
        else
        {
            m_randomTargetOffset = Vector3.zero;
        }

        if (!m_currentCombatStage.Equals(DRONE_COMBAT_STAGES.Fire))
        {
            m_randomTargetOffset += Random.insideUnitSphere * 1;
        }
    }
    #endregion

}
