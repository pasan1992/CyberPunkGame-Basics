using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgentDamagableObject : MonoBehaviour,DamagableObject
{
    protected ICyberAgent m_movingAgent;

    public void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
    }
    public virtual bool damage(float damageValue,Collider collider, Vector3 force, Vector3 point)
    {
        m_movingAgent.damageAgent(damageValue);
        return !m_movingAgent.IsFunctional();
    }

    public virtual float getArmor()
    {
        return 0;
    }

    public virtual float getRemaningHealth()
    {
       return m_movingAgent.GetAgentData().Health;
    }

    public virtual float getTotalHealth()
    {
        return m_movingAgent.GetAgentData().MaxHealth;
    }

    public virtual Transform getTransfrom()
    {
        return m_movingAgent.getTransfrom();
    }

    public virtual bool isDestroyed()
    {
       return !m_movingAgent.IsFunctional();
    }

    public virtual bool isDamagable(AgentBasicData.AgentFaction faction)
    {
        return m_movingAgent.getFaction() != faction;
    }
}
