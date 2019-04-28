using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MovingAgent))]
//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SwipeComponent))]

public class PlayerControllerMobile : MonoBehaviour
{
    // Start is called before the first frame update
    protected MovingAgent m_movingAgent;
    //protected NavMeshAgent m_navMeshAgent;
    protected SwipeComponent m_swipeComponent;

    protected Vector3 m_movePosition;
    protected bool m_enableMovment;
    protected Transform m_target = null;

    public GameObject target;

    #region initalize

    protected void Awake()
    {
        initalizeNavMesh();
    }

    protected void Start()
    {
        m_swipeComponent.setGetTapObjectFunction(onTapObject, onTapPosition);
        m_movingAgent.togglepSecondaryWeapon();
    }

    protected void initalizeNavMesh()
    {
        //m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_movingAgent = this.GetComponent<MovingAgent>();
        m_swipeComponent = this.GetComponent<SwipeComponent>();
    }

    #endregion

    // Update is called once per frame
    #region updates

    void Update()
    {
        moveUpdate();
        UpdateShooting();
    }

    protected void moveUpdate()
    {
        //if(m_enableMovment)
        //{
        //    m_navMeshAgent.SetDestination(m_movePosition);
        //    m_navMeshAgent.updateRotation = false;

        //    if (!m_navMeshAgent.pathPending)
        //    {
        //        Vector3 velocity = m_navMeshAgent.desiredVelocity.normalized;
        //        velocity = new Vector3(velocity.x, 0, velocity.z);
        //        m_movingAgent.moveCharacter(velocity);
        //    }
        //}

        float  inputHorizontal = SimpleInput.GetAxis("Horizontal");
        float inputVertical = SimpleInput.GetAxis("Vertical");

        float aimInputHorizontal = SimpleInput.GetAxis("HorizontalAim");
        float aimInputVertical = SimpleInput.GetAxis("VerticalAim");

        Vector3 aimDirection = getDirectionRelativeToCamera(new Vector3(aimInputVertical, 0, -aimInputHorizontal));
        m_movingAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(inputVertical, 0, -inputHorizontal).normalized));

        if(aimDirection.normalized.magnitude > 0)
        {
            Vector3 targetPosition = this.transform.position + aimDirection.normalized * 2 + new Vector3(0, 1.24f, 0);
            target.transform.position = targetPosition;

            m_movingAgent.aimWeapon();
            m_movingAgent.setTargetPoint(targetPosition);
        }
        else
        {
            m_movingAgent.stopAiming();
        }

        if(aimDirection.magnitude > 0.5)
        {
            m_movingAgent.pullTrigger();
        }
    }

    protected void UpdateShooting()
    {
        if(m_target != null)
        {
            m_movingAgent.aimWeapon();
            Debug.Log("target");
            m_movingAgent.setTargetPoint(m_target.position);
        }
        else
        {
           //
        }
    }
    #endregion




    #region eventHandlers

    public void onTapPosition(Vector3 position)
    {
        //m_enableMovment = true;
        //m_movePosition = position;
    }

    public void onTapObject(Transform tapObject)
    {

        if(tapObject.tag == "Player")
        {
            Debug.Log(tapObject.tag + " and " + tapObject.name);
            m_target = tapObject;
            m_movingAgent.aimWeapon();
            m_movingAgent.togglePrimaryWeapon();
        }
    }

    #endregion

    #region Utility

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
