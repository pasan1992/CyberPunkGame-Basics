﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
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
    public enum CharacterMainStates { Aimed, Armed_not_Aimed, Dodge, Idle,Interaction }
    private CharacterMainStates m_characterState = CharacterMainStates.Idle;
    private CharacterMainStates m_previousTempState = CharacterMainStates.Idle;
    protected GameObject m_target;
    private bool m_characterEnabled = true;
    //private AgentBasicData.AgentFaction m_agentFaction;
    private Vector3 m_movmentVector;
    private bool m_isDisabled = false;

    // Public
    public AgentData AgentData;

    public AgentFunctionalComponents AgentComponents;

    public Interactable tempInteractionObj; 

    #endregion

    #region Initalize
    public virtual void Awake()
    {
        m_target = new GameObject();
        m_movmentVector = new Vector3(0, 0, 0);

        // Create Animation system.
        AimIK aimIK = this.GetComponent<AimIK>();
        aimIK.solver.target = m_target.transform;
        m_animationModule = new HumanoidAnimationModule(this.GetComponent<Animator>(), this.GetComponent<AimIK>(),AgentComponents, 10);

        // Create equipment system.
        RangedWeapon[] currentWeapons = this.GetComponentsInChildren<RangedWeapon>();
        WeaponProp[] currentWeaponProps = this.GetComponentsInChildren<WeaponProp>();
        
        m_equipmentModule = new HumanoidRangedWeaponsModule(currentWeaponProps, m_characterState, m_target, GetComponent<Recoil>(), m_animationModule,AgentData,AgentComponents);

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

    [ContextMenu("ActivateTempInteraction")]
    public void TempFireInteraction()
    {
        interactWith(tempInteractionObj);
    }

    public void interactWith(Interactable interactableObj)
    {
        switch(interactableObj.properties.Type)
        {
            case Interactable.InteractableProperties.InteractableType.Pickup:
            break;
            case Interactable.InteractableProperties.InteractableType.Switch:
            break;
            case Interactable.InteractableProperties.InteractableType.TimedInteraction:
            StartCoroutine(onTimedInteraction(interactableObj));
            break;
        }
    }

    private IEnumerator onTimedInteraction(Interactable interactableObj)
    {

        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        this.transform.position = interactableObj.transform.position + interactableObj.properties.offset;
        this.transform.rotation = Quaternion.Euler(interactableObj.properties.rotation);
        // while(Vector3.Distance(transform.position,intendedPosition) < 0.3f && intentedRotation == this.transform.rotation)
        // {
        //     this.transform.rotation = Quaternion.Lerp(this.transform.rotation,intentedRotation,0.1f);
        //     this.transform.position = Vector3.Lerp(this.transform.position,intendedPosition,0.1f);
        //     yield return null;
        // }

        m_previousTempState = m_characterState;
        m_characterState = CharacterMainStates.Interaction;
        m_animationModule.setTimedInteraction(true,interactableObj.properties.interactionID);
        yield return new WaitForSeconds(interactableObj.properties.interactionTime);
        m_animationModule.setTimedInteraction(false,interactableObj.properties.interactionID);
        m_characterState = m_previousTempState;
    }

    

    public void pickupItem()
    {
        bool pickupCondition = (m_characterState.Equals(CharacterMainStates.Idle) || m_characterState.Equals(CharacterMainStates.Armed_not_Aimed));
        if(pickupCondition)
        {
            Interactable obj = AgentItemFinder.findNearItem(getCurrentPosition());

            if(obj != null)
            {
                float distance = Vector3.Distance(getCurrentPosition(),obj.transform.position);

                if(distance>0.7f)
                {
                    m_movmentModule.LookAtObject(obj.transform.position);
                }
                
                m_animationModule.triggerPickup();
                m_previousTempState = m_characterState;
                m_characterState = CharacterMainStates.Interaction;

                if(isCrouched())
                {
                    StartCoroutine(onPickup(obj,0));
                }
                else
                {
                    StartCoroutine(onPickup(obj,0.3f));
                }

            }
        }
    }

    IEnumerator onPickup(Interactable obj,float waitTime)
    {
        m_animationModule.setUpperAnimationLayerWeight(0.2f);
        yield return new WaitForSeconds(waitTime);
        m_animationModule.setUpperAnimationLayerWeight(1);

        if(obj.properties.Type.Equals(Interactable.InteractableProperties.InteractableType.Pickup))
        {
            if(obj is RangedWeapon)
            {
                if(obj is PrimaryWeapon)
                {
                    if(!AgentData.primaryWeapon)
                    {
                        AgentData.primaryWeapon = obj.GetComponent<PrimaryWeapon>();
                        m_equipmentModule.equipWeapon(AgentData.primaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        AgentData.inventryItems.Add(obj);
                        obj.OnPickUpAction();
                    }
                }
                else if(obj is SecondaryWeapon)
                {
                    if(!AgentData.secondaryWeapon)
                    {
                        AgentData.secondaryWeapon = obj.GetComponent<SecondaryWeapon>();
                        m_equipmentModule.equipWeapon(AgentData.secondaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        AgentData.inventryItems.Add(obj);
                        obj.OnPickUpAction();
                    }
                }
            }
        }

        // switch(obj.properties.Type)
        // {
        //     case Interactable.InteractableProperties.InteractableType.PrimaryWeapon:

        //         if(!AgentData.primaryWeapon)
        //         {
        //             AgentData.primaryWeapon = obj.GetComponent<PrimaryWeapon>();
        //             m_equipmentModule.equipWeapon(AgentData.primaryWeapon);
        //             obj.OnEquipAction();
        //         }
        //         else
        //         {
        //             AgentData.inventryItems.Add(obj);
        //             obj.OnPickUpAction();
        //         }

        //     break;
        //     case Interactable.InteractableProperties.InteractableType.SecondaryWeapon:

        //         if(!AgentData.secondaryWeapon)
        //         {
        //             AgentData.secondaryWeapon = obj.GetComponent<SecondaryWeapon>();
        //             m_equipmentModule.equipWeapon(AgentData.secondaryWeapon);
        //             obj.OnEquipAction();
        //         }
        //         else
        //         {
        //             AgentData.inventryItems.Add(obj);
        //             obj.OnPickUpAction();
        //         }
            
        //     break;

        //     default:
        //             AgentData.inventryItems.Add(obj);
        //             obj.OnPickUpAction();
        //     break;
        // }

         yield return new WaitForSeconds(waitTime);
        m_characterState = m_previousTempState;
        // Now do your thing here
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
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon() /*&& !m_characterState.Equals(CharacterMainStates.Dodge)*/)
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

    private void damageAgent()
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
        // To make sure that weapon toggle won't happen mid interaction.
        if(!m_characterState.Equals(CharacterMainStates.Interaction))
        {
             // !improtant. Returns the character state after the toggle.
            m_characterState = m_equipmentModule.togglePrimary();
        }
        
    }

    public void togglepSecondaryWeapon()
    {
        // To make sure that weapon toggle won't happen mid interaction.
        if(!m_characterState.Equals(CharacterMainStates.Interaction))
        {
             // !important Returns the character state after the toggle
            m_characterState = m_equipmentModule.toggleSecondary();
        }
       
    }

    public void toggleGrenede()
    {
         m_characterState = m_equipmentModule.toggleGrenede();
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

    public bool isReadyToAim()
    {
        return m_animationModule.isEquiped() && (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed));
    }

    public bool hasWeaponInHand()
    {
        return m_animationModule.isEquiped();
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

    public AgentBasicData.AgentFaction getFaction()
    {
        return AgentData.m_agentFaction;
    }

    public void setFaction(AgentBasicData.AgentFaction group)
    {
        AgentData.m_agentFaction = group;

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


    public RangedWeapon.WEAPONTYPE getCurrentWeaponType()
    {
        return m_equipmentModule.getCurrentWeapon().getWeaponType();
    }

    #endregion

    #region Events Handlers

    public void OnThrow()
    {
        m_equipmentModule.OnThrow();
    }
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

    public Vector3 getTargetPosition()
    {
        return m_target.transform.position;
    }

    public Transform getTargetTransfrom()
    {
        return m_target.transform;
    }

    public bool isInteracting()
    {
        return m_characterState == CharacterMainStates.Interaction;
    }
    public void setAnimationSpeed(float speed)
    {
        m_animationModule.setAnimationSpeed(speed);
    }

    public Vector3 getCurrentVelocity()
    {
        return m_movmentModule.getCurrentVelocity();
    }

    public AgentData GetAgentData()
    {
        return AgentData;
    }
    #endregion

    #region Commented Code
    #endregion
}

