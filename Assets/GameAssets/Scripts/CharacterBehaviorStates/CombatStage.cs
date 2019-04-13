﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatStage : BasicMovmentStage
{
    private ICyberAgent target;
    private CoverPoint[] coverPoints;
    private CoverPoint currentCoverPoint;

    public enum CombatSubStages { LookingForCover,MovingToCover, InCover }
    private CombatSubStages currentCombatSubStage = CombatSubStages.LookingForCover;

    public enum CoverShootingSubStages { Cover,Peek,Shoot};
    private CoverShootingSubStages currentCoverShootingSubStage;

    // Parameters
    float fireRangeDistance = 12;

    #region initalize
    public CombatStage(ICyberAgent selfAgent,ICyberAgent target,NavMeshAgent navMeshAgent):base(selfAgent,navMeshAgent)
    {
        this.target = target;
        coverPoints = GameObject.FindObjectsOfType<CoverPoint>();
        selfAgent.toggleHide();
        selfAgent.AimWeapon();
        selfAgent.togglepSecondaryWeapon();
    }
    #endregion

    #region update

    // Main Update function that get called for every frame.
    public override void updateStage()
    {
        // This is needed for calling step update.
        base.updateStage();
        selfAgent.setTargetPoint(target.getCurrentPosition() + new Vector3(0,1.25f,0));
    }

    // Update function that get called for every 1 second.
    protected override void stepUpdate()
    {
        updateSubStages();
    }

    private void updateSubStages()
    {
        switch (currentCombatSubStage)
        {
            case CombatSubStages.InCover:

                if(currentCoverPoint.isSafeFromTarget() && currentCoverPoint.canFireToTarget(fireRangeDistance))
                {
                    switch(currentCoverShootingSubStage)
                    {
                        case CoverShootingSubStages.Cover:
                            selfAgent.AimWeapon();
                            currentCoverShootingSubStage = CoverShootingSubStages.Peek;
                            break;
                        case CoverShootingSubStages.Peek:
                            selfAgent.weaponFireForAI();
                            currentCoverShootingSubStage = CoverShootingSubStages.Shoot;
                            break;
                        case CoverShootingSubStages.Shoot:
                            currentCoverShootingSubStage = CoverShootingSubStages.Cover;
                            selfAgent.StopAiming();
                            break;
                    }
                }
                else
                {
                    currentCombatSubStage = CombatSubStages.LookingForCover;
                    setStepIntervalSize(1);
                }
                break;
            case CombatSubStages.LookingForCover:

                if(currentCoverPoint)
                {
                    currentCoverPoint.stPointOccupentsName("");
                }

                currentCoverPoint = closestCombatLocationAvaialbe();
                agent.SetDestination(currentCoverPoint.getPosition());
                currentCombatSubStage = CombatSubStages.MovingToCover;

                // Get up and move
                selfAgent.toggleHide();
                selfAgent.AimWeapon();
                break;

            case CombatSubStages.MovingToCover:

                if(!agent.pathPending && agent.remainingDistance >1)
                {
                    if(Vector3.Distance(selfAgent.getCurrentPosition(),target.getCurrentPosition())< fireRangeDistance)
                    {
                        selfAgent.weaponFireForAI();
                    }

                }
                else
                {
                    currentCombatSubStage = CombatSubStages.InCover;

                    // Get down on cover
                    selfAgent.toggleHide();
                    selfAgent.StopAiming();
                    setStepIntervalSize(0.5f);

                }
                break;
        }
    }


    #endregion

    #region Utility

    private CoverPoint closestCombatLocationAvaialbe()
    {
        float minimumDistanceToIdealCoverPoint = 999;
        float minimumDistanceToSafeCoverPoint = 999;

        CoverPoint tempIDealCoverPoint = null;
        CoverPoint tempSafeCOverPoint = null;


        foreach (CoverPoint point in coverPoints)
        {
            if (!point.isOccupied())
            {
                point.setTargetToCover(target);
                if(point.isSafeFromTarget())
                {

                    // Find the safe cover point.
                   if(minimumDistanceToSafeCoverPoint > point.distanceTo(selfAgent.getCurrentPosition()))
                    {
                        minimumDistanceToSafeCoverPoint = point.distanceTo(selfAgent.getCurrentPosition());
                        tempSafeCOverPoint = point;
                    }

                   // Find the ideal closest cover point.
                   if(point.canFireToTarget(fireRangeDistance))
                    {
                        if (minimumDistanceToIdealCoverPoint > point.distanceTo(selfAgent.getCurrentPosition()))
                        {
                            minimumDistanceToIdealCoverPoint = point.distanceTo(selfAgent.getCurrentPosition());
                            tempIDealCoverPoint = point;
                        }
                    }

                }
            }
        }

        if(tempIDealCoverPoint !=null)
        {
            tempIDealCoverPoint.stPointOccupentsName(selfAgent.getName());
            return tempIDealCoverPoint;
        }
        else
        {
            tempSafeCOverPoint.stPointOccupentsName(selfAgent.getName());
            return tempSafeCOverPoint;
        }
    }
    
    private IEnumerator weaponFire()
    {
        if(currentCombatSubStage.Equals(CombatSubStages.InCover))
        {
            selfAgent.AimWeapon();
            yield return new WaitForSeconds(0.5f);
            selfAgent.pullTrigger();
            yield return new WaitForSeconds(0.5f);
            selfAgent.releaseTrigger();
            selfAgent.StopAiming();
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
            selfAgent.pullTrigger();
            yield return new WaitForSeconds(0.2f);
            selfAgent.releaseTrigger();
            yield return new WaitForSeconds(0.2f);
        }
    }

    #endregion

    #region Getters and Setters

    public override void setWeaponFireCapability(bool enabled)
    {
        selfAgent.setWeponFireCapability(enabled);
    }

    public override void setNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        throw new System.NotImplementedException();
    }

    public override void setTargets(ICyberAgent target)
    {
        this.target = target;
    }

    public override void setStepIntervalSize(float timeInSeconds)
    {
        stepIntervalInSeconds = timeInSeconds;
    }

    private bool isCoverPointUsable(CoverPoint point)
    {
        return point.isSafeFromTarget() && point.canFireToTarget(fireRangeDistance);
    }

    public override void stopStageBehavior()
    {
        agent.isStopped = true;
    }
    #endregion


}
