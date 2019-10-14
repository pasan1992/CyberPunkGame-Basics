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

    private AgentData m_agentData;

    private EnumUtility.MovementStatus m_currentMovmentState;

    public HumanoidMovmentModule(
        Transform transfrom, 
        HumanoidMovingAgent.CharacterMainStates characterState, 
        GameObject target, 
        HumanoidAnimationModule animationSystem,
        NavMeshAgent navMeshAgent,
        AgentData agentData
        ) : base(target, transfrom)
    {
        m_characterState = characterState;
        m_animationSystem = animationSystem;
        m_navMeshAgent = navMeshAgent;
        m_previousPosition = this.m_characterTransform.position;   
        m_agentData = agentData;     
    }

    #region Update

    private void updateMovingSpeed()
    {
        m_currentVelocity = this.m_characterTransform.position - m_previousPosition;
        m_previousPosition = this.m_characterTransform.position;
    }
    
    private void updateStamina(Vector3 movmentVelocity)
    {

        m_currentMovmentState = AnimatorConstants.getMovementStatus(movmentVelocity.magnitude,m_agentData.CurrentStamina,m_currentMovmentState);
               
        switch (m_currentMovmentState)
        {
            case EnumUtility.MovementStatus.IDLE:
            case EnumUtility.MovementStatus.WALKING:
            case EnumUtility.MovementStatus.RECOVERY:
                //Debug.Log("walking");
                if(m_agentData.CurrentStamina < m_agentData.MaxStamina) 
                {
                    m_agentData.CurrentStamina += (Time.deltaTime * m_agentData.StaminaRegenRate/2);
                }
                break;

            case EnumUtility.MovementStatus.RUNNING:
               // Debug.Log("Running");
               if(m_agentData.CurrentStamina > 0.5f)
                {
                    m_agentData.CurrentStamina -= (Time.deltaTime * m_agentData.StaminaRegenRate*2);
                }
                break;

            default:
                break;
        }
    }

    public override void UpdateMovment(int characterState, Vector3 movmentVelocity)
    {
        Debug.Log(m_agentData.CurrentStamina);
        updateMovingSpeed();
       // movmentDirection = updateStamina(movmentDirection,characterState);
        m_characterState = (HumanoidMovingAgent.CharacterMainStates)characterState;

        float crouchSpeedMultiplayer = 1;

        if (this.isCrouched())
        {
            crouchSpeedMultiplayer = 0.3f;
            movmentVelocity = movmentVelocity.normalized;
        }

        switch (m_characterState)
        {
            case HumanoidMovingAgent.CharacterMainStates.Aimed:

                //Turn player
                float angle = Vector3.Angle(getTargetDirection(), this.m_characterTransform.forward);

                if (movmentVelocity.magnitude < 0.1)
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
                Vector3 selfTransfrommoveDiection = this.m_characterTransform.InverseTransformDirection(movmentVelocity);
                m_animationSystem.setMovment(selfTransfrommoveDiection.z, selfTransfrommoveDiection.x);

                if (m_physicalLocationChange)
                {
                    // Move character transfrom
                    m_animationSystem.setRootMotionStatus(true);
                    Vector3 translateDirection = new Vector3(selfTransfrommoveDiection.x, 0, selfTransfrommoveDiection.z);
                    this.m_characterTransform.Translate(translateDirection.normalized * crouchSpeedMultiplayer / 25);
                }
                break;

            case HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed:
            case HumanoidMovingAgent.CharacterMainStates.UnArmed:

                

                if(m_agentData.CurrentStamina <= 5f || m_currentMovmentState.Equals(EnumUtility.MovementStatus.RECOVERY))
                {
                    movmentVelocity = movmentVelocity.normalized;
                }
                updateStamina(movmentVelocity);

                //Move character and turn
                if (movmentVelocity.magnitude > 0)
                {
                    Vector3 moveDirection = new Vector3(movmentVelocity.x, 0, movmentVelocity.z);
                    m_characterTransform.rotation = Quaternion.Lerp(m_characterTransform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 50f * Time.deltaTime);
                }

                m_animationSystem.setMovment(movmentVelocity.magnitude, 0);
                if(m_physicalLocationChange)
                {
                    float divider = 1;
                    if (m_characterState.Equals(HumanoidMovingAgent.CharacterMainStates.UnArmed))
                    {
                        divider = 35;
                    }
                    else
                    {
                        divider = 30;
                    }
                    m_animationSystem.setRootMotionStatus(true);
                    this.m_characterTransform.Translate(Vector3.forward * movmentVelocity.magnitude * crouchSpeedMultiplayer / divider);             
                }
               
              
             

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


