using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovingAgent))]
public class PlayerAgent : MonoBehaviour,AgentController
{
    private bool m_enabled;
    protected MovingAgent m_movingAgent;

    public LayerMask enemyHitLayerMask;
    public LayerMask floorHitLayerMask;
    public float health;

    #region Initialize
    private void Awake()
    {
        m_movingAgent = this.GetComponent<MovingAgent>();

    }

    private void Start()
    {
        createTargetPlane();
        m_movingAgent.setHealth(health);
    }

    private void createTargetPlane()
    {
        // Create target support Plane.
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = this.transform;
        plane.transform.localPosition = new Vector3(0, 1.24f, 0);
        plane.transform.localScale = new Vector3(4, 4, 4);
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
        // Setting Character Aiming.
        if (Input.GetMouseButton(1) && !m_movingAgent.isEquipingWeapon())
        {
            m_movingAgent.AimWeapon();
        }
        else
        {
            m_movingAgent.StopAiming();
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
            m_movingAgent.dodgeAttack(getDirectionRelativeToCamera( new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"))));
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(Input.GetAxis("Vertical") * 2, 0, -Input.GetAxis("Horizontal") * 2)));
        }
        else
        {
            m_movingAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"))));
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

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
        {
            // targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            targetPosition = hit.point;
        }

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
        {
            //targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            targetPosition = hit.point;
        }

        m_movingAgent.setTargetPoint(targetPosition);
    }

    private void Update()
    {
        controllerUpdate();
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

    public void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (MovingAgent)agent;
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
    #endregion
}
