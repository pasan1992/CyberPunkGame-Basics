using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplosion : MonoBehaviour
{
    // Start is called before the first frame update

    #region Initialize

    protected Rigidbody[] explosionParticles;
    //ParticleSystem m_particleSystem;
    void Awake()
    {
       explosionParticles =  this.GetComponentsInChildren<Rigidbody>();
    }

    protected virtual void resetAll()
    {
        foreach(Rigidbody rb in explosionParticles)
        {
            rb.transform.localPosition = Vector3.zero;
            //rb.Sleep();
            rb.gameObject.SetActive(false);
            rb.transform.parent = this.transform;
        }

        //if(m_particleSystem != null)
        //{
        //    m_particleSystem.Stop();
        //    m_particleSystem.gameObject.SetActive(false);
        //}
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Commands

    public virtual void exploade()
    {
        foreach (Rigidbody rb in explosionParticles)
        {
            rb.gameObject.SetActive(true);
            rb.gameObject.transform.parent = null;
            rb.transform.transform.position = this.transform.position;
            //rb.WakeUp();
            rb.AddForce(Random.insideUnitSphere *(Random.value * 5+5), ForceMode.Impulse);
            rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * (Random.value * 5 + 5), ForceMode.Impulse);
        }

        getExplosionParticleEffect();

        Invoke("resetAll", 3f);

    }

    #endregion

    #region getters and setters

    protected virtual void getExplosionParticleEffect()
    {
        //.getBasicFireExplosionParticle();
        GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.FireEXplosionParticle);
        explosion.SetActive(true);
        explosion.transform.position = this.transform.position;
    }

    #endregion
}
