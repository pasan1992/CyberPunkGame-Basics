using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using humanoid;

[RequireComponent(typeof(MovingAgent))]

public class PlayerControllerMobile : AgentController
{
    protected MovingAgent m_selfAgent;
    public float health;
    private TargetFinder m_targetFinder;
    private bool m_crouched = false;

    #region initalize
    protected void Awake()
    {
        initalizeSelfAgent();    
        m_targetFinder = new TargetFinder(this.name, this.transform.position, GameObject.Find("TargetIndicator"));
    }

    protected void Start()
    {
        m_selfAgent.togglepSecondaryWeapon();
        m_selfAgent.setHealth(health);
        m_selfAgent.setFaction(m_agentFaction);
        m_selfAgent.setPrimayWeaponAmmoCount(10);
        m_selfAgent.setSecondaryWeaponAmmoCount(10);
    }

    protected void initalizeSelfAgent()
    {
        m_selfAgent = this.GetComponent<MovingAgent>();
        intializeAgentCallbacks(m_selfAgent);
    }

    #endregion

    #region updates

    void Update()
    {
        updateSelfAgentFromInput();
    }

    protected void updateSelfAgentFromInput()
    {
        #region get control input
        float inputHorizontal = SimpleInput.GetAxis("Horizontal");
        float inputVertical = SimpleInput.GetAxis("Vertical");

        float aimInputHorizontal = SimpleInput.GetAxis("HorizontalAim");
        float aimInputVertical = SimpleInput.GetAxis("VerticalAim");

        Vector3 aimDirection = getDirectionRelativeToCamera(new Vector3(aimInputVertical, 0, -aimInputHorizontal));

        bool runPressed = SimpleInput.GetButton("Run");
        bool crouchPressed = SimpleInput.GetButtonDown("Crouch");
        bool dodge = SimpleInput.GetButtonDown("Dodge");
        bool rifle = SimpleInput.GetButtonDown("Rifle");
        bool pistol = SimpleInput.GetButtonDown("Pistol");
        #endregion

        #region control agent from input

        #region movment control

        if(rifle)
        {
            m_selfAgent.togglePrimaryWeapon();
        }

        if(pistol)
        {
            m_selfAgent.togglepSecondaryWeapon();
        }

        if (crouchPressed)
        {
            m_selfAgent.toggleHide();
            m_crouched = !m_crouched;
        }

        if(dodge)
        {
            m_selfAgent.dodgeAttack(getDirectionRelativeToCamera((new Vector3(inputVertical, 0, -inputHorizontal).normalized) * 1.5f));
        }

        if (runPressed)
        {
            if(m_selfAgent.isCrouched())
            {
                m_selfAgent.toggleHide();
            }

            m_selfAgent.moveCharacter(getDirectionRelativeToCamera((new Vector3(inputVertical, 0, -inputHorizontal).normalized)*1.5f));
        }
        else
        {
            if(m_crouched && !m_selfAgent.isCrouched())
            {
                m_selfAgent.toggleHide();
            }
            m_selfAgent.moveCharacter(getDirectionRelativeToCamera(new Vector3(inputVertical, 0, -inputHorizontal).normalized));
        }


        #endregion

        #region aiming and fire control
        if (aimDirection.normalized.magnitude >0)
        {
            m_selfAgent.aimWeapon();
            m_targetFinder.updateTargetFinder(aimDirection, this.transform.position);
            m_selfAgent.setTargetPoint(m_targetFinder.getCalculatedTargetPosition());
            
            if(m_targetFinder.canFireAtTargetAgent())
            {
                m_selfAgent.pullTrigger();
            }
            else
            {
                m_selfAgent.releaseTrigger();
            }
        }
        else
        {
            m_selfAgent.stopAiming();
            m_selfAgent.releaseTrigger();
            m_targetFinder.disableTargetIndicator();
        }
        #endregion

       #endregion

    }
    #endregion

    #region Getters and Setters

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

    #endregion

    #region Utility
    #endregion

    #region EventHandlers
    #endregion
}
