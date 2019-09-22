﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;

public class HumanoidMovingAgent : MonoBehaviour, ICyberAgent
{
    #region parameters

    // Callback
    private AgentController.agentBasicEventDelegate m_onDestoryCallback;
    private AgentController.agentBasicEventDelegate m_onDisableCallback;
    private AgentController.agentBasicEventDelegate m_onEnableCallback;

    // Main Modules
    protected HumanoidRangedWeaponsModule m_equipmentModule;
    protected HumanoidAnimationModule m_animationModule;
    protected HumanoidMovmentModule m_movmentModule;
    protected HumanoidDamageModule m_damageModule;


    // Attributes
    public enum CharacterMainStates { Aimed, Armed_not_Aimed, Dodge, Idle }
    private CharacterMainStates m_characterState = CharacterMainStates.Idle;
    protected GameObject m_target;
    private bool m_characterEnabled = true;
    private AgentController.AgentFaction m_agentFaction;
    private Vector3 m_movmentVector;
    private bool m_isDisabled = false;

    // Public
    public AgentData AgentData;

    #endregion

    #region Initalize
    public virtual void Awake()
    {
        m_target = new GameObject();
        m_movmentVector = new Vector3(0, 0, 0);

        // Create Animation system.
        AimIK aimIK = this.GetComponent<AimIK>();
        aimIK.solver.target = m_target.transform;
        m_animationModule = new HumanoidAnimationModule(this.GetComponent<Animator>(), this.GetComponent<AimIK>(), 10);

        // Create equipment system.
        Weapon[] currentWeapons = this.GetComponentsInChildren<Weapon>();
        WeaponProp[] currentWeaponProps = this.GetComponentsInChildren<WeaponProp>();
        
        m_equipmentModule = new HumanoidRangedWeaponsModule(currentWeaponProps, m_characterState, m_target, GetComponent<Recoil>(), m_animationModule,AgentData);

        // Create movment system.
        m_movmentModule = new HumanoidMovmentModule(this.transform, m_characterState, m_target, m_animationModule,this.GetComponent<NavMeshAgent>());

        // Create Damage module
        m_damageModule = new HumanoidDamageModule(AgentData, 
        this.GetComponent<RagdollUtility>(), 
        this.GetComponentInChildren<HitReaction>(),
        m_animationModule, findHeadTransfrom(), 
        findChestTransfrom(), 
        destroyCharacter, 
        this.GetComponentInChildren<Outline>());
    }
    #endregion

