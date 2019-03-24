using UnityEngine;

public class AgentMovmentSystem
{
    // Start is called before the first frame update

    protected GameObject m_target;
    protected MovingAgent.CharacterMainStates m_characterState;
    protected Transform m_characterTransform;
    protected AgentAnimationSystem m_animationSystem;
    //protected CharacterController m_characterController;
    protected bool m_enableTranslateMovment = true;

    public AgentMovmentSystem(Transform transfrom, MovingAgent.CharacterMainStates characterState, GameObject target,AgentAnimationSystem animationSystem)
    {
        m_characterTransform = transfrom;
        m_characterState = characterState;
        m_target = target;
        m_animationSystem = animationSystem;
        //m_characterController = characterController;
    }

    #region Update
    public void UpdateMovmentSystem(MovingAgent.CharacterMainStates characterState,Vector3 movmentDirection)
    {
        m_characterState = characterState;
        switch (m_characterState)
        {
            case MovingAgent.CharacterMainStates.Aimed:

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

                if(m_enableTranslateMovment)
                {
                    // Move character transfrom
                    Vector3 translateDirection = new Vector3(selfTransfrommoveDiection.x, 0, selfTransfrommoveDiection.z);
                    this.m_characterTransform.Translate(translateDirection.normalized / 50);
                }


                //Vector3 translateDirection = new Vector3(movmentDirection.x, 0, movmentDirection.z);

                //if (m_characterController.enabled)
                //{
                //    m_characterController.Move(translateDirection.normalized / 15);
                //}
                break;

            case MovingAgent.CharacterMainStates.Armed_not_Aimed:
            case MovingAgent.CharacterMainStates.Idle:

                //Move character and turn
                if (movmentDirection.magnitude > 0)
                {
                    Vector3 moveDirection = new Vector3(movmentDirection.x, 0, movmentDirection.z);
                    m_characterTransform.rotation = Quaternion.Lerp(m_characterTransform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), 5f * Time.deltaTime);
                }

                m_animationSystem.setMovment(movmentDirection.magnitude, 0);

                float divider = 1;
                if (m_characterState.Equals(MovingAgent.CharacterMainStates.Idle))
                {
                    divider = 20;
                }
                else
                {
                    divider = 15;
                }

                if(m_enableTranslateMovment)
                {
                    this.m_characterTransform.Translate(Vector3.forward * movmentDirection.magnitude / divider);
                }


                //Vector3 newDirection = m_characterTransform.TransformDirection(Vector3.forward);
                //if(m_characterController.enabled)
                //{
                //    m_characterController.Move(newDirection * movmentDirection.magnitude / divider);
                //}

                break;
        }
    }
    #endregion

    #region getters&setters
    /*
     * Current Target direction from the player
     */
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

    //private Vector3 getDirectionRelativeToCamera(Vector3 direction)
    //{
    //    var camera = Camera.main;

    //    //camera forward and right vectors:
    //    var forward = camera.transform.forward;
    //    var right = camera.transform.right;

    //    //project forward and right vectors on the horizontal plane (y = 0)
    //    forward.y = 0f;
    //    right.y = 0f;
    //    forward.Normalize();
    //    right.Normalize();

    //    return forward * direction.x - right * direction.z;
    //}

    public void enableTranslateMovment(bool enable)
    {
        m_enableTranslateMovment = enable;
    }
    #endregion

    #region commands

    public void dodge(Vector3 dodgeDirection)
    {
        //dodgeDirection = this.m_characterTransform.InverseTransformDirection(dodgeDirection);
        Vector3 convertedDodgeDirection = new Vector3(dodgeDirection.x, 0, dodgeDirection.z);
        m_characterTransform.LookAt(m_characterTransform.position + convertedDodgeDirection, Vector3.up);
    }

    #endregion
}
