using UnityEngine;

public class DamageModule
{
    protected float m_health;
    public delegate void OnDestoryDeligate();
    protected OnDestoryDeligate m_onDestroy;
    protected Outline m_outLine;
    protected float m_maxHealth;

    public DamageModule(float health,OnDestoryDeligate onDestroyCallback,Outline outline)
    {
        m_health = health;
        m_maxHealth = health;
        m_onDestroy += onDestroyCallback;
        m_outLine = outline;
    }

    #region commands

    public virtual void destroyCharacter()
    {
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
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_health / m_maxHealth);
        if (m_health <= 0)
        {
            m_health = 0;
            m_outLine.enabled = false;
            destroyCharacter();
            m_onDestroy();
        }
        
    }

    public void DamageByAmount(float amount)
    {
        m_health -= amount;
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_health / m_maxHealth);
        if (m_health <= 0)
        {
            m_health = 0;
            m_outLine.enabled = false;
            destroyCharacter();
            m_onDestroy();
        }
    }

    public virtual bool HealthAvailable()
    {
        return m_health > 0;
    }

    #endregion
}
