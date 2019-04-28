
public class DamageModule
{
    protected float m_health;
    public delegate void OnDestoryDeligate();
    protected OnDestoryDeligate m_onDestroy;

    public DamageModule(float health,OnDestoryDeligate onDestroyCallback)
    {
        m_health = health;
        m_onDestroy += onDestroyCallback;
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

        if (m_health <= 0)
        {
            m_health = 0;
            destroyCharacter();
            m_onDestroy();
        }
        
    }

    public void DamageByAmount(float amount)
    {
        m_health -= amount;
        if (m_health <= 0)
        {
            m_health = 0;
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
