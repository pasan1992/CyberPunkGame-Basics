using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    // Basic Projectile 
    private GameObject basicProjectilePrefab;
    private List<GameObject> basicProjectiles;
    private int maxBulletCount = 50;

    // Basic Explosion
    private GameObject basicExplosionPrefab;
    public List<GameObject> basicExplosions;
    private int maxExplosions = 10;

    private static ProjectilePool thisProjectilePool;

    #region initialize

    void Awake()
    {
        initalizeBasicProjectile();

        initalizeBasicExplosion();
    }

    private void initalizeBasicExplosion()
    {
        basicExplosionPrefab = Resources.Load<GameObject>("ParticleEffects/Explosion_fire");

        basicExplosions = new List<GameObject>();

        for (int i = 0; i < maxExplosions; i++)
        {
            GameObject explosion = GameObject.Instantiate(basicExplosionPrefab);
            explosion.SetActive(false);
            basicExplosions.Add(explosion);
        }
    }

    private void initalizeBasicProjectile()
    {
        basicProjectilePrefab = Resources.Load<GameObject>("Prefab/LaserBeamProjectile");
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

    public GameObject getBasicExplosion()
    {
        foreach (GameObject explosion in basicExplosions)
        {
            if(!explosion.activeInHierarchy)
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
