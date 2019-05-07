using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MovingAgent))]

public class PlayerControllerMobile : AgentController
{
    // Start is called before the first frame update
    protected MovingAgent m_selfAgent;
    public float health;
    //protected SwipeComponent m_swipeComponent;

    protected Vector3 m_movePosition;
    protected bool m_enableMovment;
    protected Transform m_target = null;

    public GameObject m_aimTarget;
    private TargetFinder m_targetFinder;
    private bool targetFound;
    private Vector3 m_previousDirection;
    private float distanceFromTarget;

    #region initalize

    protected void Awake()
    {
        initalizeSelfAgent();
        //m_target = (new GameObject()).transform;      
    }

    protected void Start()
    {
        m_selfAgent.togglepSecondaryWeapon();
        m_selfAgent.setHealth(health);
    }

    protected void initalizeSelfAgent()
    {
        m_selfAgent = this.GetComponent<MovingAgent>();
    }

    #endregion

    // Update is called once per frame
    #region updates

    void Update()
    {
        moveUpdate();
        //UpdateShooting();
    }

    protected void moveUpdate()
    {
        float  inputHorizontal = SimpleInput.GetAxis("Horizontal");
        float inputVertical = SimpleInput.GetAxis("Vertical");

        float aimInputHorizontal = SimpleInput.GetAxis("HorizontalAim");
        float aimInputVertical = SimpleInput.GetAxis("VerticalAim");

        Vector3 aimDirection = getDirectionRelativeToCamera(new Vector3(aimInputVertical, 0, -aimInputHorizontal));

        if(SimpleInput.GetButton("Run"))
        {
            if(m_selfAgent.isCrouched())
            {
                m_selfAgent.toggleHide();
            }

            m_selfAgent.moveCharacter(getDirectionRelativeToCamera((new Vector3(inputVertical, 0, -inputHorizontal).normalized)*1.8f));
        }
        else
        {
            m_selfAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(inputVertical, 0, -inputHorizontal).normalized));
        }


        if(aimDirection.normalized.magnitude > 0)
        {
            Vector3 targetPosition = this.transform.position + aimDirection.normalized * 2 + new Vector3(0, 1.24f, 0);
            m_aimTarget.transform.position = targetPosition;

            ICyberAgent targetAgent = m_targetFinder.getCurrentTarget(this.transform.position,targetPosition);

            m_selfAgent.aimWeapon();
            Debug.Log(targetAgent);
            if (targetAgent == null || targetAgent.getName() == this.name)
            {
                m_selfAgent.setTargetPoint(targetPosition);
                targetFound = false;
            }
            else
            {
                float angle = Vector3.Angle((targetPosition - this.transform.position), targetAgent.getTopPosition() - this.transform.position);
                if(angle <50 & aimDirection != m_previousDirection)
                {
                    m_selfAgent.setTargetPoint(getTargetPositionFromAgent(targetAgent));
                    targetFound = true;
                    distanceFromTarget = Vector3.Distance(this.transform.position, targetAgent.getTransfrom().position);
                }
                else
                {
                    m_selfAgent.setTargetPoint(targetPosition);
                    targetFound = false;
                }
            }
        }
        else
        {
            m_selfAgent.stopAiming();
        }

        if(aimDirection.magnitude > 0.5 && targetFound && distanceFromTarget < 15)
        {
            m_previousDirection = aimDirection;
            m_selfAgent.pullTrigger();
        }
        else
        {
            m_selfAgent.releaseTrigger();
        }

        if (Input.GetKeyDown(KeyCode.C) || SimpleInput.GetButtonDown("Jump"))
        {
            m_selfAgent.toggleHide();
        }


    }

    protected void UpdateShooting()
    {
        if(m_target != null)
        {
            m_selfAgent.aimWeapon();
            m_selfAgent.setTargetPoint(m_target.position);
        }
    }
    #endregion




    #region eventHandlers

    public void onTapObject(Transform tapObject)
    {

        if(tapObject.tag == "Player")
        {
            Debug.Log(tapObject.tag + " and " + tapObject.name);
            m_target = tapObject;
            m_selfAgent.aimWeapon();
            m_selfAgent.togglePrimaryWeapon();
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

    public override void setMovableAgent(ICyberAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_selfAgent;
    }

    public void setTargetFinder(TargetFinder targetFinder)
    {
        m_targetFinder = targetFinder;
    }

    private Vector3 getTargetPositionFromAgent(ICyberAgent agent)
    {
        MovingAgent humanoidAgent = agent as MovingAgent;

        if(humanoidAgent == null)
        {
            return agent.getTopPosition();
        }
        else
        {
            if(humanoidAgent.isCrouched())
            {
                if(humanoidAgent.isAimed())
                {
                    return agent.getTopPosition();
                }
                else
                {
                    return agent.getCurrentPosition() + new Vector3(0,0.6f,0);
                }
            }
            else
            {
                return agent.getCurrentPosition() + new Vector3(0, 1.2f, 0);
            }
        }
    }

    #endregion
}
