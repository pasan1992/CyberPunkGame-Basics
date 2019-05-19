using UnityEngine;
using RootMotion.FinalIK;

namespace humanoid
{
    public class HumanoidDamageModule : DamageModule
    {
        // Start is called before the first frame update
        protected RagdollUtility m_ragdoll;
        protected HitReaction m_hitReaction;
        protected HumanoidAnimationModule m_animationSystem;
        protected Transform m_headTransform;
        protected Transform m_chestTransform;
        private bool m_ragdollEnabled = false;

        public HumanoidDamageModule(float health, RagdollUtility ragdoll, HitReaction hitReaction, HumanoidAnimationModule animationModule, Transform headTransfrom, Transform chestTransfrom, OnDestoryDeligate onDestroyCallback, Outline outline) : base(health, onDestroyCallback, outline)
        {
            m_ragdoll = ragdoll;
            m_hitReaction = hitReaction;
            m_headTransform = headTransfrom;
            m_chestTransform = chestTransfrom;
            m_animationSystem = animationModule;
        }

        #region update

        public void update()
        {
            if (m_animationSystem.isCrouched() && !m_animationSystem.isProperlyAimed() && HealthAvailable())
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
            m_ragdollEnabled = true;
            GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
            explosion.SetActive(true);
            explosion.transform.position = m_headTransform.position;
        }

        public void emitSmoke()
        {
            GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
            explosion.SetActive(true);
            explosion.transform.position = m_headTransform.position;

            GameObject smoke = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect);
            smoke.SetActive(true);
            smoke.transform.position = m_headTransform.position;
        }

        public override void resetCharacter(float health)
        {
            base.resetCharacter(health);
            if(m_ragdollEnabled)
            {
                m_ragdollEnabled = false;
                m_ragdoll.DisableRagdoll();
            }

        }

        #endregion

        #region getters and setters

        public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
        {
            m_hitReaction.Hit(collider, force, point);
        }

        public Transform getHeadTransfrom()
        {
            return m_headTransform;
        }

        public Transform getChestTransfrom()
        {
            return m_chestTransform;
        }

        public void toggleHeadTransfromCollider(bool enable)
        {
            Collider collider = m_headTransform.GetComponent<Collider>();
            collider.enabled = enable;
        }
        #endregion
    }

}

