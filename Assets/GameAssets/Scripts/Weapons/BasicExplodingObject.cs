using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplodingObject : MonoBehaviour
{
    public void explode()
    {
       GameObject explostion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.FireEXplosionParticle);
       explostion.transform.position = this.transform.position;
       explostion.SetActive(true);
       this.gameObject.SetActive(false);
       damgeAround();
    }

    private void damgeAround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 7);

        foreach(Collider hitCollider in hitColliders)
        {
                switch (hitCollider.tag)
                {
                    case "Enemy":
                    case "Player":
                    case "Head":
                    case "Chest":
                        hitOnEnemy(hitCollider);
                        break;
                    case "Wall":
                        break;
                    case "Cover":
                        break;
                    case "Floor":

                       break;
                }
        }
    }

    private void hitOnEnemy(Collider other)
    {
      ICyberAgent agent =  other.GetComponentInParent<ICyberAgent>();
      agent.reactOnHit(other, (this.transform.forward) * 3f, other.transform.position);

      if(agent.IsFunctional())
      {
        if(other.tag =="Head")
        {
            agent.damageAgent(2);
        }
        
      }
      else
      {
         Rigidbody rb = other.GetComponent<Rigidbody>();
         float chance = Random.value;
          if(rb && chance >0.5f)
          {
              rb.AddForce(Random.insideUnitCircle*Random.value*50,ForceMode.Impulse);
          }
      }

    }
}
