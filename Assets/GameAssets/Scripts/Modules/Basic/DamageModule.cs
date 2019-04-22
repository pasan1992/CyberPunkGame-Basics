using UnityEngine;

public class DamageModule
{
    protected float m_health;

    public DamageModule(float health)
    {
        m_health = health;
    }

    #region commands

    public virtual void destroyCharacter()
    {

    }
    #endregion

    #region getters and setters

    public virtual float getHealth()
    {
        return m_health;
    }

    public virtual void setHealth(float health)
    {
        m_health = health;

        if (m_health < 0)
        {
            m_health = 0;
        }
    }

    public virtual void DamageByAmount(float amount)
    {
        m_health -= amount;
        if (m_health < 0)
        {
            m_health = 0;
        }
    }

    public virtual bool IsFunctional()
    {
        return m_health > 0;
    }

    #endregion



}
