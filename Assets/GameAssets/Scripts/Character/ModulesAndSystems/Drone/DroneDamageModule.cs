using UnityEngine;
public class DroneDamageModule : DamageModule
{
    public DroneDamageModule(AgentBasicData basicData,Outline outline,OnDestoryDeligate onDestroyCallback): base(basicData, onDestroyCallback,outline)
    {
    }

    public void ExplosionEffect(Vector3 position)
    {
        //.getBasicDroneExplosion()
        BasicExplosion droneExplosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroneExplosion).GetComponent<BasicExplosion>();
        droneExplosion.gameObject.SetActive(true);
        droneExplosion.gameObject.transform.position = position;
        droneExplosion.GetComponent<BasicExplosion>().exploade();
    }

    public void DisableDrone(Vector3 position)
    {
        GameObject electricParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
        electricParticle.gameObject.SetActive(true);
        electricParticle.gameObject.transform.position = position;
    }
}