    #region Updates
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_characterEnabled)
        {
            // Update Systems.
            m_animationModule.UpdateAnimationState(m_characterState);
            m_movmentModule.UpdateMovment((int)m_characterState, m_movmentVector);
            m_equipmentModule.UpdateSystem(m_characterState);
            m_damageModule.update();
        }
    }
    #endregion

    #region Commands

    public void pickupItem()
    {
       GameObject obj = AgentItemFinder.findNearItem(getCurrentPosition());

       if(obj != null)
       {
           m_movmentModule.LookAtObject(obj.transform.position);
           m_animationModule.triggerPickup();
           Weapon weapon = obj.GetComponent<Weapon>();

           if(weapon)
           {
               m_equipmentModule.equipWeapon(weapon);
           }
       }
    }
    public void damageAgent(float amount)
    {
        m_damageModule.DamageByAmount(amount);
    }
    public virtual void weaponFireForAI()
    {
        if(m_equipmentModule.getCurrentWeaponAmmoCount() > 0)
        {
            StartCoroutine(fireWeapon());
            return;
        }
        m_equipmentModule.reloadCurretnWeapon();
    }

    public void reloadCurretnWeapon()
    {
        m_equipmentModule.reloadCurretnWeapon();
    }

    public virtual void weaponFireForAICover()
    {
        StartCoroutine(fireWeaponCover());
    }

    // Aim Current Weapon -
    public virtual void aimWeapon()
    {
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon() && !m_characterState.Equals(CharacterMainStates.Dodge))
        {
            
            m_characterState = CharacterMainStates.Aimed;
            m_equipmentModule.getCurrentWeapon().setAimed(true);
        }
    }

    // Stop Aiming current Weapon.
    public virtual void stopAiming()
    {
        if (m_characterState.Equals(CharacterMainStates.Aimed))
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
            m_equipmentModule.getCurrentWeapon().setAimed(false);
        }
    }

    // Move character
    public virtual void moveCharacter(Vector3 movmentDirection)
    {
        m_movmentVector = movmentDirection;
    }

    // Destory Character
    private void destroyCharacter()
    {
        this.GetComponent<FullBodyBipedIK>().enabled = false;
        m_equipmentModule.DropCurrentWeapon();
        m_characterEnabled = false;
        m_animationModule.disableAnimationSystem();
        Invoke("postDestoryEffect", 1);

        if (m_onDestoryCallback != null)
        {
            m_onDestoryCallback();
        }
    }

    private void postDestoryEffect()
    {
        switch(AgentData.AgentNature)
        {
            case AgentData.AGENT_NATURE.DROID:
                m_damageModule.emitSmoke();
            break;            
        }            
    }

    public void toggleHide()
    {
        m_animationModule.toggleCrouched();
    }

    public void togglePrimaryWeapon()
    {
        // !improtant. Returns the character state after the toggle.
        m_characterState = m_equipmentModule.togglePrimary();
    }

    public void togglepSecondaryWeapon()
    {
        // !important Returns the character state after the toggle
        m_characterState = m_equipmentModule.toggleSecondary();
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        m_damageModule.reactOnHit(collider, force, point);
    }

    public void dodgeAttack(Vector3 dodgeDirection)
    {
        if (!m_characterState.Equals(CharacterMainStates.Dodge))
        {
            m_characterState = CharacterMainStates.Dodge;
            m_animationModule.triggerDodge();
            m_movmentModule.dodge(dodgeDirection);
            m_equipmentModule.releaseTrigger();
        }
    }

    public void releaseTrigger()
    {
        m_equipmentModule.releaseTrigger();
    }

    public void pullTrigger()
    {
        m_equipmentModule.pullTrigger();
    }

    public void lookAtTarget()
    {
        m_movmentModule.lookAtTarget();
    }

    #endregion

    #region Getters and Setters

    public Transform getChestTransfrom()
    {
        return m_damageModule.getChestTransfrom();
    }

    public Vector3 getMovmentDirection()
    {
        return m_movmentVector;
    }

    public void resetAgent()
    {
        m_damageModule.resetCharacter();
        m_animationModule.enableAnimationSystem();
        m_characterEnabled = true;
        m_equipmentModule.resetWeapon();
        this.GetComponent<FullBodyBipedIK>().enabled = true;
    }

    public void setOnDestoryCallback(AgentController.agentBasicEventDelegate onDestoryCallback)
    {
        m_onDestoryCallback = onDestoryCallback;
    }

    public void setOnDisableCallback(AgentController.agentBasicEventDelegate callback)
    {
        m_onDisableCallback = callback;
    }

    public void setOnEnableCallback(AgentController.agentBasicEventDelegate callback)
    {
        m_onEnableCallback = callback;
    }

    public bool IsFunctional()
    {
        return m_damageModule.HealthAvailable();
    }

    public bool isEquiped()
    {
        return m_animationModule.isEquiped() && (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed));
    }

    public void setTargetPoint(Vector3 position)
    {
        m_target.transform.position = position;
    }

    public bool isEquipingWeapon()
    {
        return m_equipmentModule.isInEquipingAction();
    }

    public void setHealth(float health)
    {
        //m_damageModule.setHealth(m_agentStats.Health);
    }

    public void enableTranslateMovment(bool enable)
    {
        m_movmentModule.setPhysicalLocationChange(enable);
    }

    public Vector3 getCurrentPosition()
    {
        return this.transform.position;
    }

    public Vector3 getTopPosition()
    {
        return m_damageModule.getHeadTransfrom().position;
    }

    public Transform getHeadTransfrom()
    {
        return m_damageModule.getHeadTransfrom();
    }

    public virtual void setWeponFireCapability(bool enadled)
    {
        if (m_equipmentModule.getCurrentWeapon() != null)
        {
            m_equipmentModule.getCurrentWeapon().setWeaponSafty(!enadled);
        }
    }

    public bool isCrouched()
    {
        return m_movmentModule.isCrouched();
    }

    public AgentController.AgentFaction getFaction()
    {
        return m_agentFaction;
    }

    public void setFaction(AgentController.AgentFaction group)
    {
        m_agentFaction = group;

        if (m_equipmentModule != null)
        {
            m_equipmentModule.setOwnerFaction(group);
        }
    }

    public CharacterMainStates getCharacterMainStates()
    {
        return m_characterState;
    }

    public Transform getTransfrom()
    {
        return this.transform;
    }

    public void setSkill(float skill)
    {
        AgentData.Skill = skill;
    }

    public float getSkill()
    {
        return AgentData.Skill;
    }

    public bool isAimed()
    {
        return m_equipmentModule.isProperlyAimed();
    }


    public bool isDisabled()
    {
        return m_isDisabled;
    }

    public int getPrimaryWeaponAmmoCount()
    {
        return m_equipmentModule.getPrimaryWeaponAmmoCount();
    }

    public int getSecondaryWeaponAmmoCount()
    {
        return m_equipmentModule.getSecondaryWeaponAmmoCount();
    }

    public void setPrimayWeaponAmmoCount(int count)
    {
        m_equipmentModule.setPrimayWeaponAmmoCount(count);
    }

    public void setSecondaryWeaponAmmoCount(int count)
    {
        m_equipmentModule.setSecondaryWeaponAmmoCount(count);
    }


    public Weapon.WEAPONTYPE getCurrentWeaponType()
    {
        return m_equipmentModule.getCurrentWeapon().getWeaponType();
    }

    #endregion

    #region Events Handlers
    public void ReloadEnd()
    {
        m_equipmentModule.ReloadEnd();
    }

    public void EquipAnimationEvent()
    {
        m_equipmentModule.EquipAnimationEvent();
    }

    public void UnEquipAnimationEvent()
    {
        m_equipmentModule.UnEquipAnimationEvent();
    }

    public bool isCharacterEnabled()
    {
        return m_characterEnabled;
    }


    public bool isHidden()
    {
        return m_movmentModule.isCrouched();
    }

    public void DodgeEnd()
    {
        if (m_animationModule.isEquiped())
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            m_characterState = CharacterMainStates.Idle;
        }
    }
    #endregion

    #region Helper Functions

    private IEnumerator fireWeapon()
    {
        m_equipmentModule.pullTrigger();
        yield return new WaitForSeconds(0.5f);
        m_equipmentModule.releaseTrigger();
    }

    private IEnumerator fireWeaponCover()
    {
        aimWeapon();
        yield return new WaitForSeconds(2f);
        aimWeapon();
        //pullTrigger();
        m_equipmentModule.pullTrigger();
        yield return new WaitForSeconds(1f);
        m_equipmentModule.releaseTrigger();
        yield return new WaitForSeconds(1f);
        stopAiming();
    }

    private Transform findHeadTransfrom()
    {
        Transform headTransfrom = null;
        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            if (rb.tag == "Head")
            {
                headTransfrom = rb.transform;
            }
        }
        return headTransfrom;
    }

    private Transform findChestTransfrom()
    {
        Transform headTransfrom = null;
        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            if (rb.tag == "Chest")
            {
                headTransfrom = rb.transform;
            }
        }

        return headTransfrom;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public GameObject getGameObject()
    {
        return this.transform.gameObject;
    }

    public void setAnimationSpeed(float speed)
    {
        m_animationModule.setAnimationSpeed(speed);
    }
    #endregion

    #region Commented Code
    #endregion
}

