using UnityEngine;

public interface DamagableObject
{
    Transform getTransfrom();
    bool damage(float damageValue,Collider collider, Vector3 force, Vector3 point);

    bool isDestroyed();

    float getRemaningHealth();

    float getTotalHealth();

    float getArmor();
}
