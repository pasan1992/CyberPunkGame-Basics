using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using humanoid;

[RequireComponent(typeof(MovingAgent))]
public class AIController : AgentController
{
    private bool m_enabled;
    protected MovingAgent m_movingAgent;
    protected NavMeshAgent m_navMeshAgent;
    public bool enableFire;
    public Weapon.WEAPONTYPE selectedWeaponType;
    public float health;
    public string enemyTag;
    // temp
    private MovingAgent enemy;
    private float moveCounter;
    private float shootingCounter =2f;
    private Vector3 moveDirection;
    private bool inScreenLimit = false;
    private bool triggerPulled;


    // temp values, need to remove later
    private float tempFloat;
    private float tempFloat2;

    #region Initalize
    public void Awake()
    {
        m_movingAgent = this.GetComponent<MovingAgent>();
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        GameObject[] playerTaggedObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject obj in playerTaggedObjects)
        {
            if(obj !=this.gameObject)
            {
                enemy = obj.GetComponent<MovingAgent>();

                if (enemy != null)
                {
                    break;
                }
            }

        }
        tempFloat = Random.value * 2 + Random.value * 2;
        tempFloat2 = Random.value * 2 + Random.value * 2;
    }

    public void Start()
    {
        m_movingAgent.setHealth(health);
        m_movingAgent.enableTranslateMovment(false);
        intializeAgentCallbacks(m_movingAgent);
    }
    #endregion

    #region Updates

    private void updateMove()
    {
        m_navMeshAgent.SetDestination(enemy.transform.position + new Vector3(tempFloat, 0, tempFloat2));
        m_navMeshAgent.updateRotation = false;

        if (!m_navMeshAgent.pathPending)
        {
            Vector3 velocity = m_navMeshAgent.desiredVelocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_movingAgent.moveCharacter(velocity);
        }
    }

    private void Update()
    {
        controllerUpdate();

        if(m_movingAgent.isCharacterEnabled())
        {
            updateMove();
        }
        else
        {
            m_navMeshAgent.isStopped = true;
        }


       // if(m_navMeshAgent.se)
       //m_navMeshAgent.Move(m_navMeshAgent.velocity);
    }

    private void controllerUpdate()
    {
        if(!m_movingAgent.isEquiped())
        {
            switch (selectedWeaponType)
            {
                case Weapon.WEAPONTYPE.primary:
                    m_movingAgent.togglePrimaryWeapon();
                    break;
                case Weapon.WEAPONTYPE.secondary:
                    m_movingAgent.togglepSecondaryWeapon();
                    break;
            }

        }
        else
        {
            if (!m_movingAgent.isEquipingWeapon())
            {
                m_movingAgent.aimWeapon();
            }
            else
            {
                m_movingAgent.stopAiming();
            }
        }

        if (moveCounter > 0.2f)
        {
            moveDirection = Random.insideUnitSphere;
            moveDirection.y = 0;
            moveCounter = 0;
        }

        Vector3 targetPostion = enemy.transform.position;
        targetPostion.y = 1.5f;
        m_movingAgent.setTargetPoint(targetPostion);

        if(isInScreenLimit() && enableFire)
        {
            //if (shootingCounter > 1)
            //{
            //    targetPostion = player.transform.position;
            //    targetPostion = new Vector3(targetPostion.x, 1.2f + targetPostion.y, targetPostion.z);
            //    m_movingAgent.setTargetPoint(targetPostion);
            //    m_movingAgent.weaponFireForAI();
            //    shootingCounter = -Random.value * 3;
            //}

            //shootingCounter += Time.deltaTime * 2;

            if(!triggerPulled)
            {
                if (shootingCounter > 0.5f)
                {
                    m_movingAgent.pullTrigger();
                    triggerPulled = true;
                    shootingCounter = 0;

                }
            }
            else
            {
                if (shootingCounter > 0.5f)
                {
                    m_movingAgent.releaseTrigger();
                    triggerPulled = false;
                    shootingCounter = 0;
                    tempFloat = Random.value * 10 + 1;
                }
            }


        }
        shootingCounter += Time.deltaTime;


        moveCounter += Time.deltaTime ;

       // m_movingAgent.moveCharacter(moveDirection.normalized);


    }
    
    private void updateShootingAction()
    {

    }

    #endregion

    #region commands

    private void selectWeapon()
    {
        if (!m_movingAgent.isEquiped())
        {
            switch (selectedWeaponType)
            {
                case Weapon.WEAPONTYPE.primary:
                    m_movingAgent.togglePrimaryWeapon();
                    break;
                case Weapon.WEAPONTYPE.secondary:
                    m_movingAgent.togglepSecondaryWeapon();
                    break;
            }
        }
    }

    #endregion

    #region getters and setters

    public void setEnabled(bool enabled)
    {
        m_enabled = enabled;
    }

    public bool getEnabled()
    {
        return m_enabled;
    }

    // Set target Height depending on the target type.
    private Vector3 setTargetHeight(Vector3 position, string tag)
    {
        switch (tag)
        {
            case "Floor":
                return new Vector3(position.x, 1.25f, position.z);
            case "Enemy":
                return position;
        }
        return Vector3.zero;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (MovingAgent)agent;
    }

    private bool playerisNear()
    {
        return Vector3.Distance(m_movingAgent.transform.position,enemy.transform.position) < 5;
    }

    public void setInScreenLimit(bool enabled)
    {
        inScreenLimit = enabled;
    }

    public bool isInScreenLimit()
    {
        return inScreenLimit;
    }
    #endregion

    #region Events

    void OnBecameVisible()
    {
        setInScreenLimit(true);
    }

    void OnBecameInvisible()
    {
        setInScreenLimit(false);
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        throw new System.NotImplementedException();
    }

    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentEnable()
    {
    }

    public override void resetCharacher()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
