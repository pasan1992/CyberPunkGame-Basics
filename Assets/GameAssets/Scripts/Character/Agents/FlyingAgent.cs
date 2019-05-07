using UnityEngine;

public class FlyingAgent : MonoBehaviour ,ICyberAgent
{
    // Start is called before the first frame update
    public float health = 5;

    private MovmentModule.BASIC_MOVMENT_STATE m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;

    private Vector3 m_movmentDirection;
    private GameObject m_target;

    // Modules
    private AnimationModule m_animationModule;
    private MovmentModule m_movmentModule;
    private DroneDamageModule m_damageModule;

    private Rigidbody m_droneRigitBody;
    public delegate void OnDestroyDeligate();
    private OnDestroyDeligate m_onDestroyCallback;
    private AgentController.AgentFaction m_faction;
    private float m_skill;

    #region initalize

    public void Awake()
    {
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;

        // Initialize self parameters
        m_droneRigitBody = this.GetComponentInChildren<Rigidbody>();

        m_animationModule = new AnimationModule(this.GetComponentInChildren<Animator>());
        m_movmentModule = new MovmentModule(m_target, this.gameObject.transform);
        m_damageModule = new DroneDamageModule(health, this.GetComponentInChildren<Outline>(), DestroyCharacter);
    }
    #endregion


    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_damageModule.HealthAvailable())
        {
            m_movmentModule.UpdateMovment((int)m_currentFlyingState, m_movmentDirection);
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
        return this.gameObject.transform.position;
    }

    public bool IsFunctional()
    {
        return m_damageModule.HealthAvailable();
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

    public void setonDestoryCallback(OnDestroyDeligate callback)
    {
        m_onDestroyCallback = callback;
    }

    public Vector3 getTopPosition()
    {
        return this.transform.position;
    }

    #endregion

    #region commands

    public void weaponFireForAI()
    {
        GameObject Tempprojectile = ProjectilePool.getInstance().getBasicProjectie();
        Tempprojectile.transform.position = this.transform.position;
        Tempprojectile.transform.rotation = this.transform.rotation;
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
        m_damageModule.ExplosionEffect(this.transform.position);
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

}
