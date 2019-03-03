using UnityEngine;
using RootMotion.FinalIK;


public class DamageSystem
{
    // Start is called before the first frame update
    protected float m_health;
    protected RagdollUtility m_ragdoll;
    protected HitReaction m_hitReaction;
    protected AgentAnimationSystem m_animationSystem;

    public DamageSystem(float health,RagdollUtility ragdoll, HitReaction hitReaction)
    {
        m_health = health;
        m_ragdoll = ragdoll;
        m_hitReaction = hitReaction;
    }

    #region Commands

    public void destroyCharacter()
    {
        m_ragdoll.EnableRagdoll();
        //m_animationSystem.disableAnimationSystem();
    }

    #endregion

    #region getters and setters

    public float getHealth()
    {
        return m_health;
    }

    public void setHealth(float health)
    {
        m_health = health;

        if(m_health <0)
        {
            m_health = 0;
        }
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        m_hitReaction.Hit(collider, force, point);
    }

    public void DamageByAmount(float amount)
    {
        m_health -= amount;
        if(m_health<0)
        {
            m_health = 0;
        }
    }

    public bool IsFunctional()
    {
        return m_health > 0;
    }

    #endregion
}
