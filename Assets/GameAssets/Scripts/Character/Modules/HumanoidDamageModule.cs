using UnityEngine;
using RootMotion.FinalIK;


public class HumanoidDamageModule : DamageModule
{
    // Start is called before the first frame update
    protected RagdollUtility m_ragdoll;
    protected HitReaction m_hitReaction;
    protected HumanoidAnimationModule m_animationSystem;
    protected Transform m_headTransfrom;

    public HumanoidDamageModule(float health,RagdollUtility ragdoll, HitReaction hitReaction,HumanoidAnimationModule animationModule,Transform headTransfrom,OnDestoryDeligate onDestroyCallback,Outline outline):base(health,onDestroyCallback,outline)
    {
        m_ragdoll = ragdoll;
        m_hitReaction = hitReaction;
        m_headTransfrom = headTransfrom;
        m_animationSystem = animationModule;
    }

    #region update

    public void update()
    {
        if(m_animationSystem.isCrouched() && !m_animationSystem.isProperlyAimed())
        {
            toggleHeadTransfromCollider(false);
        }
        else
        {
            toggleHeadTransfromCollider(true);
        }
    }

    #endregion

    #region Commands

    public override void destroyCharacter()
    {
        m_ragdoll.EnableRagdoll();
    }

    #endregion

    #region getters and setters

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        m_hitReaction.Hit(collider, force, point);
    }

    public Transform getHeadTransfrom()
    {
        return m_headTransfrom;
    }

    public void toggleHeadTransfromCollider(bool enable)
    {
        Collider collider = m_headTransfrom.GetComponent<Collider>();
        collider.enabled = enable;
    }
    #endregion
}
