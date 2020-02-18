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
    // void togglepSecondaryWeapon();
    // void togglePrimaryWeapon();
    bool IsFunctional();
    //void DestroyCharacter();
    Vector3 getCurrentPosition();
    void toggleHide();
    bool isReadyToAim();
    void weaponFireForAI();
    void setWeponFireCapability(bool enadled);
    Transform getTransfrom();
    void dodgeAttack(Vector3 dodgeDirection);
    void lookAtTarget();
    Vector3 getTopPosition();
    AgentBasicData.AgentFaction getFaction();
    void setFaction(AgentBasicData.AgentFaction group);
    float getSkill();
    void setOnDestoryCallback(GameEvents.BasicNotifactionEvent callback);
    void setOnDisableCallback(GameEvents.BasicNotifactionEvent callback);
    void setOnEnableCallback(GameEvents.BasicNotifactionEvent callback);
    bool isDisabled();
    void resetAgent();
    Vector3 getMovmentDirection();
    Vector3 getCurrentVelocity();
    bool isAimed();
    bool isArmed();
    bool isHidden();
    GameObject getGameObject();
    AgentData GetAgentData();
    bool isInteracting();
    void interactWith(Interactable interactableObj,Interactable.InteractableProperties.InteractableType type);
    void setOnDamagedCallback(GameEvents.BasicNotifactionEvent callback);
    IEnumerator waitTillUnarmed();
    void cancleInteraction();
}
