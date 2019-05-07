using UnityEngine;

public class DroneDamageModule : DamageModule
{
    public DroneDamageModule(float health,Outline outline,OnDestoryDeligate onDestroyCallback):base(health, onDestroyCallback,outline)
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
