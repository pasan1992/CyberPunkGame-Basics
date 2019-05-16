using UnityEngine;

public class FlyingAgent : MonoBehaviour ,ICyberAgent
{
    // Start is called before the first frame update
    private float health = 0;

    private MovmentModule.BASIC_MOVMENT_STATE m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;

    private Vector3 m_movmentDirection;
    private GameObject m_target;

    // Modules
    private AnimationModule m_animationModule;
    private MovmentModule m_movmentModule;
    private DroneDamageModule m_damageModule;

    private Rigidbody m_droneRigitBody;
    private AgentController.agentBasicCallbackDeligate m_onDestroyCallback;
    private AgentController.agentBasicCallbackDeligate m_onDisableCallback;
    private AgentController.agentBasicCallbackDeligate m_onEnableCallback;
    private AgentController.AgentFaction m_faction;
    private float m_skill;

    private Vector3 m_beforeDisablePositionSnapShot;
    private Quaternion m_beforeDisableRotationSnapshot;
    private bool m_disabled = false;
    private bool m_recovering = false;

    #region initalize

    public void Awake()
    {
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;

        // Initialize self parameters
        m_droneRigitBody = this.GetComponentInChildren<Rigidbody>();
        m_droneRigitBody.Sleep();

        m_animationModule = new AnimationModule(this.GetComponentInChildren<Animator>());
        m_movmentModule = new MovmentModule(m_target, this.gameObject.transform);
        m_damageModule = new DroneDamageModule(health, this.GetComponentInChildren<Outline>(), DestroyCharacter);
    }
    #endregion

