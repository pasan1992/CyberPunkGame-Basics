using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosionObject : BasicExplodingObject
{
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Enemy":
            case "Player":
            case "Head":
            case "Chest":
                explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
                //hitOnEnemy(other);
                break;
            case "Wall":
                explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
                //hitOnWall(other);
                break;
            case "Cover":
                 explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
                //hitOnCOver(other);
                break;
            case "Floor":
                explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
                //hitOnFloor(other);
                break;
        }

    }
}
