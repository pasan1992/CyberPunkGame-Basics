using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    // Basic Projectile 
    private List<GameObject> basicProjectilesList;
    private int maxBulletCount = 20;

    private List<GameObject> basicFireExplosionParticlesList;
    private List<GameObject> basicDroneExplosionList;
    private List<GameObject> bulletHitBasicParticleList;


    private int maxExplosions = 10;
    private int donreExplosions = 10;

    private static ProjectilePool thisProjectilePool;

    #region initialize

    void Awake()
    {
        initalizeBulletHitBasicParticleList();
        initalizeBasicProjectile();
        initalizeBasicExplosionParticle();
        initalizeDroneExplosions();
    }

    private void initalizeBulletHitBasicParticleList()
    {
        GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>("ParticleEffects/BulletHitBasicParticle");
        bulletHitBasicParticleList = new List<GameObject>();

        for (int i = 0; i < maxBulletCount; i++)
        {
            GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
            bulletHitParticle.transform.parent = this.transform;
            bulletHitParticle.SetActive(false);
            bulletHitBasicParticleList.Add(bulletHitParticle);
        }
    }

    private void initalizeDroneExplosions()
    {
        GameObject basicDroneExplosionPrefab = Resources.Load<GameObject>("Explosions/BasicDroneExplosion");
        basicDroneExplosionList = new List<GameObject>();

        for (int i = 0; i < donreExplosions; i++)
        {
            GameObject explosion = GameObject.Instantiate(basicDroneExplosionPrefab);
            explosion.transform.parent = this.transform;
            explosion.SetActive(false);
            basicDroneExplosionList.Add(explosion);
        }
    }

    private void initalizeBasicExplosionParticle()
    {
        GameObject basicExplosionParticlePrefab = Resources.Load<GameObject>("ParticleEffects/Explosion_fire");

        basicFireExplosionParticlesList = new List<GameObject>();

        for (int i = 0; i < maxExplosions; i++)
        {
            GameObject explosion = GameObject.Instantiate(basicExplosionParticlePrefab);
            explosion.transform.parent = this.transform;
            explosion.SetActive(false);
            basicFireExplosionParticlesList.Add(explosion);
        }
    }

    private void initalizeBasicProjectile()
    {
        GameObject basicProjectilePrefab = Resources.Load<GameObject>("Prefab/LaserBeamProjectile");
        basicProjectilesList = new List<GameObject>();

        for (int i = 0; i < maxBulletCount; i++)
        {
            GameObject projectile = GameObject.Instantiate(basicProjectilePrefab);
            projectile.transform.parent = this.transform;
            projectile.SetActive(false);
            basicProjectilesList.Add(projectile);
        }
    }
    #endregion


    #region getters and setters
    public GameObject getBasicProjectie()
    {
        foreach (GameObject projectile in basicProjectilesList)
        {
            if(!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }

        return null;
    }

    public GameObject getBasicFireExplosionParticle()
    {
        foreach (GameObject explosion in basicFireExplosionParticlesList)
        {
            if(!explosion.activeInHierarchy)
            {
                return explosion;
            }
        }

        return null;
    }

    public GameObject getBasicDroneExplosion()
    {
        foreach (GameObject explosion in basicDroneExplosionList)
        {
            if (!explosion.activeInHierarchy)
            {
                return explosion;
            }
        }

        return null;
    }

    public GameObject getBulletHitBasicParticle()
    {
        foreach (GameObject bulletHitParticle in bulletHitBasicParticleList)
        {
            if (!bulletHitParticle.activeInHierarchy)
            {
                return bulletHitParticle;
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
