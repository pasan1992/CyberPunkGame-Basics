using UnityEngine;
using UnityEngine.AI;

public class HumanoidMovmentModule : MovmentModule
{
    // Start is called before the first frame update
    protected HumanoidMovingAgent.CharacterMainStates m_characterState;
    protected HumanoidAnimationModule m_animationSystem;
    protected bool m_physicalLocationChange = true;
    private NavMeshAgent m_navMeshAgent;

    // Calulate moving speed;
    private Vector3 m_previousPosition;
    private Vector3 m_currentVelocity;

    public HumanoidMovmentModule(
        Transform transfrom, 
        HumanoidMovingAgent.CharacterMainStates characterState, 
        GameObject target, 
        HumanoidAnimationModule animationSystem,
        NavMeshAgent navMeshAgent
        ) : base(target, transfrom)
    {
        m_characterState = characterState;
        m_animationSystem = animationSystem;
        m_navMeshAgent = navMeshAgent;
        m_previousPosition = this.m_characterTransform.position;
    }

    #region Update

    private void updateMovingSpeed()
    {
        m_currentVelocity = this.m_characterTransform.position - m_previousPosition;
        m_previousPosition = this.m_characterTransform.position;
    }
    public override void UpdateMovment(int characterState, Vector3 movmentDirection)
    {
        updateMovingSpeed();
        m_characterState = (HumanoidMovingAgent.CharacterMainStates)characterState;

        float crouchSpeedMultiplayer = 1;

        if (this.isCrouched())
        {
            crouchSpeedMultiplayer = 0;
            movmentDirection = movmentDirection.normalized;
        }

        switch (m_characterState)
        {
            case HumanoidMovingAgent.CharacterMainStates.Aimed:

                //Turn player
                float angle = Vector3.Angle(getTargetDirection(), this.m_characterTransform.forward);

                if (movmentDirection.magnitude < 0.1)
                {
                    if (Mathf.Abs(angle) > 90)
                    {
                        m_characterTransform.LookAt(getTurnPoint(), Vector3.up);
                    }

                }
                else
                {
                    m_characterTransform.LookAt(getTurnPoint(), Vector3.up);
                }

                // Move Character animator
                Vector3 selfTransfrommoveDiection = this.m_characterTransform.InverseTransformDirection(movmentDirection);
                m_animationSystem.setMovment(selfTransfrommoveDiection.z, selfTransfrommoveDiection.x);

                if (m_physicalLocationChange)
                {
                    // Move character transfrom
                    m_animationSystem.setRootMotionStatus(true);
                    Vector3 translateDirection = new Vector3(selfTransfrommoveDiection.x, 0, selfTransfrommoveDiection.z);
                    this.m_characterTransform.Translate(translateDirection.normalized * crouchSpeedMultiplayer / 30);
                }
                break;

            case HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed:
            case HumanoidMovingAgent.CharacterMainStates.Idle:

                //Move character and turn
                if (movmentDirection.magnitude > 0)
                {
                    Vector3 moveDirection = new Vector3(movmentDirection.x, 0, movmentDirection.z);
                    m_characterTransform.rotation = Quaternion.Lerp(m_characterTransform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 50f * Time.deltaTime);
                }

                m_animationSystem.setMovment(movmentDirection.magnitude, 0);




                if(m_physicalLocationChange)
                {
                    float divider = 1;
                    if (m_characterState.Equals(HumanoidMovingAgent.CharacterMainStates.Idle))
                    {
                        divider = 100;
                    }
                    else
                    {
                        divider = 100;
                    }
                    m_animationSystem.setRootMotionStatus(true);
                    this.m_characterTransform.Translate(Vector3.forward * movmentDirection.magnitude * crouchSpeedMultiplayer / divider);             
                }
                // if (m_physicalLocationChange)
                // {
                //     this.m_characterTransform.Translate(Vector3.forward * movmentDirection.magnitude * crouchSpeedMultiplayer / divider);
                // }
                break;
            default:
                // No neet of movment in other states.
                // Dodge roll, interaction
                break;
        }
    }
    #endregion

    #region getters & setters
    /*
        * Current Target direction from the player
        */

    public Vector3 getCurrentVelocity()
    {
        if(m_physicalLocationChange)
        {
            return m_currentVelocity;
        }
        else
        {
            return m_navMeshAgent.velocity;
        } 
    }
    private Vector3 getTargetDirection()
    {
        return (m_target.transform.position - m_characterTransform.position).normalized;
    }

    private Vector3 getTurnPoint()
    {
        Vector3 position = m_target.transform.position;
        position.y = m_characterTransform.position.y;
        return position;
    }

    public void setPhysicalLocationChange(bool enable)
    {
        m_physicalLocationChange = enable;
    }

    public bool isCrouched()
    {
        return m_animationSystem.isCrouched();
    }
    #endregion

    #region commands

    public void dodge(Vector3 dodgeDirection)
    {
        //dodgeDirection = this.m_characterTransform.InverseTransformDirection(dodgeDirection);
        Vector3 convertedDodgeDirection = new Vector3(dodgeDirection.x, 0, dodgeDirection.z);
        m_characterTransform.LookAt(m_characterTransform.position + convertedDodgeDirection, Vector3.up);
    }

    public void lookAtTarget()
    {
        m_characterTransform.LookAt(getTurnPoint(), Vector3.up);
    }

    public void LookAtObject(Vector3 position)
    {
        m_characterTransform.LookAt(position,Vector3.up);
    }

    #endregion
}


