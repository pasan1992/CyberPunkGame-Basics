using UnityEngine;

public class AnimationModule
{
    // Start is called before the first frame update
    protected Animator m_animator;
    protected bool rootMotionEnabled;
    
    public AnimationModule(Animator animator)
    {
        m_animator = animator;
    }

    public virtual void disableAnimationSystem()
    {
        m_animator.enabled = false;
    }

    public virtual void enableAnimationSystem()
    {
        m_animator.enabled = true;
    }

    public virtual void setMovment(float forward, float side)
    {
        m_animator.SetFloat("forward", forward);
        m_animator.SetFloat("side", side);
    }

    public void setAnimationSpeed(float speed)
    {
        m_animator.SetFloat("movmenetSpeed",speed);
    }

    public virtual void setRootMotionStatus(bool status)
    {
        if(rootMotionEnabled != status)
        {
            rootMotionEnabled = status;
            m_animator.applyRootMotion =status;
        }
    }

    public virtual void setInteraction(bool enabled,int interactionId)
    {
        m_animator.SetInteger("interactionID",interactionId);
        m_animator.SetBool("timedInteraction",enabled);
    }
}
