using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image m_healtBar;
    public Image m_background;
    private Transform m_target;
    public bool TurnTowardsCamera = true;
    public bool AppearWhenDamaged = false;

    private float m_previousPercentage = 1;
    #region Initialize
    public void Start()
    {
        m_target = Camera.main.transform;

        if(AppearWhenDamaged)
        {
            makeHealthBarInvisible();
        }
    }
    #endregion


    #region Update

    private void Update()
    {
        if(TurnTowardsCamera)
        {
            Vector3 targetPos = m_target.transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
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
        if(AppearWhenDamaged)
        {
            OnlyEnableHealthBarWhenDamaged(value);
        }

        if(m_healtBar)
        {
            m_healtBar.fillAmount = value;
            m_healtBar.color = Color.Lerp(Color.red, Color.green, value);
        }


    }

    private void OnlyEnableHealthBarWhenDamaged(float percentage)
    {
        if (m_previousPercentage != percentage)
        {
            makeHealthBarVisible();
            CancelInvoke();
            Invoke("makeHealthBarInvisible", 1);
            m_previousPercentage = percentage;
        }
    }

    private void makeHealthBarVisible()
    {
        m_background.gameObject.SetActive(true);
        m_healtBar.gameObject.SetActive(true);
    }

    private void makeHealthBarInvisible()
    {
        m_background.gameObject.SetActive(false);
        m_healtBar.gameObject.SetActive(false);
    }

    #endregion

    #region Events
    #endregion

    #region Utility
    #endregion
}
