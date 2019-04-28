using UnityEngine;

public class FlyingAgent :ICyberAgent
{
    // Start is called before the first frame update
    public float health = 5;

    public MovmentModule.BASIC_MOVMENT_STATE m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;

    private Vector3 m_movmentDirection;
    private GameObject m_target;

    // Modules
    private AnimationModule m_animationModule;
    private MovmentModule m_movmentModule;
    private DroneDamageModule m_damageModule;

    private GameObject m_selfGameObject;
    private Rigidbody m_droneRigitBody;
    public delegate void OnDestroyDeligate();
    private OnDestroyDeligate m_onDestroyCallback;
    private AgentController.AgentFaction m_group;

    #region initalize

    public FlyingAgent(Animator animator,GameObject selfGameObject,Rigidbody rigidbody,OnDestroyDeligate onDestroyCallback)
    {
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;
        m_selfGameObject = selfGameObject;

        // Initalize explosion particle system.

        m_animationModule = new AnimationModule(animator);
        m_movmentModule = new MovmentModule(m_target, selfGameObject.transform);
        m_damageModule = new DroneDamageModule(health, DestroyCharacter);
        
        m_droneRigitBody = rigidbody;
        m_onDestroyCallback += onDestroyCallback;
    }
    #endregion


    #region update

    // Update is called once per frame
    public void updateAgent()
    {
        if(m_damageModule.HealthAvailable())
        {
            m_movmentModule.UpdateMovment((int)m_currentFlyingState, m_movmentDirection);
        }
    }

    //protected void updateMovment()
    //{
    //    switch (m_currentFlyingState)
    //    {
    //        case FLYINGSTATE.AIMED:

    //            if(m_target != null)
    //            {
    //                this.transform.LookAt(m_target.transform.position);
    //            }

    //            if(m_enableTransfromMovment)
    //            {
    //                Vector3 selfTransfromMovementDirection = this.transform.InverseTransformDirection(m_movmentDirection);
    //                this.transform.Translate(selfTransfromMovementDirection);
    //            }

    //            break;
    //        case FLYINGSTATE.IDLE:

    //            if (m_movmentDirection.magnitude > 0)
    //            {
    //                Vector3 moveDirection = new Vector3(m_movmentDirection.x, 0, m_movmentDirection.z);
    //                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
    //            }

    //            if (m_enableTransfromMovment)
    //            {
    //                this.transform.Translate(Vector3.forward * m_movmentDirection.magnitude);
    //            }
    //            break;
    //    }
    //}

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
        return m_selfGameObject.transform.position;
    }

    public bool IsFunctional()
    {
        return m_damageModule.HealthAvailable();
    }

    public string getName()
    {
        return m_selfGameObject.name;
    }

    public AgentController.AgentFaction getFaction()
    {
        return m_group;
    }

    public void setFaction(AgentController.AgentFaction group)
    {
        m_group = group;
    }

    #endregion

    #region commands

    private void DestroyCharacter()
    {
        //m_damageModule.destroyDrone(m_movmentDirection);
        m_animationModule.disableAnimationSystem();
        m_droneRigitBody.isKinematic = false;
        m_droneRigitBody.useGravity = true;
        m_droneRigitBody.WakeUp();
        m_droneRigitBody.AddForce(m_movmentDirection, ForceMode.Impulse);
        m_droneRigitBody.AddTorque(Vector3.forward*200, ForceMode.Impulse);
        m_droneRigitBody.transform.parent = null;
        m_onDestroyCallback();
        m_damageModule.ExplosionEffect(m_selfGameObject.transform.position);
    }

    public void enableTranslateMovment(bool enable)
    {
        //m_enableTransfromMovment = enable;
    }

    public void damageAgent(float amount)
    {
        m_damageModule.DamageByAmount(amount);

        //if(m_damageModule.getHealth() == 0)
        //{
        //    m_damageModule.destroyDrone(m_movmentDirection);
        //}
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
        throw new System.NotImplementedException();
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

    public void weaponFireForAI()
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

    public Vector3 getTopPosition()
    {
        return Vector3.zero;
    }
    #endregion

}
