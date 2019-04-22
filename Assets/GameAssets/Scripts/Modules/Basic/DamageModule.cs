
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
        m_onDestroy();
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
            destroyCharacter();
        }
        
    }

    public virtual void DamageByAmount(float amount)
    {
        m_health -= amount;
        if (m_health < 0)
        {
            m_health = 0;
            destroyCharacter();
        }
    }

    public virtual bool IsFunctional()
    {
        return m_health > 0;
    }

    #endregion
}
