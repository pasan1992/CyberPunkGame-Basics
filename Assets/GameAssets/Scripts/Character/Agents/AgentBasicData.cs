using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentBasicData
{
    public enum AGENT_NATURE { DROID,DRONE,PLAYER,HUMANOID}

#region Static
    [Header("Agent Static Parameters")]
    [Tooltip("This governs non functional behavior of the unity Ex:- Post destory effects")]
    public AGENT_NATURE AgentNature; 
    public float Skill;

    [SerializeField] 
    private float maxHealth;

    public enum AgentFaction { Player,Enemy,Neutral};
    public AgentFaction m_agentFaction;

    [SerializeField] 
    private float m_maxStamina;

    [SerializeField] 
    private float m_currentStamina;

    [SerializeField] 
    private float m_staminaRegenRate;

#endregion

[Space(10)]

#region Dynamic
      
    [Header("Agent Dynamic Parameters")]
    [SerializeField] 
    private float health;

    // Getters and Setters
    public float MaxHealth 
    {
        get => maxHealth;

        set 
        {
            maxHealth = value;
            Debug.Log("Set");

            if(maxHealth < Health)
            {
                Health = maxHealth;
            }
        }
     }

    public float Health 
    {
        get => health; 
        set
        {
            health = value;

            if(health > MaxHealth)
            {
                health = MaxHealth;
            }
        }
    }

    public float MaxStamina 
    { 
        get => m_maxStamina; 
        set
        {
            if(value < 0) m_currentStamina = AnimatorConstants.s_IdleSpeed;
            else m_maxStamina = value;
        } 
        
    }

    public float CurrentStamina 
    { 
        get => m_currentStamina; 
        set
        {
            if(value < 0) m_currentStamina = AnimatorConstants.s_IdleSpeed;
            else m_currentStamina = (value < m_maxStamina) ? value : m_maxStamina; 
        }   
    }
    
    public float StaminaRegenRate { get => m_staminaRegenRate; set => m_staminaRegenRate = value; }

#endregion
}
