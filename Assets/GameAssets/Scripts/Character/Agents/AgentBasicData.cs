using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentBasicData
{
    public enum AGENT_NATURE { DROID,DRONE,PLAYER,HUMANOID}


    [Header("Agent Static Parameters")]
    [Tooltip("This governs non functional behavior of the unity Ex:- Post destory effects")]
    public AGENT_NATURE AgentNature; 
    public float Skill;

    [SerializeField] 
    private float maxHealth;


    [Space(10)]
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
}
