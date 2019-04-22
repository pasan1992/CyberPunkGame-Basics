using UnityEngine;
using RootMotion.FinalIK;


public class HumanoidDamageModule : DamageModule
{
    // Start is called before the first frame update
    protected RagdollUtility m_ragdoll;
    protected HitReaction m_hitReaction;
    protected HumanoidAnimationModule m_animationSystem;

    public HumanoidDamageModule(float health,RagdollUtility ragdoll, HitReaction hitReaction,OnDestoryDeligate onDestroyCallback):base(health,onDestroyCallback)
    {
        m_ragdoll = ragdoll;
        m_hitReaction = hitReaction;
    }

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
    #endregion
}
