using UnityEngine;

public class DroneDamageModule : DamageModule
{
    public DroneDamageModule(float health,OnDestoryDeligate onDestroyCallback):base(health, onDestroyCallback)
    {
        
    }

    public void ExplosionEffect(Vector3 position)
    {
        BasicExplosion droneExplosion = ProjectilePool.getInstance().getBasicDroneExplosion().GetComponent<BasicExplosion>();
        droneExplosion.gameObject.SetActive(true);
        droneExplosion.gameObject.transform.position = position;
        droneExplosion.GetComponent<BasicExplosion>().exploade();
    }
}
