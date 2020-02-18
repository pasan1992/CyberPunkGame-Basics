using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableItem : MonoBehaviour,DamagableObject
{
    public float Total_Health;
    private float m_remaning_Health;
    public ProjectilePool.POOL_OBJECT_TYPE particleEffectOnDestroy;
    public Color color;

    public void Awake()
    {
        m_remaning_Health = Total_Health;
    }
    public bool damage(float damageValue, Collider collider, Vector3 force, Vector3 point)
    {
        if(!isDestroyed())
        {
            m_remaning_Health -= damageValue;

            if(m_remaning_Health <= 0)
            {
                m_remaning_Health = 0;
                GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(particleEffectOnDestroy);
                basicHitParticle.SetActive(true);
                basicHitParticle.transform.position = point;
                basicHitParticle.transform.LookAt(Vector3.up);
                basicHitParticle.transform.localScale = new Vector3(0.6f,0.6f,0.6f);

                // ParticleSystem.MainModule main = basicHitParticle.GetComponent<ParticleSystem>().main;
                // main.startColor = color; // <- or whatever color you want to assign
                // basicHitParticle.GetComponent<ParticleSystem>().Play();

                Destroy(this.gameObject);
                return true;
            }       
        }
        return false;
    }

    public float getArmor()
    {
        return 0;
    }

    public float getRemaningHealth()
    {
        return m_remaning_Health;
    }

    public float getTotalHealth()
    {
        return Total_Health;
    }

    public Transform getTransfrom()
    {
        return this.transform;
    }

    public bool isDestroyed()
    {
        return m_remaning_Health == 0;
    }
}
