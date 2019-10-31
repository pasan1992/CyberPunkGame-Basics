using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class PlayerController : AgentController
{
    private bool m_enabled;
    protected HumanoidMovingAgent m_movingAgent;

    private LayerMask enemyHitLayerMask;
    private LayerMask targetHitLayerMask;

    private LayerMask floorHitLayerMask;
    
    private float speedModifyVale;
    private float verticleSpeed;
    private float horizontalSpeed;
    private HealthBar m_healthBar;


    private NavMeshAgent agent;

    private Vector3 m_currentTargetPosition;

    #region Initialize
    private void Start()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
        //m_movingAgent.setFaction(m_agentFaction);
        enemyHitLayerMask = LayerMask.GetMask("Enemy");
        targetHitLayerMask = LayerMask.GetMask("Target");
        floorHitLayerMask = LayerMask.GetMask("Floor");
        m_healthBar = this.GetComponentInChildren<HealthBar>();

        createTargetPlane();
        m_movingAgent.enableTranslateMovment(true);
        intializeAgentCallbacks(m_movingAgent);

        //m_movingAgent.setOnDestoryCallback(OnAgentDestroy);
        //m_movingAgent.setOnDisableCallback(onAgentDisable);
        agent = this.GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;

        //m_movingAgent.enableTranslateMovment(false);
        MouseCurserSystem.getInstance().setTargetLineTrasforms(m_movingAgent.AgentComponents.weaponAimTransform.transform,m_movingAgent.getTargetTransfrom());
    }

    private void createTargetPlane()
    {
        // Create target support Plane.
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = this.transform;
        plane.transform.localPosition = new Vector3(0, 1.24f, 0);
        plane.transform.localScale = new Vector3(7, 7, 7);
        plane.layer = 10; // Target layer 
        plane.GetComponent<MeshFilter>().mesh = null;
        plane.name = "TargetPlane";
    }
    #endregion

    #region Updates

    private void updateNavMesgAgnet()
    {
        if(Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorHitLayerMask))
            {
                agent.isStopped = true;
                agent.SetDestination(hit.point);
                agent.isStopped = false;
            }
        }
    }

    private void controllerUpdate()
    {
        //updateNavMesgAgnet();

        verticleSpeed = Mathf.Lerp(verticleSpeed, Input.GetAxis("Vertical"),1f);
        horizontalSpeed = Mathf.Lerp(horizontalSpeed, Input.GetAxis("Horizontal"), 1f);
        //Vector3 velocity = agent.desiredVelocity;
       //velocity = velocity.normalized;

        //verticleSpeed = agent.velocity.x;
        //horizontalSpeed = agent.velocity.z;

        // if(Input.GetMouseButtonDown(1) && !m_movingAgent.isEquipingWeapon() && !m_movingAgent.isEquiped())
        // {
        //     m_movingAgent.togglePrimaryWeapon();
        // }

        // Setting Character Aiming.
        if (Input.GetMouseButton(1) && !m_movingAgent.isEquipingWeapon() && m_movingAgent.isReadyToAim())
        {
            m_movingAgent.aimWeapon();
        }
        else
        {
            m_movingAgent.stopAiming();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_movingAgent.togglepSecondaryWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_movingAgent.togglePrimaryWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_movingAgent.toggleGrenede();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            m_movingAgent.toggleHide();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_movingAgent.dodgeAttack(getDirectionRelativeToCamera( new Vector3(verticleSpeed, 0, -horizontalSpeed)));
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            speedModifyVale = Mathf.Lerp(speedModifyVale, 1.5f, 0.1f);
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(verticleSpeed * speedModifyVale, 0, -horizontalSpeed * speedModifyVale)));
            //velocity = new Vector3(velocity.x, 0, velocity.z);
            //m_movingAgent.moveCharacter(velocity);
        }
        else
        {
            speedModifyVale = Mathf.Lerp(speedModifyVale, 1f, 0.1f);
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera((new Vector3(verticleSpeed, 0, -horizontalSpeed)).normalized*speedModifyVale));
            //velocity = new Vector3(velocity.x, 0, velocity.z);
            //m_movingAgent.moveCharacter(velocity);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_movingAgent.reloadCurretnWeapon();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            m_movingAgent.pickupItem();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            summonRockets();
        }


        UpdateShooting();

        UpdateTargetPoint();
    }

    private void UpdateShooting()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
            m_movingAgent.pullTrigger();
        }

        if(Input.GetMouseButtonUp(0) && Input.GetMouseButton(1))
        {
            m_movingAgent.releaseTrigger();
        }
    }

    private void UpdateTargetPoint()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        bool found = false;

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
        {
            //targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.ONTARGET);
            targetPosition = hit.point;
          Debug.Log(hit.transform.name);
            found = true;
            MouseCurserSystem.getInstance().enableTargetLine(true);
        }

        if (!found && Physics.Raycast(castPoint, out hit, Mathf.Infinity, targetHitLayerMask))
        {
            // targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            


            if(m_movingAgent.isAimed())
            {
                MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.AIMED);
                MouseCurserSystem.getInstance().enableTargetLine(true);
            }
            else
            {
                MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.IDLE);
                MouseCurserSystem.getInstance().enableTargetLine(false);
            }

            targetPosition = hit.point;
        }



        m_movingAgent.setTargetPoint(targetPosition);
        m_currentTargetPosition = targetPosition;
    }

    private void Update()
    {
        controllerUpdate();

        if(m_healthBar)
        {
            m_healthBar.setHealthPercentage(m_movingAgent.AgentData.Health/m_movingAgent.AgentData.MaxHealth);
        }

    }

    private void FixedUpdate()
    {
      //  UpdateShooting();
      //  UpdateTargetPoint();
    }
    #endregion

    #region Commands

    private void summonRockets()
    {

        var agents =  FindObjectsOfType<FlyingAgent>();

       StartCoroutine(  waitAndFire(agents));


    }

    IEnumerator waitAndFire(FlyingAgent[] targets)
    {

        foreach(FlyingAgent agent in targets)
        {
            GameObject basicRocketObj =  ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
            basicRocketObj.transform.position = this.transform.position + new Vector3(-50,10,0);
            basicRocketObj.SetActive(true);
            BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
            rocket.fireRocket(agent.GetComponent<DamagableFlyingObject>());
            rocket.rocketScale = 0.4f; 
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    #endregion

    #region getters and setters

    public void setEnabled (bool enabled)
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
                return new Vector3(position.x, position.y, position.z);
            case "Enemy":
                return position;
        }
        return Vector3.zero;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (HumanoidMovingAgent)agent;
    }

    private Vector3 getDirectionRelativeToCamera(Vector3 direction)
    {
        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward * direction.x - right * direction.z;
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_movingAgent;
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
    }

    public override void onAgentDisable()
    {
    }

    public override void onAgentEnable()
    {
    }

    public override void resetCharacher()
    {
        throw new System.NotImplementedException();
    }

    public override void setPosition(Vector3 postion)
    {
    }

    #endregion
}
