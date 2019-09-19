﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class PlayerController : AgentController
{
    private bool m_enabled;
    protected HumanoidMovingAgent m_movingAgent;

    private LayerMask enemyHitLayerMask;
    private LayerMask floorHitLayerMask;
    private float speedModifyVale;
    private float verticleSpeed;
    private float horizontalSpeed;
    private HealthBar m_healthBar;


    private NavMeshAgent agent;

    #region Initialize
    private void Start()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
        m_movingAgent.setFaction(m_agentFaction);
        enemyHitLayerMask = LayerMask.GetMask("Enemy");
        floorHitLayerMask = LayerMask.GetMask("Target");
        m_healthBar = this.GetComponentInChildren<HealthBar>();

        createTargetPlane();
        m_movingAgent.enableTranslateMovment(true);
        intializeAgentCallbacks(m_movingAgent);

        //m_movingAgent.setOnDestoryCallback(OnAgentDestroy);
        //m_movingAgent.setOnDisableCallback(onAgentDisable);
        agent = this.GetComponent<NavMeshAgent>();
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

    //public PlayerAgent(LayerMask enemyHitLayerMask, LayerMask floorHitLayerMask)
    //{
    //    this.enemyHitLayerMask = enemyHitLayerMask;
    //    this.floorHitLayerMask = floorHitLayerMask;
    //}


    #region Updates

    private void controllerUpdate()
    {
        verticleSpeed = Mathf.Lerp(verticleSpeed, Input.GetAxis("Vertical"),1);
        horizontalSpeed = Mathf.Lerp(horizontalSpeed, Input.GetAxis("Horizontal"), 1f);

        // Setting Character Aiming.
        if (Input.GetMouseButton(1) && !m_movingAgent.isEquipingWeapon())
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
            speedModifyVale = Mathf.Lerp(speedModifyVale, 2, 0.1f);
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(verticleSpeed * speedModifyVale, 0, -horizontalSpeed * speedModifyVale)));
        }
        else
        {
            speedModifyVale = Mathf.Lerp(speedModifyVale, 1f, 0.1f);
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera((new Vector3(verticleSpeed, 0, -horizontalSpeed)).normalized*speedModifyVale));
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_movingAgent.reloadCurretnWeapon();
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
            found = true;
        }

        if (!found && Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
        {
            // targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            


            if(m_movingAgent.isAimed())
            {
                MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.AIMED);
            }
            else
            {
                MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.IDLE);
            }

            targetPosition = hit.point;
        }



        m_movingAgent.setTargetPoint(targetPosition);
    }

    private void Update()
    {
        controllerUpdate();

        if(m_healthBar)
        {
            m_healthBar.setHealthPercentage(m_movingAgent.AgentData.Health/m_movingAgent.AgentData.MaxHealth);
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
