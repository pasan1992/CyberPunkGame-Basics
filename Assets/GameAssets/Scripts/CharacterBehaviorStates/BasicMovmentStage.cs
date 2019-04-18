﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BasicMovmentStage : ICharacterBehaviorState
{
    protected NavMeshAgent agent;
    protected ICyberAgent selfAgent;

    protected float stepIntervalInSeconds = 0.5f;
    protected float timeFromLastStep;
    protected bool enableRun;

    public abstract void setNavMeshAgent(NavMeshAgent navMeshAgent);
    public abstract void setStepIntervalSize(float timeInSeconds);
    public abstract void setTargets(ICyberAgent target);

    public BasicMovmentStage(ICyberAgent selfAgent,NavMeshAgent agent)
    {
        this.selfAgent = selfAgent;
        this.agent = agent;
        agent.updateRotation = false;
    }

    public virtual void updateStage()
    {
        #region update navmesh agent
        // Move agent to coverPoint.
        if (!agent.pathPending)
        {
            Vector3 velocity = agent.desiredVelocity;
            if (!enableRun)
            {
                velocity = velocity.normalized;
            }
            else
            {
                velocity = velocity;
            }

            velocity = new Vector3(velocity.x, 0, velocity.z);
            selfAgent.moveCharacter(velocity);
        }
        #endregion

        #region Step update Logic
        if (timeFromLastStep > stepIntervalInSeconds)
        {
            timeFromLastStep = 0;
            stepUpdate();
        }
        else
        {
            timeFromLastStep += Time.deltaTime;
        }
        #endregion
    }

    protected abstract void stepUpdate();
    public abstract void setWeaponFireCapability(bool enabled);
    public abstract void stopStageBehavior();
}
