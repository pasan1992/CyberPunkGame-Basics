using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAgent : MonoBehaviour,ICyberAgent
{
    // Start is called before the first frame update
    public float flyingHeight;
    public float speed;
    public float health = 5;

    public MovmentModule.BASIC_MOVMENT_STATE m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;

    private Vector3 m_movmentDirection;
    private GameObject m_target;
    private bool m_enableTransfromMovment;

    // Modules
    private AnimationModule m_animationModule;
    private MovmentModule m_movmentModule;
    private DamageModule m_damageModule;


    #region initalize
    void Start()
    {
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;
        m_animationModule = new AnimationModule(this.GetComponentInChildren<Animator>());
        m_movmentModule = new MovmentModule(m_target, this.transform);
        m_damageModule = new DamageModule(health);
    }
    #endregion


    #region update

    // Update is called once per frame
    void Update()
    {
        m_movmentModule.UpdateMovment((int)m_currentFlyingState, m_movmentDirection);
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
        return this.transform.position;
    }

    public bool IsFunctional()
    {
        return m_damageModule.IsFunctional();
    }

    public string getName()
    {
        return this.name;
    }

    #endregion

    #region commands

    public void DestroyCharacter()
    {
        m_damageModule.destroyCharacter();
    }

    public void enableTranslateMovment(bool enable)
    {
        m_enableTransfromMovment = enable;
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

    public void AimWeapon()
    {
        m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.AIMED_MOVMENT;
    }

    public void StopAiming()
    {
        m_currentFlyingState = MovmentModule.BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;
    }
    #endregion

    #region un-implemented functions 

    public void WeaponFireForAICover()
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
