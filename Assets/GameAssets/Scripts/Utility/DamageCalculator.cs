using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    private static readonly float s_maximumHitReactionValue = 2.5f;
    // public static void SetDamageFromExplosion(ICyberAgent agent,BasicExplodingObject explosionObject,Collider hitObject)
    // {
    //     Vector3 forceDirection = (agent.getCurrentPosition() - explosionObject.transform.position);
    //     float distance = forceDirection.magnitude;
    //     forceDirection = forceDirection.normalized;

    //     if(distance < explosionObject.Range)
    //     {
    //         float damageProposion =  (1- (distance/explosionObject.Range));
    //         agent.damageAgent(explosionObject.BaseDamage*damageProposion);
    //         agent.reactOnHit(hitObject, forceDirection*s_maximumHitReactionValue*damageProposion,hitObject.transform.position);
    //     }
    // }

    public static float getExplosionDamgage(Vector3 explosionPositon,Vector3 targetPosition,float explosionMaxRange , out Vector3 direction)
    {
        
        direction = targetPosition - explosionPositon;
        float distance = direction.magnitude;
        direction = direction.normalized;

        if(distance < explosionMaxRange)
        {
            //return  (1- (distance/explosionMaxRange));
            return 1;
        }

        return 0;
               
    }

    public static bool isSafeFromTarget(Vector3 explosionPosition, Vector3 target, float range)
    {
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles","FullCoverObsticles" };
        if (Physics.Raycast(explosionPosition, target - explosionPosition, out hit,range, LayerMask.GetMask(layerMaskNames)))
        {
            if(hit.transform.tag =="Cover" || hit.transform.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public static void hitOnWall(Collider wall,Vector3 hitPositon)
    {
            GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
            basicHitParticle.SetActive(true);
            basicHitParticle.transform.position = hitPositon;
            basicHitParticle.transform.LookAt(Vector3.up);
    }
    public static void onHitEnemy(Collider other,AgentBasicData.AgentFaction m_fireFrom,Vector3 hitDirection)
    {
        AgentController agentController = other.transform.GetComponentInParent<AgentController>();
        if (agentController != null)
        {
            ICyberAgent cyberAgent = agentController.getICyberAgent();
            if (cyberAgent !=null && !m_fireFrom.Equals(cyberAgent.getFaction()))
            {

                //cyberAgent.reactOnHit(other, (hitDirection) * 3f, other.transform.position);
                //cyberAgent.damageAgent(1);
                agentController.GetComponent<HumanoidDamagableObject>().damage(1,other,(hitDirection) * 3f,other.transform.position);
            
                GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
                basicHitParticle.SetActive(true);
                basicHitParticle.transform.position = other.transform.position;
                basicHitParticle.transform.LookAt(Vector3.up);
 
                if (!cyberAgent.IsFunctional())
                {
                    HumanoidMovingAgent movingAgent = cyberAgent as HumanoidMovingAgent;
                    if(movingAgent !=null)
                    {
                        Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                        if (rb != null)
                        {
                            rb.isKinematic = false;
                            rb.AddForce((hitDirection) * 150, ForceMode.Impulse);
                        }

                        Rigidbody hitRb =  movingAgent.getChestTransfrom().GetComponent<Rigidbody>();

                        if(hitRb)
                        {
                            hitRb.AddForce((hitDirection) * 2 + Random.insideUnitSphere*2, ForceMode.Impulse);
                        }

                    }
                    else
                    {
                       basicHitParticle.transform.position = cyberAgent.getTopPosition();
                    }
                }
            }

        }
    }

    public static void onHitEnemy2(Collider other,AgentBasicData.AgentFaction m_fireFrom,Vector3 hitDirection)
    {
        DamagableObject damagableObject = other.transform.GetComponentInParent<DamagableObject>();
        if (damagableObject != null)
        {
            MovingAgentDamagableObject movingDamagableObject = (MovingAgentDamagableObject)damagableObject;
            if (movingDamagableObject !=null && movingDamagableObject.isDamagable(m_fireFrom))
            {

               // cyberAgent.reactOnHit(other, (hitDirection) * 3f, other.transform.position);
                //cyberAgent.damageAgent(1);
               movingDamagableObject.damage(1,other,(hitDirection) * 3f,other.transform.position);
            
                GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
                basicHitParticle.SetActive(true);
                basicHitParticle.transform.position = other.transform.position;
                basicHitParticle.transform.LookAt(Vector3.up);
 
                if (movingDamagableObject.isDestroyed())
                {
                    Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddForce((hitDirection) * 150, ForceMode.Impulse);
                    }
                }
            }

        }
    }
}