    public void Start()
    {
        //Invoke("disableDrone", 3);
    }

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_damageModule.HealthAvailable())
        {
            m_movmentModule.UpdateMovment((int)m_currentFlyingState, m_movmentDirection);
        }

        updateDisabledMovment();
    }

    private void updateDisabledMovment()
    {
        if(m_disabled && m_recovering)
        {
            m_droneRigitBody.transform.position = Vector3.Lerp(m_droneRigitBody.transform.position, m_beforeDisablePositionSnapShot,0.01f);
            m_droneRigitBody.transform.rotation = Quaternion.Lerp(m_droneRigitBody.transform.rotation, m_beforeDisableRotationSnapshot, 0.01f);

            if(Vector3.Distance(m_droneRigitBody.transform.position,m_beforeDisablePositionSnapShot) < 0.1f)
            {
                enableDrone();
            }
        }
    }
    #endregion

    #region getters and setters

    public void setHealth(float value)
    {
        m_damageModule.setHealth(value);
    }

    public void setTargetPoint(Vector3 position)
    {
        m_target.transform.position = position;
    }

    public Vector3 getCurrentPosition()
    {
        return this.transform.position;
    }

    public bool IsFunctional()
    {
        return m_damageModule.HealthAvailable();
    }

    public Color getHealthColor()
    {
        return m_damageModule.getHealthColor();
    }

    public string getName()
    {
        return this.gameObject.name;
    }

    public AgentController.AgentFaction getFaction()
    {
        return m_faction;
    }

    public void setFaction(AgentController.AgentFaction group)
    {
        m_faction = group;
    }

    public void setOnDestoryCallback(AgentController.agentBasicCallbackDeligate callback)
    {
        m_onDestroyCallback = callback;
    }

    public void setOnDisableCallback(AgentController.agentBasicCallbackDeligate callback)
    {
        m_onDisableCallback = callback;
    }

    public void setOnEnableCallback(AgentController.agentBasicCallbackDeligate callback)
    {
        m_onEnableCallback = callback;
    }

    public Vector3 getTopPosition()
    {
        return m_droneRigitBody.transform.position;
    }

    public float getHealthPercentage()
    {
        return m_damageModule.getHealthPercentage();
    }


    public bool isDisabled()
    {
        return m_disabled;
    }

    #endregion

    #region commands

    public void weaponFireForAI()
    {
        //.getBasicProjectie();
        GameObject Tempprojectile = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile);
        Tempprojectile.transform.position = m_droneRigitBody.transform.position;
        Tempprojectile.transform.rotation = m_droneRigitBody.transform.rotation;
        Tempprojectile.SetActive(true);
        Tempprojectile.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);


        Tempprojectile.transform.forward = (m_target.transform.position - this.transform.position).normalized;
        ProjectileBasic tempProjectile = Tempprojectile.GetComponent<ProjectileBasic>();
        tempProjectile.speed = 1f;
        tempProjectile.setFiredFrom(m_faction);
        tempProjectile.resetToMicroBeam();
    }

    private void DestroyCharacter()
    {
        m_onDestroyCallback();
        m_damageModule.ExplosionEffect(m_droneRigitBody.transform.position);
        CancelInvoke();
    }

    public void enableTranslateMovment(bool enable)
    {
    }

    public void damageAgent(float amount)
    {
        m_damageModule.DamageByAmount(amount);
    }

    public void moveCharacter(Vector3 movmentDirection)
    {
        m_movmentDirection = movmentDirection;
        m_animationModule.setMovment(movmentDirection.x,movmentDirection.z);
    }

    public void aimWeapon()
    {
        m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.AIMED_MOVMENT;
    }

    public void stopAiming()
    {
        m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;
    }

    public void disableDrone()
    {
        m_beforeDisablePositionSnapShot = this.transform.position;
        m_beforeDisableRotationSnapshot = this.transform.rotation;
        m_droneRigitBody.WakeUp();
        m_droneRigitBody.isKinematic = false;
        m_droneRigitBody.useGravity = true;
        m_droneRigitBody.AddTorque(Random.insideUnitSphere * Random.value * 3, ForceMode.Impulse);
        m_onDisableCallback();
        m_disabled = true;
        Invoke("recover", 4);
        m_animationModule.disableAnimationSystem();
        m_damageModule.DisableDrone(m_droneRigitBody.transform.position);
    }
    
    public void enableDrone()
    {
        m_onEnableCallback();
        m_disabled = false;
        m_recovering = false;
    }

    private void recover()
    {
        m_recovering = true;
        m_droneRigitBody.Sleep();
        m_droneRigitBody.isKinematic = true;
        m_droneRigitBody.useGravity = false;
        m_animationModule.enableAnimationSystem();
    }
    #endregion

    #region un-implemented functions 

    public void weaponFireForAICover()
    {
        throw new System.NotImplementedException();
    }

    public void setWeponFireCapability(bool enadled)
    {
        throw new System.NotImplementedException();
    }


    public Transform getTransfrom()
    {
        return this.transform;
    }

    public void dodgeAttack(Vector3 dodgeDirection)
    {
        throw new System.NotImplementedException();
    }

    public void lookAtTarget()
    {
        throw new System.NotImplementedException();
    }

    public bool isEquiped()
    {
        throw new System.NotImplementedException();
    }

    public void pullTrigger()
    {
        throw new System.NotImplementedException();
    }

    public void releaseTrigger()
    {
        throw new System.NotImplementedException();
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {

    }

    public void togglepSecondaryWeapon()
    {

    }

    public void togglePrimaryWeapon()
    {

    }

    public void toggleHide()
    {

    }

    public void setSkill(float skill)
    {
        m_skill = skill;
    }

    public float getSkill()
    {
        return m_skill;
    }
    #endregion

    #region Events
    private void OnDisable()
    {
        CancelInvoke();
    }

    public void resetAgent(float health, float skill)
    {
        m_damageModule.resetCharacter(health);
        if(isDisabled())
        {
            m_droneRigitBody.transform.position = m_beforeDisablePositionSnapShot;
            m_droneRigitBody.transform.rotation = m_beforeDisableRotationSnapshot;
            recover();
            enableDrone();
        }
    }

    public Vector3 getMovmentDirection()
    {
       return m_movmentDirection;
    }
    #endregion
}
