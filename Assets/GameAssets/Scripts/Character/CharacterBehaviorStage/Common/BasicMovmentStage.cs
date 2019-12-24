using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BasicMovmentStage : ICharacterBehaviorState
{
    protected NavMeshAgent m_navMeshAgent;
    protected ICyberAgent m_selfAgent;

    protected float m_stepIntervalInSeconds = 0.5f;
    protected float m_timeFromLastStep;
    protected bool m_enableRun;

    #region Initialize

    public BasicMovmentStage(ICyberAgent selfAgent,NavMeshAgent agent)
    {
        this.m_selfAgent = selfAgent;
        this.m_navMeshAgent = agent;
        m_navMeshAgent.updateRotation = false;
        m_stepIntervalInSeconds = Random.Range(0.4f,1f);
    }

    #endregion

    #region Updates

    public virtual void updateStage()
    {
        #region update navmesh agent

        // Move agent to coverPoint.
        if (!m_navMeshAgent.pathPending)
        {
            Vector3 velocity = m_navMeshAgent.desiredVelocity;

            if (!m_enableRun)
            {
                velocity = velocity.normalized;
            }
            else
            {
                velocity = velocity * 2.2f;
            }

            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_selfAgent.moveCharacter(velocity);
        }
        #endregion

        #region Step update Logic
        if (m_timeFromLastStep > m_stepIntervalInSeconds)
        {
            m_timeFromLastStep = 0;
            stepUpdate();
        }
        else
        {
            m_timeFromLastStep += Time.deltaTime;
        }
        #endregion
    }

    protected abstract void stepUpdate();

    #endregion

    #region Commands

    public virtual void stopStageBehavior()
    {
        m_navMeshAgent.isStopped = true;
    }
    #endregion

    #region Getters and Setters

    protected bool checkDestniationReached()
    {
        if(!m_navMeshAgent.pathPending && 
          m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance && 
          (m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            return true;
        }
        return false;
    }

    public abstract void setTargets(ICyberAgent target);

    public virtual void setWeaponFireCapability(bool enabled)
    {
        m_selfAgent.setWeponFireCapability(enabled);
    }

    public virtual void setNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        m_navMeshAgent = navMeshAgent;
    }

    public virtual void setStepIntervalSize(float timeInSeconds)
    {
        m_stepIntervalInSeconds = timeInSeconds;
    }

    public virtual void initalizeStage()
    {
    }

    public virtual void endStage()
    {
        
    }
    #endregion
}
