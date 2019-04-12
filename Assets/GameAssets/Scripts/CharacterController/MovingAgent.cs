using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MovingAgent : MonoBehaviour,ICyberAgent
{
    #region parameters
    // Public - Editor Values
    public enum CharacterMainStates { Aimed, Armed_not_Aimed,Dodge, Idle }
    //public LayerMask floorHitLayerMask;
   // public LayerMask enemyHitLayerMask;
   // public bool isPlayer = false;

    // Attribute - Systems
    protected EquipmentSystem m_equipmentSystem;
    protected AgentAnimationSystem m_animationSystem;
    protected AgentMovmentSystem m_movmentSystem;
    protected DamageSystem m_damageSystem;
    //protected AgentController m_agentController;
    private Vector3 m_movmentVector;
   
    // Attributes
    public CharacterMainStates m_characterState = CharacterMainStates.Idle;
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
        m_animationSystem = new AgentAnimationSystem(this.GetComponent<Animator>(), this.GetComponent<AimIK>(), 10);

        // Create equipment system.
        Weapon[] m_currentWeapons = this.GetComponentsInChildren<Weapon>();
        WeaponProp[] m_currentWeaponProps = this.GetComponentsInChildren<WeaponProp>();
        m_equipmentSystem = new EquipmentSystem(m_currentWeapons, m_currentWeaponProps, this.transform.name, m_characterState, m_target,GetComponent<Recoil>(),m_animationSystem);


        // Create movment system.
        m_movmentSystem = new AgentMovmentSystem(this.transform,m_characterState,m_target,m_animationSystem);

        m_damageSystem = new DamageSystem(5, this.GetComponent<RagdollUtility>(), this.GetComponentInChildren<HitReaction>());
        
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
    }
    #endregion

    #region updates
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_characterEnabled)
        {
            // Update Systems.
            m_animationSystem.UpdateAnimationState(m_characterState);
            m_movmentSystem.UpdateMovmentSystem(m_characterState, m_movmentVector);
            m_equipmentSystem.UpdateSystem(m_characterState);

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
        m_damageSystem.DamageByAmount(amount);
    }

    public virtual void pullTrigger()
    {
        if (m_equipmentSystem.isProperlyAimed() && m_characterState.Equals(CharacterMainStates.Aimed))
        {
            m_equipmentSystem.pullTrigger();
        }
    }

    public virtual void releaseTrigger()
    {
        m_equipmentSystem.releaseTrigger();
    }

    public virtual void weaponFireForAI()
    {
        StartCoroutine(fireWeapon());
    }

    // Aim Current Weapon -
    public virtual void AimWeapon()
    {
        if (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) && !isEquipingWeapon() && !m_characterState.Equals(CharacterMainStates.Dodge))
        {
            m_characterState = CharacterMainStates.Aimed;
            m_equipmentSystem.getCurrentWeapon().setAimed(true);
        }
    }

    // Stop Aiming current Weapon.
    public virtual void StopAiming()
    {
        if (m_characterState.Equals(CharacterMainStates.Aimed))
        {
             m_characterState = CharacterMainStates.Armed_not_Aimed;
            m_equipmentSystem.getCurrentWeapon().setAimed(false);
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
        m_equipmentSystem.DropCurrentWeapon();
        m_characterEnabled = false;
        m_damageSystem.destroyCharacter();
        m_animationSystem.disableAnimationSystem();
    }

    public void toggleHide()
    {
        m_animationSystem.toggleCrouched();
    }

    public void togglePrimaryWeapon()
    {
        m_characterState = m_equipmentSystem.togglePrimary();
    }

    public void togglepSecondaryWeapon()
    {
        m_characterState = m_equipmentSystem.toggleSecondary();
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        m_damageSystem.reactOnHit(collider, force, point);
    }

    public void dodgeAttack(Vector3 dodgeDirection)
    {
        if (!m_characterState.Equals(CharacterMainStates.Dodge))
        {
            m_characterState = CharacterMainStates.Dodge;
            m_animationSystem.triggerDodge();
            m_movmentSystem.dodge(dodgeDirection);
            //m_equipmentSystem.aimCurrentEquipment(false);
            m_equipmentSystem.releaseTrigger();
        }
    }

    #endregion

    #region getters and setters

    public bool IsFunctional()
    {
        return m_damageSystem.IsFunctional();
    }

    public string getNamge()
    {
        return this.name;
    }

    public bool isEquiped()
    {
        return m_animationSystem.isEquiped() && (m_characterState.Equals(CharacterMainStates.Armed_not_Aimed) || m_characterState.Equals(CharacterMainStates.Aimed));
    }

    public void setTargetPoint(Vector3 position)
    {
        m_target.transform.position = position;
    }

    public bool isEquipingWeapon()
    {
        return m_equipmentSystem.isInEquipingAction();
    }

    public void setHealth(float health)
    {
        m_damageSystem.setHealth(health);
    }

    public void enableTranslateMovment(bool enable)
    {
        m_movmentSystem.enableTranslateMovment(enable);
    }

    public Vector3 getCurrentPosition()
    {
        return this.transform.position;
    }
    #endregion

    #region Events Handlers

    public void EquipAnimationEvent()
    {
        m_equipmentSystem.EquipAnimationEvent();
    }

    public void UnEquipAnimationEvent()
    {
        m_equipmentSystem.UnEquipAnimationEvent();
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

        if(m_animationSystem.isEquiped())
        {
            m_characterState = CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            Debug.Log("idle");
            m_characterState = CharacterMainStates.Idle;
        }
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
    #endregion
}
