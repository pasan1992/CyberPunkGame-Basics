using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public enum POOL_OBJECT_TYPE {
        FireEXplosionParticle,

        RocketExplosionParticle,
        DroneExplosion,
        HitBasicParticle,
        BasicProjectile,
        ElectricParticleEffect,
        SmokeEffect,
        PistolAmmo,
        RifleAmmo,
        Grenade,
        BasicRocket,
        DroidExplosionParticleEffect
    }

    // Basic Projectile 
    private List<GameObject> basicProjectilesList;
    public int maxBulletCount = 20;

    // Explosions
    private List<GameObject> basicDroneExplosionList;

    // Particle effects
    private List<GameObject> basicFireExplosionParticlesList;

    private List<GameObject> basicRocketExplosionParticleList;
    private List<GameObject> bulletHitBasicParticleList;
    private List<GameObject> electricParticleEffectList;
    private List<GameObject> smokeEffectList;

    private List<GameObject> basicRocketList;

    private List<GameObject> droidExplosionsList;


    public int maxExplosions = 10;
    public int donreExplosions = 10;

    private static ProjectilePool thisProjectilePool;

    public int ammoCount = 5;
    private List<GameObject> pistolAmmoList;
    private List<GameObject> rifleAmmoList;

    public List<GameObject> grenadeList;

    #region initialize

    void Awake()
    {
        //initalizeBulletHitBasicParticleList();
        //initalizeBasicProjectile();
        //initalizeBasicExplosionParticle();
        //initalizeDroneExplosions();

        foreach (POOL_OBJECT_TYPE type in System.Enum.GetValues(typeof(POOL_OBJECT_TYPE)))
        {
            initalziePool(type);
        }
    }

    private void initalziePool(POOL_OBJECT_TYPE typeofEffect)
    {
        string resourcePath = "";
        List<GameObject> effectList = null;
        int count =0;

        switch (typeofEffect)
        {
            case POOL_OBJECT_TYPE.FireEXplosionParticle:
                resourcePath = "ParticleEffects/Explosion_fire";
                count = maxExplosions;
                basicFireExplosionParticlesList = new List<GameObject>();
                effectList = basicFireExplosionParticlesList;
                break;
            case POOL_OBJECT_TYPE.RocketExplosionParticle:
                resourcePath = "ParticleEffects/RocketExplosionParticle";
                count = maxExplosions;
                basicRocketExplosionParticleList = new List<GameObject>();
                effectList = basicRocketExplosionParticleList;
                break;
            case POOL_OBJECT_TYPE.DroneExplosion:
                resourcePath = "Explosions/BasicDroneExplosion";
                count = donreExplosions;
                basicDroneExplosionList = new List<GameObject>();
                effectList = basicDroneExplosionList;
                break;
            case POOL_OBJECT_TYPE.HitBasicParticle:
                count = maxExplosions;
                resourcePath = "ParticleEffects/BulletHitBasicParticle";
                bulletHitBasicParticleList = new List<GameObject>();
                effectList = bulletHitBasicParticleList;
                break;
            case POOL_OBJECT_TYPE.BasicProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/LaserBeamProjectile";
                basicProjectilesList = new List<GameObject>();
                effectList = basicProjectilesList;
                break;
            case POOL_OBJECT_TYPE.ElectricParticleEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/ElectricShock";
                electricParticleEffectList = new List<GameObject>();
                effectList = electricParticleEffectList;
                break;
            case POOL_OBJECT_TYPE.SmokeEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/SmokeParticleEffect";
                smokeEffectList = new List<GameObject>();
                effectList = smokeEffectList;
                break;
            case POOL_OBJECT_TYPE.RifleAmmo:
                count = ammoCount;
                resourcePath = "Drops/Rifle_Mag";
                rifleAmmoList = new List<GameObject>();
                effectList = rifleAmmoList;
                break;
            case POOL_OBJECT_TYPE.PistolAmmo:
                count = ammoCount;
                resourcePath = "Drops/Pistol_Mag";
                pistolAmmoList = new List<GameObject>();
                effectList = pistolAmmoList;
                break;
            case POOL_OBJECT_TYPE.Grenade:
                count = ammoCount;
                resourcePath = "Prefab/Grenede_throwObject";
                grenadeList = new List<GameObject>();
                effectList = grenadeList;  
                break;
            case POOL_OBJECT_TYPE.BasicRocket:
                count = ammoCount;
                resourcePath = "Prefab/BasicRocket"; 
                basicRocketList = new List<GameObject>();
                effectList = basicRocketList;       
                break;
            case POOL_OBJECT_TYPE.DroidExplosionParticleEffect:
                resourcePath = "Explosions/BasicDroidExplosion";
                count = donreExplosions;
                droidExplosionsList = new List<GameObject>();
                effectList = droidExplosionsList;
                break;
        }

        GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>(resourcePath);
        
        for (int i = 0; i < count; i++)
        {
            GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
            bulletHitParticle.transform.parent = this.transform;
            bulletHitParticle.SetActive(false);
            effectList.Add(bulletHitParticle);
        }
    }

    #region Not Using
    // private void initalizeBulletHitBasicParticleList()
    // {
    //     GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>("ParticleEffects/BulletHitBasicParticle");
    //     bulletHitBasicParticleList = new List<GameObject>();

    //     for (int i = 0; i < maxBulletCount; i++)
    //     {
    //         GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
    //         bulletHitParticle.transform.parent = this.transform;
    //         bulletHitParticle.SetActive(false);
    //         bulletHitBasicParticleList.Add(bulletHitParticle);
    //     }
    // }

    // private void initalizeDroneExplosions()
    // {
    //     GameObject basicDroneExplosionPrefab = Resources.Load<GameObject>("Explosions/BasicDroneExplosion");
    //     basicDroneExplosionList = new List<GameObject>();

    //     for (int i = 0; i < donreExplosions; i++)
    //     {
    //         GameObject explosion = GameObject.Instantiate(basicDroneExplosionPrefab);
    //         explosion.transform.parent = this.transform;
    //         explosion.SetActive(false);
    //         basicDroneExplosionList.Add(explosion);
    //     }
    // }

    // private void initalizeBasicExplosionParticle()
    // {
    //     GameObject basicExplosionParticlePrefab = Resources.Load<GameObject>("ParticleEffects/Explosion_fire");

    //     basicFireExplosionParticlesList = new List<GameObject>();

    //     for (int i = 0; i < maxExplosions; i++)
    //     {
    //         GameObject explosion = GameObject.Instantiate(basicExplosionParticlePrefab);
    //         explosion.transform.parent = this.transform;
    //         explosion.SetActive(false);
    //         basicFireExplosionParticlesList.Add(explosion);
    //     }
    // }

    // private void initalizeBasicProjectile()
    // {
    //     GameObject basicProjectilePrefab = Resources.Load<GameObject>("Prefab/LaserBeamProjectile");
    //     basicProjectilesList = new List<GameObject>();

    //     for (int i = 0; i < maxBulletCount; i++)
    //     {
    //         GameObject projectile = GameObject.Instantiate(basicProjectilePrefab);
    //         projectile.transform.parent = this.transform;
    //         projectile.SetActive(false);
    //         basicProjectilesList.Add(projectile);
    //     }
    // }
    #endregion

    #endregion


    #region getters and setters

    public GameObject getPoolObject(POOL_OBJECT_TYPE type)
    {
        List<GameObject> effectList = null;

        switch (type)
        {
            case POOL_OBJECT_TYPE.FireEXplosionParticle:
                effectList = basicFireExplosionParticlesList;
                break;
            case POOL_OBJECT_TYPE.RocketExplosionParticle:
                effectList = basicRocketExplosionParticleList;
                break;
            case POOL_OBJECT_TYPE.DroneExplosion:
                effectList = basicDroneExplosionList;
                break;
            case POOL_OBJECT_TYPE.HitBasicParticle:
                effectList = bulletHitBasicParticleList;
                break;
            case POOL_OBJECT_TYPE.BasicProjectile:
                effectList = basicProjectilesList;
                break;
            case POOL_OBJECT_TYPE.ElectricParticleEffect:
                effectList = electricParticleEffectList;
                break;
            default:
                effectList = null;
                break;
            case POOL_OBJECT_TYPE.SmokeEffect:
                effectList = smokeEffectList;
                break;
            case POOL_OBJECT_TYPE.RifleAmmo:
                effectList = rifleAmmoList;
                break;
            case POOL_OBJECT_TYPE.PistolAmmo:
                effectList = pistolAmmoList;
                break;
            case POOL_OBJECT_TYPE.Grenade:
                effectList = grenadeList;
                break;
            case POOL_OBJECT_TYPE.BasicRocket:
                effectList = basicRocketList;
                break;
            case POOL_OBJECT_TYPE.DroidExplosionParticleEffect:
                effectList = droidExplosionsList;
                break;
        }

        foreach (GameObject projectile in effectList)
        {
            if (!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }

        return null;
    }


    public static ProjectilePool getInstance()
    {
        if(thisProjectilePool ==null)
        {
            thisProjectilePool = GameObject.FindObjectOfType<ProjectilePool>();
        }

        return thisProjectilePool;
    }
    #endregion
}
