using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ICyberAgent))]
public class AutoHumanoidAgentController :  AgentController
{
    public MovingAgent target;
    protected ICyberAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected NavMeshAgent m_navMeshAgent;
    public float health;
    public float skillLevel;

    #region initaialize
    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_currentState = new CombatStage(m_movingAgent, target,m_navMeshAgent,this);
        m_movingAgent.setHealth(health);
        m_movingAgent.setWeponFireCapability(false);
        ((MovingAgent)m_movingAgent).setOndestroyCallback(OnAgentDestroy);
        m_movingAgent.setFaction(m_agentFaction);
    }
    #endregion

    #region update
    void Update()
    {
        if(m_movingAgent.IsFunctional())
        {
            m_currentState.updateStage();
        }
    }
    #endregion

    #region events

    void OnBecameVisible()
    {
        //Debug.Log("Visible");
        m_currentState.setWeaponFireCapability(true);
    }

    void OnBecameInvisible()
    {
        m_currentState.setWeaponFireCapability(false);
    }

    void OnAgentDestroy()
    {
        m_navMeshAgent.enabled = false;
    }

    #endregion

    #region getters and setters

    public override float getSkill()
    {
        return skillLevel;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = agent;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_movingAgent;
    }
    #endregion
}
