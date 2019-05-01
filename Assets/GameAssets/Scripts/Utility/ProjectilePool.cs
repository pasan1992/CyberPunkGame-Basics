using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    // Basic Projectile 
    private List<GameObject> basicProjectiles;
    private int maxBulletCount = 50;

    // Basic Explosion
    //private GameObject basicExplosionParticlePrefab;
    //private GameObject basicDroneExplosionPrefab;

    private List<GameObject> basicFireExplosionParticles;
    private List<GameObject> basicDroneExplosion;


    private int maxExplosions = 30;
    private int donreExplosions = 30;

    private static ProjectilePool thisProjectilePool;

    #region initialize

    void Awake()
    {
        initalizeBasicProjectile();
        initalizeBasicExplosionParticle();
        initalizeDroneExplosions();
    }

    private void initalizeDroneExplosions()
    {
        GameObject basicDroneExplosionPrefab = Resources.Load<GameObject>("Explosions/BasicDroneExplosion");
        basicDroneExplosion = new List<GameObject>();

        for (int i = 0; i < donreExplosions; i++)
        {
            GameObject explosion = GameObject.Instantiate(basicDroneExplosionPrefab);
            explosion.SetActive(false);
            basicDroneExplosion.Add(explosion);
        }
    }

    private void initalizeBasicExplosionParticle()
    {
        GameObject basicExplosionParticlePrefab = Resources.Load<GameObject>("ParticleEffects/Explosion_fire");

        basicFireExplosionParticles = new List<GameObject>();

        for (int i = 0; i < maxExplosions; i++)
        {
            GameObject explosion = GameObject.Instantiate(basicExplosionParticlePrefab);
            explosion.SetActive(false);
            basicFireExplosionParticles.Add(explosion);
        }
    }

    private void initalizeBasicProjectile()
    {
        GameObject basicProjectilePrefab = Resources.Load<GameObject>("Prefab/LaserBeamProjectile");
        basicProjectiles = new List<GameObject>();

        for (int i = 0; i < maxBulletCount; i++)
        {
            GameObject projectile = GameObject.Instantiate(basicProjectilePrefab);
            projectile.SetActive(false);
            basicProjectiles.Add(projectile);
        }
    }
    #endregion


    #region getters and setters
    public GameObject getBasicProjectie()
    {
        foreach (GameObject projectile in basicProjectiles)
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
        foreach (GameObject explosion in basicFireExplosionParticles)
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
        foreach (GameObject explosion in basicDroneExplosion)
        {
            if (!explosion.activeInHierarchy)
            {
                return explosion;
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
