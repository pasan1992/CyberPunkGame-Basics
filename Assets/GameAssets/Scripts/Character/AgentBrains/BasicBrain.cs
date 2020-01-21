using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BasicBrain 
{
    public BasicBrain(NavMeshAgent navMeshAgent, ICyberAgent movingAgent)
    {
        Initialize(navMeshAgent,movingAgent);
    }
    private NavMeshAgent m_navMeshAgent;
    protected ICyberAgent m_movingAgent;
    public virtual void  Initialize(NavMeshAgent navMeshAgent, ICyberAgent movingAgent)
    {
        m_navMeshAgent = navMeshAgent;
        m_movingAgent = movingAgent;
    }
    public abstract void Update();
    public abstract void alertDamage();
    public abstract void onAllClear();    
    public abstract void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction);
    public abstract void onEnemyDetection(ICyberAgent opponent);
    public abstract void onAgentDisable();
    public abstract void onDamaged();
    public abstract void OnAgentDestroy();
}
