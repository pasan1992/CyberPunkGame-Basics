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
}
