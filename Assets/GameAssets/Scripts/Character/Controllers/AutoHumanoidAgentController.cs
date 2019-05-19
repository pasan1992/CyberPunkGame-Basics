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

    #region initaialize
    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_currentState = new CombatStage(m_movingAgent, target,m_navMeshAgent);
        m_movingAgent.setHealth(health);
        m_movingAgent.setWeponFireCapability(false);
        intializeAgentCallbacks(m_movingAgent);
        //m_movingAgent.setOnDestoryCallback(OnAgentDestroy);
        //m_movingAgent.setOnDisableCallback(onAgentDisable);
        m_movingAgent.setFaction(m_agentFaction);
        m_movingAgent.setSkill(skillLevel);
        m_movingAgent.enableTranslateMovment(false);
    }
    #endregion

    #region update
    void Update()
    {
        if(m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled())
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

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        m_navMeshAgent.enabled = false;

        //TEMP CODE
        //Invoke("resetCharacher", Random.value*10 + 10);
    }

    public override void resetCharacher()
    {
        //TEMP CODE
        //this.transform.position = FindObjectOfType<SpawnPoint>().transform.position;

        m_navMeshAgent.enabled = true;
        m_movingAgent.resetAgent(health, skillLevel);
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
    #endregion
}
