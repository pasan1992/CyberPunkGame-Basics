using UnityEngine;
[RequireComponent(typeof(DamagableFlyingObject))]
public class FlyingAgent : MonoBehaviour ,ICyberAgent
{
    // Start is called before the first frame update
    private MovmentModule.BASIC_MOVMENT_STATE m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;

    private Vector3 m_movmentDirection;
    private GameObject m_target;

    // Modules
    private AnimationModule m_animationModule;
    private DroneMovmentModule m_movmentModule;
    public DroneDamageModule m_damageModule;

    private Rigidbody m_droneRigitBody;
    private GameEvents.BasicNotifactionEvent m_onDestroyCallback;
    private GameEvents.BasicNotifactionEvent m_onDisableCallback;
    private GameEvents.BasicNotifactionEvent m_onEnableCallback;

    public GameEnums.DroneState m_currentDroneState = GameEnums.DroneState.Flying;
    private Vector3 landPosition;
    private float flyingHeight;
    //private AgentBasicData.AgentFaction m_faction;

    private Vector3 m_beforeDisablePositionSnapShot;
    private Quaternion m_beforeDisableRotationSnapshot;
    private bool m_disabled = false;
    private bool m_recovering = false;
    private AudioSource m_audioSource;

    public AgentData m_agentData;

    public Transform landingPad;

    #region initalize

    public void Awake()
    {
        m_audioSource = this.GetComponent<AudioSource>();
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;

        // Initialize self parameters
        m_droneRigitBody = this.GetComponentInChildren<Rigidbody>();
        m_droneRigitBody.Sleep();

        m_animationModule = new AnimationModule(this.GetComponentInChildren<Animator>());
        m_movmentModule = new DroneMovmentModule(m_target, this.gameObject.transform,m_currentDroneState,m_droneRigitBody.transform,m_animationModule);

    }
    #endregion

    public void Start()
    {
        m_damageModule= new DroneDamageModule(m_agentData, this.GetComponentInChildren<Outline>(), DestroyCharacter);
        //disableDrone();
    }

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_damageModule.HealthAvailable())
        {
            m_movmentModule.UpdateMovment((int)m_currentFlyingState, m_movmentDirection,m_currentDroneState,out m_currentDroneState);
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

    public AgentBasicData.AgentFaction getFaction()
    {
        return m_agentData.m_agentFaction;
    }

    public void setFaction(AgentBasicData.AgentFaction group)
    {
        m_agentData.m_agentFaction = group;
    }

    public void setOnDestoryCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDestroyCallback = callback;
    }

    public void setOnDisableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onDisableCallback = callback;
    }

    public void setOnEnableCallback(GameEvents.BasicNotifactionEvent callback)
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
        BasicProjectile tempProjectile = Tempprojectile.GetComponent<BasicProjectile>();
        tempProjectile.speed = 1f;
        tempProjectile.setFiredFrom(m_agentData.m_agentFaction);
        tempProjectile.resetToMicroBeam();

        m_audioSource.Play();
    }

    private void DestroyCharacter()
    {    
        m_damageModule.ExplosionEffect(m_droneRigitBody.transform.position);
        m_onDestroyCallback();
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

    public void landDrone(Vector3 landPosition)
    {
         Debug.Log("here");
        if(m_currentDroneState.Equals(GameEnums.DroneState.Flying))
        {
            m_droneRigitBody.Sleep();
            m_droneRigitBody.isKinematic = true;
            m_droneRigitBody.useGravity = false;
            m_animationModule.enableAnimationSystem();
            m_currentDroneState = GameEnums.DroneState.Landing;
            m_movmentModule.setLandPosition(landPosition);
        }
    }

    [ContextMenu("Land")]
    public void landDrone()
    {
        landDrone(landingPad.transform.position);
    }

    [ContextMenu("TakeOff")]
    public void takeOff()
    {
        if(m_currentDroneState.Equals(GameEnums.DroneState.Landed))
        {
            m_currentDroneState = GameEnums.DroneState.TakeOff;
            m_droneRigitBody.Sleep();
            m_droneRigitBody.isKinematic = true;
            m_droneRigitBody.useGravity = false;
        }
    }
    #endregion

    #region un-implemented functions 

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

    public bool isReadyToAim()
    {
        throw new System.NotImplementedException();
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {

    }
    public void toggleHide()
    {

    }

    public float getSkill()
    {
        return m_agentData.Skill;
    }

    public Vector3 getCurrentVelocty()
    {
        return Vector3.zero;
    }
    #endregion

    #region Events
    private void OnDisable()
    {
        CancelInvoke();
    }

    public void resetAgent()
    {
        if(m_damageModule !=null)
        {
            m_damageModule.resetCharacter();
        }

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

    public bool isAimed()
    {
        return true;
    }

    public bool isHidden()
    {
        return false;
    }

    public GameObject getGameObject()
    {
        return this.transform.gameObject;
    }

    public Vector3 getCurrentVelocity()
    {
        throw new System.NotImplementedException();
    }

    public AgentData GetAgentData()
    {
        return m_agentData;
    }

    public bool isInteracting()
    {
        return false;
    }

    public void interactWith(Interactable interactableObj,Interactable.InteractableProperties.InteractableType type)
    {

    }
    #endregion
}
