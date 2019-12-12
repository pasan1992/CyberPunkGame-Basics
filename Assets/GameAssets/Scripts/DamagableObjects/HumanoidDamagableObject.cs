using UnityEngine;
public class HumanoidDamagableObject : MovingAgentDamagableObject
{
    public GameObject currentAvatar;

    public GameObject[] avatar;

    private bool destroyed = false;

    private int postDestroyDamage = 0 ;

    public override bool damage(float damageValue,Collider collider, Vector3 force, Vector3 point)
    {
        m_movingAgent.damageAgent(damageValue);
        m_movingAgent.reactOnHit(collider,force,point);
        
        if(getRemaningHealth() == 0)
        {
            if(!destroyed)
            {
                destroyed = true;
                GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
                explosion.transform.position = this.transform.position;
                explosion.SetActive(true);
                explosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Head);
                currentAvatar.SetActive(false);
                avatar[0].SetActive(true);
                Invoke("postDestoryEffect",1);
            }
            else if(m_movingAgent.GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID)
            {
                GameObject postDestoryExplosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
                postDestoryExplosion.transform.position = collider.transform.position;
                postDestoryExplosion.SetActive(true);

                GameObject postDestoryExplosion2 = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
                postDestoryExplosion2.transform.position = collider.transform.position;
                postDestoryExplosion2.SetActive(true);
                switch (postDestroyDamage)
                {
                    case 2:
                        postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Hands);
                        postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                        //postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Legs);
                        avatar[0].SetActive(false);
                        avatar[1].SetActive(true);
                    break;
                    case 4:
                        postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
                        postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                        avatar[1].SetActive(false);
                        //avatar[2].SetActive(true);
                    break;
                    // case 3:
                    //     postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
                    //     headlessAvatar[2].SetActive(false);
                    // break;
                }

                postDestroyDamage++;
                
            }

        }

        return !m_movingAgent.IsFunctional();
    }

    private void postDestoryEffect()
    {
        GameObject smoke = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
        GameObject electricEffet = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect);
        smoke.SetActive(true);
        electricEffet.SetActive(true);
        smoke.transform.position = this.transform.position;
        electricEffet.transform.position = this.transform.position;
    }
}