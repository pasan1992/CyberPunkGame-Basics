using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAgent : MonoBehaviour,ICyberAgent
{
    // Start is called before the first frame update
    public float flyingHeight;
    public float speed;
    public float health = 5;

    public enum FLYINGSTATE { AIMED,IDLE}
    public FLYINGSTATE m_currentFlyingState = FLYINGSTATE.IDLE;

    private Vector3 m_movmentDirection;
    private GameObject m_target;
    private bool m_enableTransfromMovment;



    #region initalize
    void Start()
    {
        m_target = new GameObject();
        m_target.transform.position = Vector3.zero;
    }
    #endregion


    #region update

    // Update is called once per frame
    void Update()
    {
        updateMovment();
    }

    protected void updateMovment()
    {
        switch (m_currentFlyingState)
        {
            case FLYINGSTATE.AIMED:

                if(m_target != null)
                {
                    this.transform.LookAt(m_target.transform.position);
                }

                if(m_enableTransfromMovment)
                {
                    Vector3 selfTransfromMovementDirection = this.transform.InverseTransformDirection(m_movmentDirection);
                    this.transform.Translate(selfTransfromMovementDirection);
                }

                break;
            case FLYINGSTATE.IDLE:

                if (m_movmentDirection.magnitude > 0)
                {
                    Vector3 moveDirection = new Vector3(m_movmentDirection.x, 0, m_movmentDirection.z);
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
                }

                if (m_enableTransfromMovment)
                {
                    this.transform.Translate(Vector3.forward * m_movmentDirection.magnitude);
                }
                break;
        }
    }

    #endregion

    #region getters and setters

    public void setTargetPoint(Vector3 position)
    {
        m_target.transform.position = position;
    }

    public string getNamge()
    {
        return this.name;
    }

    #endregion

    #region commands

    public void DestroyCharacter()
    {

    }

    public void enableTranslateMovment(bool enable)
    {
        m_enableTransfromMovment = enable;
    }

    public void damageAgent(float amount)
    {
        health -= amount;
        if(health <0)
        {
            health = 0;
        }
    }

    public void moveCharacter(Vector3 movmentDirection)
    {
        m_movmentDirection = movmentDirection;
    }

    public void AimWeapon()
    {
       m_currentFlyingState = FLYINGSTATE.AIMED; 
    }

    public void StopAiming()
    {
        m_currentFlyingState = FLYINGSTATE.IDLE;
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {

    }

    public bool IsFunctional()
    {
        return health > 0;
    }

    #endregion

}
