using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using humanoid;

[RequireComponent(typeof(ICyberAgent))]
public class AutoHumanoidAgentController :  AgentController
{
    public MovingAgent target;
    protected ICyberAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected NavMeshAgent m_navMeshAgent;
    public float health;
    public float skillLevel;

    public BasicWaypoint[] basicWaypoints;

    #region initaialize

    private void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
    }

    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_currentState = new CombatStage(m_movingAgent, target,m_navMeshAgent);
        m_movingAgent.setHealth(health);
        m_movingAgent.setWeponFireCapability(false);
        intializeAgentCallbacks(m_movingAgent);
        m_movingAgent.setFaction(m_agentFaction);
        m_movingAgent.setSkill(skillLevel);
        m_movingAgent.enableTranslateMovment(false);
    }
    #endregion

    #region update
    void Update()
    {
        if(m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            m_currentState.updateStage();
            m_movingAgent.setAnimationSpeed(m_navMeshAgent.velocity.normalized.magnitude/0.8f);
        }
    }
    #endregion

    #region events

    void OnBecameVisible()
    {
        m_currentState.setWeaponFireCapability(true);
    }

    void OnBecameInvisible()
    {
        m_currentState.setWeaponFireCapability(false);
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        m_navMeshAgent.enabled = false;

        // reset character
        Invoke("reUseCharacter", m_timeToReset);
    }

    private void reUseCharacter()
    {
        setInUse(false);
    }

    public override void resetCharacher()
    {
        if (m_navMeshAgent != null)
        {
            m_navMeshAgent.enabled = true;
        }

        if (m_movingAgent != null)
        {
            m_movingAgent.resetAgent(health, skillLevel);
        }
    }

    public override void onAgentDisable()
    {
        m_navMeshAgent.enabled = false;
    }

    public override void onAgentEnable()
    {
        m_navMeshAgent.enabled = true;
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

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }
    #endregion
}
