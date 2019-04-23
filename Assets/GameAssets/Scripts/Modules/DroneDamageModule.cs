using UnityEngine;

public class DroneDamageModule : DamageModule
{
    ParticleSystem m_particleSystem;

    public DroneDamageModule(float health,OnDestoryDeligate onDestroyCallback):base(health, onDestroyCallback)
    {
    }

    public void ExplosionEffect( Vector3 explodePosition)
    {
       GameObject explosion =  ProjectilePool.getInstance().getBasicExplosion();
       explosion.SetActive(true);
       explosion.transform.position = explodePosition;
       m_particleSystem = explosion.GetComponent<ParticleSystem>();
       m_particleSystem.Play();
    }

    private void disableParticleSystem()
    {
        m_particleSystem.Stop();
        m_particleSystem.gameObject.SetActive(false);
    }
}
