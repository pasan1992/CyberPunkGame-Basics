using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICyberAgent
{
    void enableTranslateMovment(bool enable);
    void moveCharacter(Vector3 movmentDirection);
    void AimWeapon();
    void StopAiming();
    void setTargetPoint(Vector3 position);
    void damageAgent(float amount);
    void reactOnHit(Collider collider, Vector3 force, Vector3 point);
    bool IsFunctional();
    string getNamge();
    void DestroyCharacter();
}
