using UnityEngine;

public interface DamagableObject
{
    Transform getTransfrom();
    bool damage(float damageValue);

    bool isDestroyed();

    float getRemaningHealth();

    float getTotalHealth();

    float getArmor();
}
