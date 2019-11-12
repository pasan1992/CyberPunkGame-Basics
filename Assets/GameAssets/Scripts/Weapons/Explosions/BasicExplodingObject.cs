using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplodingObject : MonoBehaviour
{
     [SerializeField] 
    private float m_baseDamage;

     [SerializeField] 
    private float m_range;

    public float BaseDamage { get => m_baseDamage; set => m_baseDamage = value; }
    public float Range { get => m_range; set => m_range = value; }

    public void explode()
    {
        explode(ProjectilePool.POOL_OBJECT_TYPE.FireEXplosionParticle);
    }   

    public void explode(ProjectilePool.POOL_OBJECT_TYPE explosionType)
    {
       GameObject explostion = ProjectilePool.getInstance().getPoolObject(explosionType);
       explostion.transform.position = this.transform.position;
       explostion.SetActive(true);
       damgeAround();
       this.gameObject.SetActive(false);
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
      Vector3 direction;
      float damagePropotion = DamageCalculator.getExplosionDamgage(this.transform.position,other.transform.position,m_range,out direction);
      if(damagePropotion > 0 && agent !=null)
      {
        if(agent.IsFunctional())
        { 
            if(other.tag =="Chest")
            {   
                if(!DamageCalculator.isSafeFromTarget(this.transform.position,other.transform.position,m_range))
                {
                    agent.damageAgent(m_baseDamage*damagePropotion);
                }           
            }
        }
        else
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            
            float chance = Random.value;

            if(rb && chance >0.5f)
            {
                rb.AddForce(direction*damagePropotion*BaseDamage*10,ForceMode.Impulse);
            }
        }
      }
    }
}
