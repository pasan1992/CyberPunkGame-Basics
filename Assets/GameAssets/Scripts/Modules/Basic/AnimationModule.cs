using UnityEngine;

public class AnimationModule
{
    // Start is called before the first frame update
    protected Animator m_animator;
    
    public AnimationModule(Animator animator)
    {
        m_animator = animator;
    }

    public virtual void disableAnimationSystem()
    {
        m_animator.enabled = false;
    }

    public virtual void setMovment(float forward, float side)
    {
        m_animator.SetFloat("forward", forward);
        m_animator.SetFloat("side", side);
    }
}
