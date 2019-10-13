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
}
