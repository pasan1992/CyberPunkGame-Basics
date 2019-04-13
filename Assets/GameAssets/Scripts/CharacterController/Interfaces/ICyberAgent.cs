﻿using System.Collections;
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
    void togglepSecondaryWeapon();
    void togglePrimaryWeapon();
    bool IsFunctional();
    string getNamge();
    void DestroyCharacter();
    Vector3 getCurrentPosition();
    void toggleHide();
    bool isEquiped();
    void pullTrigger();
    void releaseTrigger();
    void weaponFireForAI();
    string getName();
    void WeaponFireForAICover();
    void setWeponFireCapability(bool enadled);
}
