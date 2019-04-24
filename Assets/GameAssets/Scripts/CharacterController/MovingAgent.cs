using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;

public class MovingAgent : MonoBehaviour,ICyberAgent
{
    #region parameters
    // Public - Editor Values
    public enum CharacterMainStates { Aimed, Armed_not_Aimed,Dodge, Idle }
    private DamageModule.OnDestoryDeligate m_onDestoryCallback;
    //public LayerMask floorHitLayerMask;
   // public LayerMask enemyHitLayerMask;
   // public bool isPlayer = false;

    // Attribute - Systems
    protected HumanoidEquipmentModule m_equipmentModule;
    protected HumanoidAnimationModule m_animationModule;
    protected HumanoidMovmentModule m_movmentModule;
    protected HumanoidDamageModule m_damageModule;
    //protected AgentController m_agentController;
    private Vector3 m_movmentVector;

    private NavMeshAgent tempNavMeshAgent;
   
    // Attributes
    private CharacterMainStates m_characterState = CharacterMainStates.Idle;
    protected GameObject m_target;
    bool m_characterEnabled = true;
    #endregion

    #region initalize
    public virtual void Awake ()
    {
        m_target = new GameObject();
        m_movmentVector = new Vector3(0, 0, 0);

        // Create Animation system.
        AimIK m_aimIK = this.GetComponent<AimIK>();
        m_aimIK.solver.target = m_target.transform;
        m_animationModule = new HumanoidAnimationModule(this.GetComponent<Animator>(), this.GetComponent<AimIK>(), 10);

        // Create equipment system.
        Weapon[] m_currentWeapons = this.GetComponentsInChildren<Weapon>();
        WeaponProp[] m_currentWeaponProps = this.GetComponentsInChildren<WeaponProp>();
        m_equipmentModule = new HumanoidEquipmentModule(m_currentWeapons, m_currentWeaponProps, this.transform.name, m_characterState, m_target,GetComponent<Recoil>(),m_animationModule);


        // Create movment system.
        m_movmentModule = new HumanoidMovmentModule(this.transform,m_characterState,m_target,m_animationModule);

        Transform tempHeadTransfrom = null;

        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            if (rb.tag == "Head")
            {
                tempHeadTransfrom = rb.transform;
            }
        }

        m_damageModule = new HumanoidDamageModule(5, this.GetComponent<RagdollUtility>(), this.GetComponentInChildren<HitReaction>(),m_animationModule, tempHeadTransfrom, DestroyCharacter);
        
        //if(isPlayer)
        //{
        //    m_agentController = new PlayerAgent(enemyHitLayerMask,floorHitLayerMask);
        //    m_agentController.setMovableAgent(this);


        //}
        //else
        //{
        //    m_agentController = new AIAgent();
        //    m_agentController.setMovableAgent(this);
        //}

        foreach (Weapon wp in m_currentWeapons)
        {
            wp.SetGunTargetLineStatus(true);
        }

