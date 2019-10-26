using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgentDamagableObject : MonoBehaviour,DamagableObject
{
    private ICyberAgent m_movingAgent;

    public void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
    }
    public bool damage(float damageValue)
    {
        m_movingAgent.damageAgent(damageValue);
        return !m_movingAgent.IsFunctional();
    }

    public float getArmor()
    {
        return 0;
    }

    public float getRemaningHealth()
    {
       return m_movingAgent.GetAgentData().Health;
    }

    public float getTotalHealth()
    {
        return m_movingAgent.GetAgentData().MaxHealth;
    }

    public Transform getTransfrom()
    {
        return m_movingAgent.getTransfrom();
    }

    public bool isDestroyed()
    {
       return !m_movingAgent.IsFunctional();
    }
}
