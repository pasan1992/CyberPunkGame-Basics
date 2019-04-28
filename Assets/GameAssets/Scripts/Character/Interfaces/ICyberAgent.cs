using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICyberAgent
{
    void enableTranslateMovment(bool enable);
    void moveCharacter(Vector3 movmentDirection);
    void aimWeapon();
    void stopAiming();
    void setTargetPoint(Vector3 position);
    void damageAgent(float amount);
    void reactOnHit(Collider collider, Vector3 force, Vector3 point);
    void togglepSecondaryWeapon();
    void togglePrimaryWeapon();
    bool IsFunctional();
    //void DestroyCharacter();
    Vector3 getCurrentPosition();
    void toggleHide();
    bool isEquiped();
    void pullTrigger();
    void releaseTrigger();
    void weaponFireForAI();
    string getName();
    void weaponFireForAICover();
    void setWeponFireCapability(bool enadled);
    void setHealth(float value);
    Transform getTransfrom();
    void dodgeAttack(Vector3 dodgeDirection);
    void lookAtTarget();
    Vector3 getTopPosition();
    AgentController.AgentFaction getFaction();
    void setFaction(AgentController.AgentFaction group);
}
