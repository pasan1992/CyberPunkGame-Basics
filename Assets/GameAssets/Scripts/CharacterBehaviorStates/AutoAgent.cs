using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ICyberAgent))]
public class AutoAgent : MonoBehaviour, AgentController
{
    public MovingAgent target;
    protected ICyberAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected NavMeshAgent m_navMeshAgent;
    public float health;

    public void setMovableAgent(ICyberAgent agent)
    {
        throw new System.NotImplementedException();
    }

    #region initaialize
    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_currentState = new CombatStage(m_movingAgent, target,m_navMeshAgent);
        m_movingAgent.setHealth(health);
        m_movingAgent.setWeponFireCapability(false);
    }
    #endregion

    #region update
    void Update()
    {
        if(m_movingAgent.IsFunctional())
        {
            m_currentState.updateStage();
        }
        else
        {
            m_currentState.stopStageBehavior();
        }
    }
    #endregion

    #region events

    void OnBecameVisible()
    {
        Debug.Log("Visible");
        m_currentState.setWeaponFireCapability(true);
    }

    void OnBecameInvisible()
    {
        m_currentState.setWeaponFireCapability(false);
    }

    #endregion
}
