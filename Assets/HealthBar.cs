using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image m_healtBar;
    private Transform m_target;

    #region Initialize
    public void Start()
    {
        m_target = Camera.main.transform;
    }
    #endregion


    #region Update

    private void Update()
    {
        Vector3 targetPos = m_target.transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
    }

    #endregion

    #region Getters and Setters
    //public void setMaxHealth(float health)
    //{
    //    calculateHealthBar();
    //    m_maxHealth = health;
    //}

    //public float getMaxHealth()
    //{
    //    return m_maxHealth;
    //}

    //public void setHealth(float health)
    //{
    //    m_currentHealth = health;
    //    calculateHealthBar();
    //}

    //public float getHealth()
    //{
    //    return m_currentHealth;
    //}

    public void setHealthPercentage(float value)
    {
        m_healtBar.fillAmount = value;
        m_healtBar.color = Color.Lerp(Color.red, Color.green, value);
    }

    #endregion

    #region Events
    #endregion

    #region Utility
    #endregion
}
