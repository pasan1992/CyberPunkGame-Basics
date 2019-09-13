using UnityEngine;

public class DamageModule
{
    public delegate void OnDestoryDeligate();
    protected OnDestoryDeligate m_onDestroy;
    protected Outline m_outLine;

    protected AgentBasicData m_basicData;

    public DamageModule(AgentBasicData basicData,OnDestoryDeligate onDestroyCallback,Outline outline)
    {
        m_basicData = basicData;
        m_onDestroy += onDestroyCallback;
        m_outLine = outline;
        m_outLine.OutlineColor = Color.green;
    }

    #region commands

    public virtual void destroyCharacter()
    {
    }
    #endregion

    #region getters and setters
    public virtual void resetCharacter()
    {
        m_outLine.enabled = true;
        m_basicData.Health = m_basicData.MaxHealth;
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);
    }

    public void setHealth(float health)
    {
        m_basicData.Health = health;
        m_basicData.MaxHealth = health;
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);

        if (m_basicData.Health <= 0)
        {
            m_basicData.Health = 0;
            m_outLine.enabled = false;
            destroyCharacter();
            m_onDestroy();
        }
        
    }

    public void DamageByAmount(float amount)
    {
        m_basicData.Health -= amount;

        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);

        if (m_basicData.Health <= 0)
        {
            m_basicData.Health = 0;
            m_outLine.enabled = false;
            destroyCharacter();
            m_onDestroy();
        }
    }

    public virtual bool HealthAvailable()
    {
        return m_basicData.Health > 0;
    }

    public Color getHealthColor()
    {
       return m_outLine.OutlineColor;
    }

    public float getHealthPercentage()
    { 
        if(m_basicData.Health != 0)
        {
            return m_basicData.Health / m_basicData.MaxHealth;
        }
        else
        {
            return 0;
        }
    }

    #endregion
}