        tempNavMeshAgent = this.GetComponent<NavMeshAgent>();
    }
    #endregion

    #region updates
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

            //if (m_characterEnabled && m_agentController != null)
            //{
            //    m_agentController.controllerUpdate();
            //}
        }
    }

    //private void Update()
    //{
    //    if (m_characterEnabled && m_agentController != null)
    //    {
    //        m_agentController.controllerUpdate();
    //    }
    //}
    #endregion

    #region Commands

    // Fire Weapon Once
    //public virtual void FireWeapon()
    //{
    //    if (m_equipmentSystem.isProperlyAimed() && m_characterState.Equals(CharacterMainStates.Aimed))
    //    {
    //        m_equipmentSystem.FireCurrentWeapon();
    //    }
    //}

    //// Fire weapon continously.
    //public virtual void continouseFire()
    //{
    //    if (m_equipmentSystem.isProperlyAimed() && m_characterState.Equals(CharacterMainStates.Aimed))
    //    {
    //        m_equipmentSystem.continouseFire();
    //    }
    //}

    public void damageAgent(float amount)
    {
        m_damageModule.DamageByAmount(amount);

        //if(m_damageSystem.getHealth() == 0)
        //{
        //    DestroyCharacter();
        //}
    }

    public virtual void pullTrigger()
    {
        if (m_equipmentModule.isProperlyAimed() && m_characterState.Equals(CharacterMainStates.Aimed))
        {
            m_equipmentModule.pullTrigger();
        }
    }

    public virtual void releaseTrigger()
    {
        m_equipmentModule.releaseTrigger();
    }

    public virtual void weaponFireForAI()
    {
        StartCoroutine(fireWeapon());
    }

    public virtual void WeaponFireForAICover()
    {
        StartCoroutine(fireWeaponCover());
    }

    // Aim Current Weapon -
    public virtual void AimWeapon()
    {
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon() && !m_characterState.Equals(CharacterMainStates.Dodge))
        {
            m_characterState = CharacterMainStates.Aimed;
            m_equipmentModule.getCurrentWeapon().setAimed(true);
        }
    }

    // Stop Aiming current Weapon.
    public virtual void StopAiming()
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
    public void DestroyCharacter()
    {
        m_equipmentModule.DropCurrentWeapon();
        m_characterEnabled = false;
        m_animationModule.disableAnimationSystem();

        if (m_onDestoryCallback != null)
        {   
            m_onDestoryCallback();
        }
    }

    public void toggleHide()
    {
        m_animationModule.toggleCrouched();

        // Disable moving agent head collider when crouched.
        //m_damageSystem.toggleHeadTransfromCollider(!m_animationModule.isCrouched());
    }

    public void togglePrimaryWeapon()
    {
        m_characterState = m_equipmentModule.togglePrimary();
    }

    public void togglepSecondaryWeapon()
    {
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
            //m_equipmentSystem.aimCurrentEquipment(false);
            m_equipmentModule.releaseTrigger();
            //tempNavMeshAgent.enabled = false;
        }
    }

    public void lookAtTarget()
    {
        m_movmentModule.lookAtTarget();
    }

    #endregion

    #region getters and setters

    public void setOndestroyCallback(DamageModule.OnDestoryDeligate onDestoryCallback)
    {
        m_onDestoryCallback = onDestoryCallback;
    }

    public bool IsFunctional()
    {
        return m_damageModule.IsFunctional();
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
        m_damageModule.setHealth(health);
    }

    public void enableTranslateMovment(bool enable)
    {
        m_movmentModule.enableTranslateMovment(enable);
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
        if(m_equipmentModule.getCurrentWeapon() !=null)
        {
            m_equipmentModule.getCurrentWeapon().setWeaponSafty(!enadled);
        }
    }



    public bool isCrouched()
    {
        return m_animationModule.isCrouched();
    }

    #endregion

    #region Events Handlers

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
    //void OnBecameVisible()
    //{
    //    AIAgent agent = (AIAgent)m_agentController;

    //    if (agent != null)
    //    {
    //        agent.setEnabledFirint(true);
    //    }
    //}

    //void OnBecameInvisible()
    //{
    //    AIAgent agent = (AIAgent)m_agentController;

    //    if (agent != null)
    //    {
    //        agent.setEnabledFirint(false);
    //    }
    //}

    public void DodgeEnd()
    {
        if(m_animationModule.isEquiped())
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            Debug.Log("idle");
            m_characterState = CharacterMainStates.Idle;
        }
        //tempNavMeshAgent.enabled = true;
    }
    #endregion

    #region commented Code

    //public void unEquipCurentWeapon()
    //{
    //    m_characterState = m_equipmentSystem.unEquipCurrentEquipment();
    //}

    //public void equipCurrentWeapon()
    //{
    //    m_characterState = m_equipmentSystem.unEquipCurrentEquipment();
    //}

    // Getters


    /*
     * Start Shooting.
     */
    //public virtual void updateShooting()
    //{
    //    if(Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
    //    {
    //       if(m_equipmentSystem.isProperlyAimed())
    //        {
    //            m_equipmentSystem.FireCurrentWeapon();
    //        }
    //    }
    //}
    #endregion

    #region helperFunctions
    IEnumerator fireWeapon()
    {
        pullTrigger();
        yield return new WaitForSeconds(0.5f);
        releaseTrigger();
    }

    IEnumerator fireWeaponCover()
    {
        AimWeapon();
        yield return new WaitForSeconds(2f);
        AimWeapon();
        pullTrigger();
        yield return new WaitForSeconds(1f);
        releaseTrigger();
        yield return new WaitForSeconds(1f);
        StopAiming();
    }

    public string getName()
    {
        return this.name;
    }

    public Transform getTransfrom()
    {
        return this.transform;
    }

    #endregion
}
