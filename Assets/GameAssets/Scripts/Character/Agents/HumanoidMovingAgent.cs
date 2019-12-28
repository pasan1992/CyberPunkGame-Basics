using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class HumanoidMovingAgent : MonoBehaviour, ICyberAgent
{
    #region parameters

    // Callback
    private GameEvents.BasicNotifactionEvent m_onDestoryCallback;
    private GameEvents.BasicNotifactionEvent m_onDisableCallback;
    private GameEvents.BasicNotifactionEvent m_onEnableCallback;

    // Main Modules
    protected HumanoidRangedWeaponsModule m_equipmentModule;
    protected HumanoidAnimationModule m_animationModule;
    protected HumanoidMovmentModule m_movmentModule;
    protected HumanoidDamageModule m_damageModule;
    protected HumanoidInteractionModule m_interactionModule;

    // Attributes
    public enum CharacterMainStates { Aimed, Armed_not_Aimed, Dodge, Idle,Interaction }
    public CharacterMainStates m_characterState = CharacterMainStates.Idle;
    private CharacterMainStates m_previousTempState = CharacterMainStates.Idle;
    protected GameObject m_target;
    private bool m_characterEnabled = true;
    private Vector3 m_movmentVector;
    private bool m_isDisabled = false;
    private GameEvents.BasicNotifactionEvent m_onDamagedCallback;
    // Public
    // Main Agent data component
    public AgentData AgentData;

    // These components required for the function of the agent
    public AgentFunctionalComponents AgentComponents;


    //Event Callbacks
    private GameEvents.BasicNotifactionEvent m_onReloadEndCallback;
    private GameEvents.BasicNotifactionEvent m_onInteractionEndCallback;

    private GameEvents.BasicNotifactionEvent m_onWeaponEquipCallback;
    private GameEvents.BasicNotifactionEvent m_onWeaponUnEquipCallback;
    private GameEvents.BasicNotifactionEvent m_onThrowCallback;
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
        NavMeshAgent navMeshAgent = this.GetComponent<NavMeshAgent>();

        m_movmentModule = new HumanoidMovmentModule(this.transform, m_characterState, m_target, m_animationModule,navMeshAgent);

        // Create Damage module
        m_damageModule = new HumanoidDamageModule(AgentData, 
        this.GetComponent<RagdollUtility>(), 
        this.GetComponentInChildren<HitReaction>(),
        m_animationModule, findHeadTransfrom(), 
        findChestTransfrom(), 
        destroyCharacter, 
        this.GetComponentInChildren<Outline>());

        // Create intearction system.
        m_interactionModule = new HumanoidInteractionModule(m_animationModule,
        m_movmentModule,
        AgentData,
        m_equipmentModule,
        navMeshAgent,
        // This is the callback for interaction done.
        OnInteractionDone);
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

    /**
     Interact with given interatable object
     - Interaction type can be different from the interaction type of the interatable object given.
    */
    public void interactWith(Interactable obj,Interactable.InteractableProperties.InteractableType type)
    {
        bool interactCondition =( (m_characterState.Equals(CharacterMainStates.Idle) || 
        m_characterState.Equals(CharacterMainStates.Armed_not_Aimed)) && !m_characterState.Equals(CharacterMainStates.Interaction));
        
        // Check if character is ready to interact - Do not interact if the character is already interacting.
        if(interactCondition)
        {
            // Check if the object in interaction is already in interacting state by other agent.
            if(obj != null && !obj.isInteracting())
            {
                m_movmentVector = Vector3.zero;
                m_previousTempState = m_characterState;
                m_characterState = CharacterMainStates.Interaction;
                StartCoroutine(m_interactionModule.interactWith(obj,type));
            }
        }      
    }

    /**
     Cancle the current interaction
    */
    public void cancleInteraction()
    {
        m_interactionModule.cancleInteraction();
    }
    public void Interact()
    {
        Interactable obj = AgentItemFinder.findNearItem(getCurrentPosition());
        
        if(obj)
        {
            interactWith(obj,obj.properties.Type);
        }
    }

    public void damageAgent(float amount)
    {
        m_damageModule.DamageByAmount(amount);

        if(m_onDamagedCallback != null)
        {
            m_onDamagedCallback();
        }
        
    }

    // This fire wepon is called by the AI Agent controlers to fire weapon.
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

    // Aim Current Weapon 
    public virtual void aimWeapon()
    {
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon())
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
        //Invoke("postDestoryEffect", 1);

        if (m_onDestoryCallback != null)
        {
            Debug.Log("Destroy Get called");
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

    public void setOnDamagedCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDamagedCallback += callback;
    }

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

    public void setOnDestoryCallback(GameEvents.BasicNotifactionEvent onDestoryCallback)
    {
        m_onDestoryCallback += onDestoryCallback;
    }

    public void setOnDisableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDisableCallback += callback;
    }

    public void setOnEnableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onEnableCallback += callback;
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

    // public bool isEquiped()
    // {
    //     return (m_animationModule.isEquiped() || m_equipmentModule.isInEquipingAction());
    // }

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

    // Return true if weapon is already hosted or actualy wepon is husted from this command
    public bool hosterWeapon()
    {
        if((m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed)) && !m_equipmentModule.isInEquipingAction() && !m_equipmentModule.isReloading())
        {
            switch (m_equipmentModule.getCurrentWeapon().getWeaponType())
            {
                case RangedWeapon.WEAPONTYPE.grenede:
                    toggleGrenede();
                break;
                case Weapon.WEAPONTYPE.primary:
                    togglePrimaryWeapon();
                break;
                case Weapon.WEAPONTYPE.secondary:
                    togglepSecondaryWeapon();
                break;
            }
            return true;
        }
        else if(m_characterState.Equals(CharacterMainStates.Idle))
        {
            return true;
        }
        return false;
    }

    public IEnumerator waitTillWeaponHosted()
    {
        while(!this.hosterWeapon())
        {
            yield return null;
        }
    }

    #endregion

    #region Events Handlers

    public void OnInteractionDone()
    {
        if(m_characterState == CharacterMainStates.Interaction)
        {
            m_characterState = m_previousTempState;
            OnInteractionDone();
        }      
    }

    public void OnThrow()
    {
        m_equipmentModule.OnThrow();
    }
    public void ReloadEnd()
    {
        m_equipmentModule.ReloadEnd();

        if(m_onReloadEndCallback != null)
        {
            m_onReloadEndCallback();
        }
        
    }

    public void EquipAnimationEvent()
    {
        m_equipmentModule.EquipAnimationEvent();

        if(m_onWeaponEquipCallback != null)
        {
            m_onWeaponEquipCallback();
        }
        
    }

    public void UnEquipAnimationEvent()
    {
        m_equipmentModule.UnEquipAnimationEvent();

        if(m_onWeaponUnEquipCallback != null)
        {
            m_onWeaponUnEquipCallback();
        }
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

    public void setBasicCallbacks(
        GameEvents.BasicNotifactionEvent onEquip,
        GameEvents.BasicNotifactionEvent onUnequip,
        GameEvents.BasicNotifactionEvent onReloadEnd,
        GameEvents.BasicNotifactionEvent onDamaged,
        GameEvents.BasicNotifactionEvent onDestoryCallback,
        GameEvents.BasicNotifactionEvent onInteractionDone,
        GameEvents.BasicNotifactionEvent onThrowItem
    )
    {
        m_onWeaponEquipCallback +=onEquip;
        m_onWeaponUnEquipCallback += onUnequip;
        m_onDestoryCallback +=onDestoryCallback;
        m_onDamagedCallback +=onDamaged;
        this.m_onInteractionEndCallback += onInteractionDone;
        this.m_onThrowCallback += onThrowItem;
        // if(onDestoryCallback !=null)
        // {
        //     m_onDestoryCallback +=onDestoryCallback;
        // }

        // if(onEquip != null)
        // {
            
        // }

        // if(onUnequip != null)
        // {

        // }

        // if(onReloadEnd != null)
        // {

        // }

        // if(onDamaged != null)
        // {

        // }        
        // if(onDestoryCallback != null)
        // {

        // }
    }
    #endregion

    #region Commented Code
    #endregion
}

